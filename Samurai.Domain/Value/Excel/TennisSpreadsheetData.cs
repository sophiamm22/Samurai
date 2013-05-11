using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data;
using System.Data.OleDb;

using Model = Samurai.Domain.Model;
using Samurai.Domain.Infrastructure;
using Samurai.Domain.Entities;
using Samurai.Domain.Repository;
using Samurai.Domain.APIModel;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Core;
using Samurai.Domain.Entities.ComplexTypes;

namespace Samurai.Domain.Value.Excel
{

  public interface ITennisSpreadsheetData
  {
    DateTime CouponDate { get; set; }
    void ReadData();
    IEnumerable<GenericMatchDetailQuery> UpdateResults(DateTime fixtureDate);
    IEnumerable<Model.IGenericTournamentCoupon> GetTournaments(Model.OddsDownloadStage stage = Model.OddsDownloadStage.Tournament);
    IEnumerable<Model.GenericMatchCoupon> GetMatches(Uri tournamentURL);
    IEnumerable<Model.GenericMatchCoupon> GetMatches();
    IDictionary<Model.Outcome, IEnumerable<Model.GenericOdd>> GetOdds(Model.GenericMatchCoupon matchCoupon, DateTime timeStamp);
    IEnumerable<Model.GenericPrediction> GetPredictions(Model.IValueOptions valueOptions);

  }

  public class TennisSpreadsheetData : ITennisSpreadsheetData
  {
    private readonly IBookmakerRepository bookmakerRepository;
    private readonly IFixtureRepository fixtureRepository;
    private readonly IPredictionRepository predictionRepository;
    private readonly IWebRepository webRepository;
    private readonly IStoredProceduresRepository storedProcRepository;

    private EnumerableRowCollection<DataRow> excelMatches;

    private Dictionary<string, Model.GenericPrediction> predictions;
    public DateTime CouponDate { get; set; }

    public TennisSpreadsheetData(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IPredictionRepository predictionRepository,
      IWebRepository webRepository, IStoredProceduresRepository storedProcRepository)
    {
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (predictionRepository == null) throw new ArgumentNullException("predictionRepository");
      if (webRepository == null) throw new ArgumentNullException("webRepository");
      if (storedProcRepository == null) throw new ArgumentNullException("storedProcRepository");

      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.predictionRepository = predictionRepository;
      this.webRepository = webRepository;
      this.storedProcRepository = storedProcRepository;

      this.predictions = new Dictionary<string, Model.GenericPrediction>();
    }

    public void ReadData()
    {
      var file = @"C:\Users\u0158158\Documents\VS\Tennis\2012TennisOdds.xlsx";
      var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"{0}\"; Extended Properties=\"Excel 12.0; IMEX=1; HDR=YES\";", file);
      var adapter = new OleDbDataAdapter("SELECT * FROM [TennisOdds$]", connectionString);
      using (var ds = new DataSet())
      {
        adapter.Fill(ds, "TennisOdds");
        this.excelMatches = ds.Tables["TennisOdds"].AsEnumerable();
      }

      foreach (var match in this.excelMatches)
      {
        var predictionURL = new Uri(match.Field<string>("URL").Replace(".", "").Replace("’", "").Replace("'", "").Replace("&", "").Replace(",", "").RemoveDiacritics());
        var jsonTennisPrediction = (APITennisPrediction)this.webRepository.ParseJson<APITennisPrediction>(
          predictionURL, s => ProgressReporterProvider.Current.ReportProgress(s, Model.ReporterImportance.Low, Model.ReporterAudience.Admin));

        var genericPrediction = TennisPredictionStrategy.ConvertAPIToGeneric(jsonTennisPrediction, predictionURL);
        genericPrediction.MatchDate = match.Field<DateTime>("DateToTake").Date;
        if (!this.predictions.ContainsKey(match.Field<string>("URL")))
          this.predictions.Add(match.Field<string>("URL"), genericPrediction);
      }
    }

    public IEnumerable<GenericMatchDetailQuery> UpdateResults(DateTime fixtureDate)
    {
      this.excelMatches.Where(x =>
                            x.Field<DateTime>("DateToTake").Date == fixtureDate.Date)
                              .ToList()
                              .ForEach(x =>
                              {
                                var player1 = this.fixtureRepository.GetTeamOrPlayerFromNameAndMaybeFirstName(x.Field<string>("Player1Surname"), x.Field<string>("Player1FirstName"));
                                var player2 = this.fixtureRepository.GetTeamOrPlayerFromNameAndMaybeFirstName(x.Field<string>("Player2Surname"), x.Field<string>("Player2FirstName"));
                                var matchDate = x.Field<DateTime>("DateToTake");
                                var persistedMatch = this.fixtureRepository.GetTennisMatch(player1.Slug, player2.Slug, matchDate);
                                if (persistedMatch == null)
                                {
                                  var tournamentEvent = this.fixtureRepository.GetTournamentEventFromTournamentAndDate(matchDate, x.Field<string>("WebName"));
                                  var newMatch = new Match()
                                  {
                                    TournamentEvent = tournamentEvent,
                                    MatchDate = matchDate,
                                    TeamsPlayerA = player1,
                                    TeamsPlayerB = player2,
                                    EligibleForBetting = true,
                                  };

                                  int[] scores = new int[] 
                                  { 
                                    (int)x.Field<double>("FirstSet"), 
                                    (int)x.Field<double>("SecondSet"),
                                    (int)x.Field<double>("ThirdSet"),
                                    (int)x.Field<double>("FourthSet"),
                                    (int)x.Field<double>("FifthSet")
                                  };

                                  newMatch.ObservedOutcomes.Add(new ObservedOutcome()
                                  {
                                    Match = newMatch,
                                    ScoreOutcome = this.fixtureRepository.GetScoreOutcome(scores.Count(s => s == 1), scores.Count(s => s == -1)) //TODO <- this is bullshit, retirie's will get a null returned
                                  });
                                  this.fixtureRepository.AddMatch(newMatch);
                                }
                              });

      this.fixtureRepository.SaveChanges();

      return this.storedProcRepository
                 .GetGenericMatchDetails(fixtureDate, "Tennis");
    }

    public IEnumerable<Model.IGenericTournamentCoupon> GetTournaments(Model.OddsDownloadStage stage = Model.OddsDownloadStage.Tournament)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<Model.GenericMatchCoupon> GetMatches(Uri tournamentURL)
    {
      return GetMatches();
    }

    public IEnumerable<Model.GenericMatchCoupon> GetMatches()
    {
      var matches = new List<Model.GenericMatchCoupon>();
      var returnMatches = new List<Match>();

      this.excelMatches.Where(x => x.Field<DateTime>("DateToTake").Date == CouponDate.Date)
                       .ToList()
                       .ForEach(x =>
                         {
                           matches.Add(new Model.GenericMatchCoupon()
                           {
                             MatchDate = x.Field<DateTime>("DateToTake").Date,
                             TeamOrPlayerA = string.Format("{0}-{1}", x.Field<string>("Player1FirstName"), x.Field<string>("Player1Surname")).ToHyphenated().RemoveDiacritics().ToLower(),
                             TeamOrPlayerB = string.Format("{0}-{1}", x.Field<string>("Player2FirstName"), x.Field<string>("Player2Surname")).ToHyphenated().RemoveDiacritics().ToLower(),
                             LastChecked = CouponDate.Date,
                             Source = "Tennis Data Odds",
                             MatchURL = new Uri(x.Field<string>("URL")),
                             HeadlineOdds = new Dictionary<Model.Outcome, double>()
                             {
                               { Model.Outcome.HomeWin, x.Field<double?>("TDB1") ?? 1.01 },
                               { Model.Outcome.AwayWin, x.Field<double?>("TDB2") ?? 1.01 }
                             }
                           });
                         });
      return matches;
    }

    public IDictionary<Model.Outcome, IEnumerable<Model.GenericOdd>> GetOdds(Model.GenericMatchCoupon matchCoupon, DateTime timeStamp)
    {
      var returnOdds = new Dictionary<Model.Outcome, IEnumerable<Model.GenericOdd>>();

      var outcomes = Enum.GetNames(typeof(Model.Outcome)).Where(o => o != "NotAssigned" && o != "Draw");
      string[] bookies = new string[] { "B365", "EX", "LB", "PS", "SJ" };
      var tennisData = this.bookmakerRepository.GetExternalSource("Tennis Data Odds");
      var valueSamurai = this.bookmakerRepository.GetExternalSource("Value Samurai");
      var bookiesDic = bookies.ToDictionary(b => b, b => this.bookmakerRepository.FindByName(this.bookmakerRepository.GetAlias(b, tennisData, valueSamurai)));

      var tennisDataBest = this.bookmakerRepository.FindByName("Tennis Data Odds Best Available");

      var oddsRow = this.excelMatches.FirstOrDefault(x => x.Field<string>("URL") == matchCoupon.MatchURL.ToString());

      foreach (var outcome in outcomes)
      {
        var outcomeEnum = (Model.Outcome)Enum.Parse(typeof(Model.Outcome), outcome);
        var oddsForOutcome = new List<Model.GenericOdd>();

        returnOdds.Add(outcomeEnum, oddsForOutcome);

        foreach (var bookieKey in bookiesDic.Keys)
        {
          var bookie = bookiesDic[bookieKey];
          var lookup = bookieKey + (outcome == "HomeWin" ? "1" : "2");

          var odd = oddsRow.Field<double?>(lookup);

          if (odd != null)
          {
            var genericOdd = CreateConcreateOdd(bookie, odd ?? 0.0);
            oddsForOutcome.Add(genericOdd);
          }
        }
      }
      return returnOdds;
    }

    public IEnumerable<Model.GenericPrediction> GetPredictions(Model.IValueOptions valueOptions)
    {
      var predictionsReturn = new List<Model.GenericPrediction>();
      var todaysRows = this.excelMatches.Where(x => x.Field<DateTime>("DateToTake").Date == valueOptions.CouponDate.Date);
      foreach (var row in todaysRows)
      {
        predictionsReturn.Add(this.predictions[row.Field<string>("URL")]);
      }
      return predictionsReturn;
    }

    private Model.ConcreateOdd CreateConcreateOdd(Bookmaker bookmaker, double odd)
    {
      var genericOdd = new Model.ConcreateOdd()
      {
        OddsBeforeCommission = odd,
        CommissionPct = (double)(bookmaker.CurrentCommission ?? 0.0m),
        DecimalOdds = odd * (1 - (double)(bookmaker.CurrentCommission ?? 0.0m)),
        BookmakerName = bookmaker.BookmakerName,
        Source = "Tennis Data Odds",
        TimeStamp = CouponDate,
        Priority = bookmaker.Priority
      };
      return genericOdd;
    }
  }
}