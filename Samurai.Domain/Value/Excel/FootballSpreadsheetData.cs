using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data;
using System.Data.OleDb;

using Model = Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.Value.Excel
{
  public interface ISpreadsheetData
  {
    DateTime CouponDate { get; set; }
    void ReadData();
    IEnumerable<Match> UpdateResults(DateTime fixtureDate);
    IEnumerable<Model.IGenericTournamentCoupon> GetTournaments(Model.OddsDownloadStage stage = Model.OddsDownloadStage.Tournament);
    IEnumerable<Model.GenericMatchCoupon> GetMatches(Uri tournamentURL);
    IEnumerable<Model.GenericMatchCoupon> GetMatches();
    IDictionary<Model.Outcome, IEnumerable<Model.GenericOdd>> GetOdds(Model.GenericMatchCoupon matchCoupon, DateTime timeStamp);
    IEnumerable<Model.GenericPrediction> GetPredictions(Model.IValueOptions valueOptions);

  }

  public class FootballSpreadsheetData : ISpreadsheetData
  {
    private readonly IBookmakerRepository bookmakerRepository;
    private readonly IFixtureRepository fixtureRepository;
    private readonly IPredictionRepository predictionRepository;

    private List<Model.GenericPrediction> genericPredictions;

    public DateTime CouponDate { get; set; }
    public IDictionary<int, EnumerableRowCollection<DataRow>> Predictions { get; set; }
    public EnumerableRowCollection<DataRow> FixturesCouponsOdds { get; set; }

   public FootballSpreadsheetData(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IPredictionRepository predictionRepository)
    {
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (predictionRepository == null) throw new ArgumentNullException("predictionRepository");

      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.predictionRepository = predictionRepository;
    }

    public void ReadData()
    {
      ReadFixtureData();
      ReadPredictionData();
    }

    private void ReadPredictionData()
    {
      genericPredictions = new List<Model.GenericPrediction>();

      var files = Directory.GetFiles(@"C:\Users\u0158158\Documents\Visual Studio 2010\Projects\ValueSamurai\ValueSamurai.IKTS\bin\Debug")
                           .Where(f => f.IndexOf("IKTS 2011") > 0)
                           .ToList();
      int i = 1;
      foreach (var file in files)
      {
        var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"{0}\"; Extended Properties=\"Excel 12.0; IMEX=1; HDR=NO\";", file);
        var adapter = new OleDbDataAdapter("SELECT * FROM [DataDump$]", connectionString);
        using (var ds = new DataSet())
        {
          adapter.Fill(ds, "DataDump");
          var rowCollection = ds.Tables["DataDump"].AsEnumerable();
          Predictions.Add(i, rowCollection);
        }
        i++;
      }

      var teams = new Dictionary<string, TeamPlayer>();
      var matches = new Dictionary<string, Model.FootballPrediction>();

      foreach (var predictionSet in Predictions.Values)
      {
        foreach (var row in predictionSet)
        {
          if (row.Field<string>(2) == null) continue;
          TeamPlayer teamA = null;
          TeamPlayer teamB = null;
          Model.FootballPrediction prediction = null;

          var teamAName = row.Field<string>(2);
          var teamBName = row.Field<string>(3);
          var predictionName = string.Format("{0}|{1}", teamAName, teamBName);

          if (teams.ContainsKey(teamAName))
            teamA = teams[teamAName];
          else
          {
            teamA = this.fixtureRepository.GetTeamOrPlayerFromName(teamAName);
            teams.Add(teamAName, teamA);
          }

          if (teams.ContainsKey(teamBName))
            teamB = teams[teamBName];
          else
          {
            teamB = this.fixtureRepository.GetTeamOrPlayerFromName(teamBName);
            teams.Add(teamBName, teamB);
          }

          if (matches.ContainsKey(predictionName))
            prediction = matches[predictionName];
          else
          {
            prediction = new Model.FootballPrediction()
            {
              TournamentName = "Premier League",
              TeamOrPlayerA = teamAName,
              TeamOrPlayerB = teamBName,
              MatchDate = this.FixturesCouponsOdds.First(x => x.Field<string>("HomeTeam") == teamAName && x.Field<string>("AwayTeam") == teamBName).Field<DateTime>("Date"),

            };
            prediction.OutcomeProbabilities.Add(Model.Outcome.HomeWin, 0);
            prediction.OutcomeProbabilities.Add(Model.Outcome.Draw, 0);
            prediction.OutcomeProbabilities.Add(Model.Outcome.AwayWin, 0);
            matches.Add(predictionName, prediction);
          }

          int scoreA = (int)row.Field<double>(4);
          int scoreB = (int)row.Field<double>(5);
          if (!prediction.ScoreLineProbabilities.ContainsKey(string.Format("{0}-{1}", scoreA.ToString(), scoreB.ToString())))
          {
            prediction.ScoreLineProbabilities.Add(string.Format("{0}-{1}", scoreA.ToString(), scoreB.ToString()), row.Field<double>(6));
            if (scoreA == scoreB)
              prediction.OutcomeProbabilities[Model.Outcome.Draw] += row.Field<double>(6);
            else if (scoreA > scoreB)
              prediction.OutcomeProbabilities[Model.Outcome.HomeWin] += row.Field<double>(6);
            else if (scoreA < scoreB)
              prediction.OutcomeProbabilities[Model.Outcome.AwayWin] += row.Field<double>(6);
          }
        }
      }
      genericPredictions.AddRange(matches.Values);
    }

    private void ReadFixtureData()
    {
      var fileName = string.Format(@"C:\Users\u0158158\Documents\VS\Football\2011 12 season.xlsx");
      var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"{0}\"; Extended Properties=\"Excel 12.0; HDR=YES\"", fileName);
      var adapter = new OleDbDataAdapter("SELECT * FROM [Fixtures$]", connectionString);
      using (var ds = new DataSet())
      {
        adapter.Fill(ds, "Fixtures");
        FixturesCouponsOdds = ds.Tables["Fixtures"].AsEnumerable();
      }
    }

    public IEnumerable<Match> UpdateResults(DateTime fixtureDate)
    {
      var returnMatches = new List<Match>();
      FixturesCouponsOdds.Where(x => 
                                x.Field<DateTime>("Date").Date == fixtureDate.Date)
                                 .ToList()
                                 .ForEach(x =>
                                 {
                                   var homeTeam = this.fixtureRepository.GetTeamOrPlayerFromName(x.Field<string>("HomeTeam"));
                                   var awayTeam = this.fixtureRepository.GetTeamOrPlayerFromName(x.Field<string>("AwayTeam"));
                                   var matchDate = x.Field<DateTime>("Date");
                                   var persistedMatch = this.fixtureRepository.GetMatchFromTeamSelections(homeTeam, awayTeam, matchDate);
                                   if (persistedMatch == null)
                                   {
                                     var tournamentEvent = this.fixtureRepository.GetFootballTournamentEvent(1, fixtureDate);
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
                                       ScoreOutcome = this.fixtureRepository.GetScoreOutcome((int)x.Field<double>("FTHG"), (int)x.Field<double>("FTAG"))
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
      var tournaments = new List<Model.IGenericTournamentCoupon>();
      tournaments.Add(new Model.GenericTournamentCoupon()
      {
        TournamentName = "Premier League",
        Matches = GetMatches()
      });
      return tournaments;
    }

    public IEnumerable<Model.GenericMatchCoupon> GetMatches(Uri tournamentURL)
    {
      return GetMatches();
    }

    public IEnumerable<Model.GenericMatchCoupon> GetMatches()
    {
      var matches = new List<Model.GenericMatchCoupon>();
      var returnMatches = new List<Match>();

      FixturesCouponsOdds.Where(x => x.Field<DateTime>("Date").Date == CouponDate.Date)
                                 .ToList()
                                 .ForEach(x =>
                                 {
                                   matches.Add(new Model.GenericMatchCoupon()
                                   {
                                     MatchDate = x.Field<DateTime>("Date").Date,
                                     TeamOrPlayerA = x.Field<string>("HomeTeam"),
                                     TeamOrPlayerB = x.Field<string>("AwayTeam"),
                                     LastChecked = CouponDate.Date,
                                     Source = "Football Data Odds",
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

    public IDictionary<Model.Outcome, IEnumerable<Model.GenericOdd>> GetOdds(Model.GenericMatchCoupon matchCoupon, DateTime timeStamp)
    {
      var returnOdds = new Dictionary<Model.Outcome, IEnumerable<Model.GenericOdd>>();

      var outcomes = Enum.GetNames(typeof(Model.Outcome)).Where(o => o != "NotAssigned");
      string[] bookies = new string[] { "B365", "BS", "LB", "SB", "SJ", "WH", "BW", "GB", "IW", "VC" };
      var footballData = this.bookmakerRepository.GetExternalSource("Football Data Odds");
      var valueSamurai = this.bookmakerRepository.GetExternalSource("Value Samurai");
      var bookiesDic = bookies.ToDictionary(b => b, b => this.bookmakerRepository.FindByName(this.bookmakerRepository.GetAlias(b, footballData, valueSamurai)));

      var footballDataBest = this.bookmakerRepository.FindByName("Football Data Odds Best Available");

      var oddsRow = FixturesCouponsOdds.Where(x => x.Field<string>("HomeTeam") == matchCoupon.TeamOrPlayerA &&
                                                   x.Field<string>("AwayTeam") == matchCoupon.TeamOrPlayerB)
                                       .FirstOrDefault();

      foreach (var outcome in outcomes)
      {
        var outcomeEnum = (Model.Outcome)Enum.Parse(typeof(Model.Outcome), outcome);
        var oddsForOutcome = new List<Model.GenericOdd>();

        returnOdds.Add(outcomeEnum, oddsForOutcome);

        //already taken care of?
        //var bestOdd = oddsRow.Field<double>("BbMx" + outcome.Substring(0, 1));
        //oddsForOutcome.Add(CreateConcreateOdd(footballDataBest, bestOdd));

        foreach (var bookieKey in bookiesDic.Keys)
        {
          var bookie = bookiesDic[bookieKey];
          var lookup = bookieKey + outcome.Substring(0, 1);

          var odd = oddsRow.Field<double>(lookup);

          var genericOdd = CreateConcreateOdd(bookie, odd);
          oddsForOutcome.Add(genericOdd);
        }
      }
      return returnOdds;
    }

    public IEnumerable<Model.GenericPrediction> GetPredictions(Model.IValueOptions valueOptions)
    {
      return genericPredictions.Where(p => p.MatchDate.Date == valueOptions.CouponDate.Date);
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
        TimeStamp = CouponDate,
        Priority = bookmaker.Priority
      };
      return genericOdd;
    }

  }
}

