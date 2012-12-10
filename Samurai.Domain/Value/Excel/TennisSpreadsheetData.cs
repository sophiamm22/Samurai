﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data;
using System.Data.OleDb;

using Model = Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Domain.Repository;
using Samurai.Domain.APIModel;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Core;

namespace Samurai.Domain.Value.Excel
{
  public class TennisSpreadsheetData : ISpreadsheetData
  {
    private readonly IBookmakerRepository bookmakerRepository;
    private readonly IFixtureRepository fixtureRepository;
    private readonly IPredictionRepository predictionRepository;
    private readonly IWebRepository webRepository;

    private EnumerableRowCollection<DataRow> excelMatches;

    private Dictionary<string, Model.GenericPrediction> predictions;
    public DateTime CouponDate { get; set; }

    public TennisSpreadsheetData(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IPredictionRepository predictionRepository,
      IWebRepository webRepository)
    {
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (predictionRepository == null) throw new ArgumentNullException("predictionRepository");
      if (webRepository == null) throw new ArgumentNullException("webRepository");

      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.predictionRepository = predictionRepository;
      this.webRepository = webRepository;
      this.predictions = new Dictionary<string, Model.GenericPrediction>();
    }

    public void ReadData()
    {
      var file = @"C:\Users\u0158158\Documents\VS\Tennis\2012TennisOdds.xlsx";
      var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"{0}\"; Extended Properties=\"Excel 12.0; IMEX=1; HDR=NO\";", file);
      var adapter = new OleDbDataAdapter("SELECT * FROM [TennisOdds$]", connectionString);
      using (var ds = new DataSet())
      {
        adapter.Fill(ds, "TennisOdds");
        this.excelMatches = ds.Tables["TennisOdds"].AsEnumerable();
      }

      foreach (var match in this.excelMatches)
      {
        var predictionURL = new Uri(match.Field<string>("URL").Replace(".", "").RemoveDiacritics());
        var jsonTennisPrediction = (APITennisPrediction)this.webRepository.ParseJson<APITennisPrediction>(
          predictionURL, s => Console.WriteLine(s));

        var genericPrediction = TennisPredictionStrategy.ConvertAPIToGeneric(jsonTennisPrediction, predictionURL);
        this.predictions.Add(match.Field<string>("URL"), genericPrediction);
      }
    }

    public IEnumerable<Match> UpdateResults(DateTime fixtureDate)
    {
      var returnMatches = new List<Match>();
      this.excelMatches.Where(x =>
                            x.Field<DateTime>("Date").Date == fixtureDate.Date)
                              .ToList()
                              .ForEach(x =>
                              {
                                var player1 = this.fixtureRepository.GetTeamOrPlayerFromNameAndMaybeFirstName(x.Field<string>("Player1FirstName"), x.Field<string>("Player1Surname"));
                                var player2 = this.fixtureRepository.GetTeamOrPlayerFromNameAndMaybeFirstName(x.Field<string>("Player2FirstName"), x.Field<string>("Player2Surname"));
                                var matchDate = x.Field<DateTime>("Date");
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
                                    ScoreOutcome = this.fixtureRepository.GetScoreOutcome(scores.Count(s => s == 1), scores.Count(s => s == -1))
                                  });
                                  returnMatches.Add(newMatch);
                                  this.fixtureRepository.AddMatch(newMatch);
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
      throw new NotImplementedException();
    }

    public IEnumerable<Model.IGenericMatchCoupon> GetMatches(Uri tournamentURL)
    {
      return GetMatches();
    }

    public IEnumerable<Model.IGenericMatchCoupon> GetMatches()
    {
      var matches = new List<Model.IGenericMatchCoupon>();
      var returnMatches = new List<Match>();

      this.excelMatches.Where(x => x.Field<DateTime>("DateToTake").Date == CouponDate.Date)
                       .ToList()
                       .ForEach(x =>
                         {
                           matches.Add(new Model.GenericMatchCoupon()
                           {
                             MatchDate = x.Field<DateTime>("Date").Date,
                             TeamOrPlayerA = string.Format("{0}-{1}", x.Field<string>("Player1FirstName"), x.Field<string>("Player1Surname")).ToHyphenated().RemoveDiacritics().ToLower(),
                             TeamOrPlayerB = string.Format("{0}-{1}", x.Field<string>("Player2FirstName"), x.Field<string>("Player2Surname")).ToHyphenated().RemoveDiacritics().ToLower(),
                             LastChecked = CouponDate.Date,
                             Source = "Tennis Data Odds",
                             MatchURL = new Uri(x.Field<string>("URL")),
                             HeadlineOdds = new Dictionary<Model.Outcome, double>()
                             {
                               { Model.Outcome.HomeWin, x.Field<double>("Player1TennisDataBestAvailable") },
                               { Model.Outcome.AwayWin, x.Field<double>("Player2TennisDataBestAvailable") }
                             }
                           });
                         });
      return matches;
    }

    public IDictionary<Model.Outcome, IEnumerable<Model.GenericOdd>> GetOdds(Model.IGenericMatchCoupon matchCoupon, DateTime timeStamp)
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

          var odd = oddsRow.Field<double>(lookup);

          var genericOdd = CreateConcreateOdd(bookie, odd);
          oddsForOutcome.Add(genericOdd);
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