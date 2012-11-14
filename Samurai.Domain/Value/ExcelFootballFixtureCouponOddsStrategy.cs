using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.OleDb;

using Model = Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.Value
{
  public class ExcelFootballFixtureCouponOddsStrategy : IFixturesAndOdds
  {
    private readonly IBookmakerRepository bookmakerRepository;
    private readonly IFixtureRepository fixtureRepository;
    private readonly IPredictionRepository predictionRepository;
    private readonly Model.IValueOptions valueOptions;

    private string fileName;
    private EnumerableRowCollection<DataRow> excelData;

    public ExcelFootballFixtureCouponOddsStrategy(IBookmakerRepository bookmakerRepository, 
      IFixtureRepository fixtureRepository, IPredictionRepository predictionRepository,
      Model.IValueOptions valueOptions)
    {
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (predictionRepository == null) throw new ArgumentNullException("predictionRepository");
      if (valueOptions == null) throw new ArgumentNullException("valueOptions");

      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.predictionRepository = predictionRepository;
      this.valueOptions = valueOptions;
    }

    public void ReadExcelFile()
    {
      this.fileName = string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), this.valueOptions.Tournament.TournamentName);
      var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0; HDR=YES", this.fileName);
      var adapter = new OleDbDataAdapter("SELECT * FROM [Fixtures$]", connectionString);
      using (var ds = new DataSet())
      {
        adapter.Fill(ds, "Fixtures");
        this.excelData = ds.Tables["Fixtures"].AsEnumerable();
      }
    }

    public IEnumerable<Match> UpdateFixtures(DateTime fixtureDate)
    {
      return UpdateResults(fixtureDate);
    }

    public IEnumerable<Match> UpdateResults(DateTime fixtureDate, string reusedHTML = "")
    {
      var returnMatches = new List<Match>();
      this.excelData.Where(x => x.Field<DateTime>("Date").Date == fixtureDate.Date)
                                 .ToList()
                                 .ForEach(x =>
                                 {
                                   var homeTeam = this.fixtureRepository.GetTeamOrPlayerFromName(x.Field<string>("HomeTeam"));
                                   var awayTeam = this.fixtureRepository.GetTeamOrPlayerFromName(x.Field<string>("AwayTeam"));
                                   var matchDate = x.Field<DateTime>("Date");
                                   var persistedMatch = this.fixtureRepository.GetMatchFromTeamSelections(homeTeam, awayTeam, matchDate);
                                   if (persistedMatch == null)
                                   {
                                     var tournamentEvent = this.fixtureRepository.GetFootballTournamentEvent(this.valueOptions.Tournament.Id, fixtureDate);
                                     var newMatch = new Match()
                                     {
                                       TournamentEvent = tournamentEvent,
                                       MatchDate = matchDate,
                                       TeamsPlayerA = homeTeam,
                                       TeamsPlayerB = awayTeam,
                                       EligibleForBetting = true,
                                     };
                                     newMatch.ObservedOutcomes.Add(new ObservedOutcome()
                                     {
                                       Match = newMatch,
                                       ScoreOutcome = this.fixtureRepository.GetScoreOutcome(x.Field<int>("FTHG"), x.Field<int>("FTAG"))
                                     });
                                     returnMatches.Add(newMatch);
                                   }
                                   else //can't be bothered to do properly, this will always be run by me only
                                   {
                                     returnMatches.Add(persistedMatch);
                                   }
                                 });

      this.fixtureRepository.SaveChanges();
      return returnMatches;
    }

    public IEnumerable<Model.IGenericTournamentCoupon> GetTournaments(Model.OddsDownloadStage stage = Model.OddsDownloadStage.Tournament)
    {
      var tournaments = new List<Model.IGenericTournamentCoupon>();
      tournaments.Add(new Model.GenericTournamentCoupon()
      {
        TournamentName = this.valueOptions.Tournament.TournamentName,
        Matches = GetMatches()
      });
      return tournaments;
    }

    public IEnumerable<Model.IGenericMatchCoupon> GetMatches(Uri tournamentURL)
    {
      return GetMatches();
    }

    public IEnumerable<Model.IGenericMatchCoupon> GetMatches()
    {
      var matches = new List<Model.IGenericMatchCoupon>();

      var returnMatches = new List<Match>();
      this.excelData.Where(x => x.Field<DateTime>("Date").Date == this.valueOptions.CouponDate.Date)
                                 .ToList()
                                 .ForEach(x =>
                                 {
                                   matches.Add(new Model.GenericMatchCoupon()
                                   {
                                     MatchDate = x.Field<DateTime>("Date").Date,
                                     TeamOrPlayerA = x.Field<string>("HomeTeam"),
                                     TeamOrPlayerB = x.Field<string>("AwayTeam"),
                                     HeadlineOdds = new Dictionary<Model.Outcome, double>()
                                     {
                                       { Model.Outcome.HomeWin, x.Field<double>("BbMxH") },
                                       { Model.Outcome.Draw, x.Field<double>("BbMxD") },
                                       { Model.Outcome.AwayWin, x.Field<double>("BbMxA") }
                                     }
                                   });
                                 });
      return matches;
    }

    public IDictionary<Model.Outcome, IEnumerable<Model.GenericOdd>> GetOdds(Model.IGenericMatchCoupon matchCoupon, DateTime timeStamp)
    {
      var returnOdds = new Dictionary<Model.Outcome, IEnumerable<Model.GenericOdd>>();
      
      var outcomes = Enum.GetNames(typeof(Model.Outcome)).Where(o => o != "NotAssigned");
      string[] bookies = new string[] { "B365", "BS", "LB", "PS", "SB", "SJ", "WH", "BW", "GB", "IW", "SO", "SY", "VC" };
      var footballData = this.bookmakerRepository.GetExternalSource("Football Data Odds");
      var valueSamurai = this.bookmakerRepository.GetExternalSource("Value Samurai");
      var bookiesDic = bookies.ToDictionary(b => b, b => this.bookmakerRepository.FindByName(this.bookmakerRepository.GetAlias(b, footballData, valueSamurai)));

      var footballDataBest = this.bookmakerRepository.FindByName("Football Data Best Available");

      var oddsRow = this.excelData.Where(x => x.Field<string>("HomeTeam") == matchCoupon.TeamOrPlayerA &&
                                            x.Field<string>("AwayTeam") == matchCoupon.TeamOrPlayerB)
                                  .FirstOrDefault();

      foreach (var outcome in outcomes)
      {
        var outcomeEnum = (Model.Outcome)Enum.Parse(typeof(Model.Outcome), outcome);
        var oddsForOutcome = new List<Model.GenericOdd>();

        returnOdds.Add(outcomeEnum, oddsForOutcome);

        var bestOdd = oddsRow.Field<double>("BbMx" + outcome.Substring(0, 1));
        oddsForOutcome.Add(CreateConcreateOdd(footballDataBest, bestOdd));

        foreach (var bookieKey in bookiesDic.Keys)
        {
          var bookie = bookiesDic[bookieKey];
          var lookup = bookieKey + outcome.Substring(0, 1);

          var odd = oddsRow.Field<double>(lookup);

          var genericOdd = CreateConcreateOdd(bookie, odd);
        }
      }
      return returnOdds;
    }

    private Model.ConcreateOdd CreateConcreateOdd(Bookmaker bookmaker, double odd)
    {
      var genericOdd = new Model.ConcreateOdd()
      {
        OddsBeforeCommission = odd,
        CommissionPct = (double)(bookmaker.CurrentCommission ?? 0.0m),
        DecimalOdds = odd * (1 - (double)(bookmaker.CurrentCommission ?? 0.0m)),
        BookmakerName = bookmaker.BookmakerName,
        Source = "Football Data Odds",
        TimeStamp = this.valueOptions.CouponDate,
        Priority = bookmaker.Priority
      };
      return genericOdd;
    }

  }
}
