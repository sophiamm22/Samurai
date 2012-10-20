using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Collections.Generic;
using WebMatrix.WebData;
using System.Web.Security;
using System.Diagnostics;
using System.Text;

using Samurai.Domain.Entities;
using Samurai.SqlDataAccess;

namespace Samurai.SqlDataAccess.Migrations
{
  internal sealed class Configuration : DbMigrationsConfiguration<ValueSamuraiContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = true;
    }

    protected override void Seed(ValueSamuraiContext context)
    {

      WebSecurity.InitializeDatabaseConnection(
     "ValueSamuraiContext",
     "UserProfile",
     "UserProfileID_pk",
     "UserName",
     autoCreateTables: true);



      SeedUsersAndRoles();
      SeedSamurai(context);

    }

    private void SeedUsersAndRoles()
    {
      if (!Roles.RoleExists("Administrator"))
        Roles.CreateRole("Administrator");

      if (!WebSecurity.UserExists("martinstaniforth"))
        WebSecurity.CreateUserAndAccount(
          "martinstaniforth",
          "password"/*,
          new { EmailAddress = "martinstaniforth@gmail.com" }*/);

      if (!Roles.GetRolesForUser("martinstaniforth").Contains("Administrator"))
        Roles.AddUsersToRoles(new[] { "martinstaniforth" }, new[] { "Administrator" });
    }

    private void SeedSamurai(ValueSamuraiContext context)
    {
      //bookmakers
      var b10bet = new Bookmaker { BookmakerName = "10Bet", Slug = "10bet", IsExchange = false, BookmakerURL = "nothing" };
      var b32red_bet = new Bookmaker { BookmakerName = "32Red bet", Slug = "32red-bet", IsExchange = false, BookmakerURL = "nothing" };
      var b888_sport = new Bookmaker { BookmakerName = "888 Sport", Slug = "888-sport", IsExchange = false, BookmakerURL = "nothing" };
      var bbet_365 = new Bookmaker { BookmakerName = "Bet 365", Slug = "bet-365", IsExchange = false, BookmakerURL = "nothing" };
      var bbet_victor = new Bookmaker { BookmakerName = "Bet Victor", Slug = "bet-victor", IsExchange = false, BookmakerURL = "nothing" };
      var bbet770 = new Bookmaker { BookmakerName = "Bet770", Slug = "bet770", IsExchange = false, BookmakerURL = "nothing" };
      var bbetdaq = new Bookmaker { BookmakerName = "BETDAQ", Slug = "betdaq", IsExchange = false, BookmakerURL = "nothing" };
      var bbetfair = new Bookmaker { BookmakerName = "Betfair", Slug = "betfair", IsExchange = false, BookmakerURL = "nothing" };
      var bbetfred = new Bookmaker { BookmakerName = "Betfred", Slug = "betfred", IsExchange = false, BookmakerURL = "nothing" };
      var bbetinternet = new Bookmaker { BookmakerName = "Betinternet", Slug = "betinternet", IsExchange = false, BookmakerURL = "nothing" };
      var bbetvictor = new Bookmaker { BookmakerName = "BetVictor", Slug = "betvictor", IsExchange = false, BookmakerURL = "nothing" };
      var bblue_square = new Bookmaker { BookmakerName = "Blue Square", Slug = "blue-square", IsExchange = false, BookmakerURL = "nothing" };
      var bbodog = new Bookmaker { BookmakerName = "Bodog", Slug = "bodog", IsExchange = false, BookmakerURL = "nothing" };
      var bboylesports = new Bookmaker { BookmakerName = "Boylesports", Slug = "boylesports", IsExchange = false, BookmakerURL = "nothing" };
      var bbwin = new Bookmaker { BookmakerName = "Bwin", Slug = "bwin", IsExchange = false, BookmakerURL = "nothing" };
      var bcoral = new Bookmaker { BookmakerName = "Coral", Slug = "coral", IsExchange = false, BookmakerURL = "nothing" };
      var bcorbetts = new Bookmaker { BookmakerName = "Corbetts", Slug = "corbetts", IsExchange = false, BookmakerURL = "nothing" };
      var bladbrokes = new Bookmaker { BookmakerName = "Ladbrokes", Slug = "ladbrokes", IsExchange = false, BookmakerURL = "nothing" };
      var bmatchbook_com = new Bookmaker { BookmakerName = "Matchbook.com", Slug = "matchbook-com", IsExchange = false, BookmakerURL = "nothing" };
      var bpaddy_power = new Bookmaker { BookmakerName = "Paddy Power", Slug = "paddy-power", IsExchange = false, BookmakerURL = "nothing" };
      var bpanbet = new Bookmaker { BookmakerName = "Panbet", Slug = "panbet", IsExchange = false, BookmakerURL = "nothing" };
      var bpinnacle_sports = new Bookmaker { BookmakerName = "Pinnacle Sports", Slug = "pinnacle-sports", IsExchange = false, BookmakerURL = "nothing" };
      var bsky_bet = new Bookmaker { BookmakerName = "Sky Bet", Slug = "sky-bet", IsExchange = false, BookmakerURL = "nothing" };
      var bsmarkets = new Bookmaker { BookmakerName = "Smarkets", Slug = "smarkets", IsExchange = false, BookmakerURL = "nothing" };
      var bsporting_bet = new Bookmaker { BookmakerName = "Sporting Bet", Slug = "sporting-bet", IsExchange = false, BookmakerURL = "nothing" };
      var bspreadex = new Bookmaker { BookmakerName = "Spreadex", Slug = "spreadex", IsExchange = false, BookmakerURL = "nothing" };
      var bstan_james = new Bookmaker { BookmakerName = "Stan James", Slug = "stan-james", IsExchange = false, BookmakerURL = "nothing" };
      var btotesport = new Bookmaker { BookmakerName = "Totesport", Slug = "totesport", IsExchange = false, BookmakerURL = "nothing" };
      var bwbx = new Bookmaker { BookmakerName = "WBX", Slug = "wbx", IsExchange = false, BookmakerURL = "nothing" };
      var bwilliam_hill = new Bookmaker { BookmakerName = "William Hill", Slug = "william-hill", IsExchange = false, BookmakerURL = "nothing" };
      var byouwin = new Bookmaker { BookmakerName = "youwin", Slug = "youwin", IsExchange = false, BookmakerURL = "nothing" };

      context.Set<Bookmaker>().AddOrUpdate(b10bet, b32red_bet, b888_sport, bbet_365, bbet_victor, bbet770, bbetdaq, bbetfair, bbetfred, bbetinternet, bbetvictor, bblue_square, bbodog, bboylesports, bbwin, bcoral, bcorbetts, bladbrokes, bmatchbook_com, bpaddy_power, bpanbet, bpinnacle_sports, bsky_bet, bsmarkets, bsporting_bet, bspreadex, bstan_james, btotesport, bwbx, bwilliam_hill, byouwin);

      //sports
      var football = new Sport { SportName = "Football" };
      var tennis = new Sport { SportName = "Tennis" };

      context.Set<Sport>().AddOrUpdate(football, tennis);

      //competitions
      var premierLeague = new Competition { Sport = football, CompetitionName = "Premier League", OverroundRequired = 0.1M };
      var championship = new Competition { Sport = football, CompetitionName = "Championship", OverroundRequired = 0.1M };
      var leagueOne = new Competition { Sport = football, CompetitionName = "League One", OverroundRequired = 0.1M };
      var leagueTwo = new Competition { Sport = football, CompetitionName = "League Two", OverroundRequired = 0.1M };
      var atp = new Competition { Sport = tennis, CompetitionName = "ATP", OverroundRequired = 0.1M, GamesRequiredForBet = 50 };

      context.Set<Competition>().AddOrUpdate(premierLeague, championship, leagueOne, leagueTwo, atp);

      //funds
      var premierFund = new Fund { FundName = "Premier", Bank = 500M, Competitions = new List<Competition>() { premierLeague }, KellyMultiplier = 0.25M };
      var footballLeagueFund = new Fund { FundName = "Football League", Bank = 500M, Competitions = new List<Competition>() { championship, leagueOne, leagueTwo }, KellyMultiplier = 0.25M };
      var tennisFund = new Fund { FundName = "ATP", Bank = 500M, Competitions = new List<Competition>() { atp }, KellyMultiplier = 0.25M };

      context.Set<Fund>().AddOrUpdate(premierFund, footballLeagueFund, tennisFund);

      //external sources
      var skySports = new ExternalSource { Source = "Sky Sports", OddsSource = false, TheoreticalOddsSource = false };
      var bestBetting = new ExternalSource { Source = "Best Bestting", OddsSource = true, TheoreticalOddsSource = false };
      var oddsCheckerMobi = new ExternalSource { Source = "Odds Checker Mobi", OddsSource = true, TheoreticalOddsSource = false };
      var oddsCheckerWeb = new ExternalSource { Source = "Odds Checker Web", OddsSource = true, TheoreticalOddsSource = false };
      var tennisDataOdds = new ExternalSource { Source = "Tennis Data Odds", OddsSource = true, TheoreticalOddsSource = true };
      var footballDataOdds = new ExternalSource { Source = "Football Data Odds", OddsSource = true, TheoreticalOddsSource = true };
      var tb365 = new ExternalSource { Source = "Tennis Betting 365", OddsSource = false, TheoreticalOddsSource = false };
      var finkTank = new ExternalSource { Source = "Fink Tank (dectech)", OddsSource = false, TheoreticalOddsSource = false };

      context.Set<ExternalSource>().AddOrUpdate(skySports, bestBetting, oddsCheckerMobi, oddsCheckerWeb, tennisDataOdds, footballDataOdds, tb365, finkTank);

      //match outcomes
      var teamOrPlayerAWin = new MatchOutcome { MatchOutcomeString = "Team or player A win" };
      var draw = new MatchOutcome { MatchOutcomeString = "Draw" };
      var teamOrPlayerBWin = new MatchOutcome { MatchOutcomeString = "Team or player A win" };

      context.Set<MatchOutcome>().AddOrUpdate(teamOrPlayerAWin, draw, teamOrPlayerBWin);

      //scoreOutcomes
      (
        from scoreA in Enumerable.Range(0, 21)
        from scoreB in Enumerable.Range(0, 21)
        let outcome = (scoreA == scoreB ? draw : (scoreA > scoreB ? teamOrPlayerAWin : teamOrPlayerBWin))
        select new ScoreOutcome
        {
          TeamAScore = scoreA,
          TeamBScore = scoreB,
          MatchOutcome = outcome
        }
      ).ToList()
      .ForEach(s => context.Set<ScoreOutcome>().AddOrUpdate(s));


      //teams
      var arsenal = new TeamsPlayer { TeamName = "Arsenal", Slug = "arsenal", FinkTankID = 0 };
      var astonvilla = new TeamsPlayer { TeamName = "Aston Villa", Slug = "aston-villa", FinkTankID = 1 };
      var birmingham = new TeamsPlayer { TeamName = "Birmingham", Slug = "birmingham", FinkTankID = 2 };
      var blackburn = new TeamsPlayer { TeamName = "Blackburn", Slug = "blackburn", FinkTankID = 3 };
      var blackpool = new TeamsPlayer { TeamName = "Blackpool", Slug = "blackpool", FinkTankID = 45 };
      var bolton = new TeamsPlayer { TeamName = "Bolton", Slug = "bolton", FinkTankID = 4 };
      var chelsea = new TeamsPlayer { TeamName = "Chelsea", Slug = "chelsea", FinkTankID = 6 };
      var everton = new TeamsPlayer { TeamName = "Everton", Slug = "everton", FinkTankID = 7 };
      var fulham = new TeamsPlayer { TeamName = "Fulham", Slug = "fulham", FinkTankID = 8 };
      var liverpool = new TeamsPlayer { TeamName = "Liverpool", Slug = "liverpool", FinkTankID = 10 };
      var mancity = new TeamsPlayer { TeamName = "Man City", Slug = "man-city", FinkTankID = 11 };
      var manunited = new TeamsPlayer { TeamName = "Man United", Slug = "man-united", FinkTankID = 12 };
      var newcastle = new TeamsPlayer { TeamName = "Newcastle", Slug = "newcastle", FinkTankID = 14 };
      var stoke = new TeamsPlayer { TeamName = "Stoke", Slug = "stoke", FinkTankID = 39 };
      var sunderland = new TeamsPlayer { TeamName = "Sunderland", Slug = "sunderland", FinkTankID = 16 };
      var tottenham = new TeamsPlayer { TeamName = "Tottenham", Slug = "tottenham", FinkTankID = 17 };
      var westbrom = new TeamsPlayer { TeamName = "West Brom", Slug = "west-brom", FinkTankID = 18 };
      var westham = new TeamsPlayer { TeamName = "West Ham", Slug = "west-ham", FinkTankID = 19 };
      var wigan = new TeamsPlayer { TeamName = "Wigan", Slug = "wigan", FinkTankID = 66 };
      var wolves = new TeamsPlayer { TeamName = "Wolves", Slug = "wolves", FinkTankID = 43 };
      var barnsley = new TeamsPlayer { TeamName = "Barnsley", Slug = "barnsley", FinkTankID = 44 };
      var bristolcity = new TeamsPlayer { TeamName = "Bristol City", Slug = "bristol-city", FinkTankID = 47 };
      var burnley = new TeamsPlayer { TeamName = "Burnley", Slug = "burnley", FinkTankID = 22 };
      var cardiff = new TeamsPlayer { TeamName = "Cardiff", Slug = "cardiff", FinkTankID = 48 };
      var coventry = new TeamsPlayer { TeamName = "Coventry", Slug = "coventry", FinkTankID = 23 };
      var crystalpalace = new TeamsPlayer { TeamName = "Crystal Palace", Slug = "crystal-palace", FinkTankID = 24 };
      var derby = new TeamsPlayer { TeamName = "Derby", Slug = "derby", FinkTankID = 25 };
      var doncaster = new TeamsPlayer { TeamName = "Doncaster", Slug = "doncaster", FinkTankID = 95 };
      var hull = new TeamsPlayer { TeamName = "Hull", Slug = "hull", FinkTankID = 77 };
      var ipswich = new TeamsPlayer { TeamName = "Ipswich", Slug = "ipswich", FinkTankID = 28 };
      var leeds = new TeamsPlayer { TeamName = "Leeds", Slug = "leeds", FinkTankID = 9 };
      var leicester = new TeamsPlayer { TeamName = "Leicester", Slug = "leicester", FinkTankID = 29 };
      var middlesboro = new TeamsPlayer { TeamName = "Middlesboro", Slug = "middlesboro", FinkTankID = 13 };
      var millwall = new TeamsPlayer { TeamName = "Millwall", Slug = "millwall", FinkTankID = 30 };
      var norwich = new TeamsPlayer { TeamName = "Norwich", Slug = "norwich", FinkTankID = 31 };
      var nottmforest = new TeamsPlayer { TeamName = "Nott'm Forest", Slug = "nottm-forest", FinkTankID = 32 };
      var portsmouth = new TeamsPlayer { TeamName = "Portsmouth", Slug = "portsmouth", FinkTankID = 33 };
      var preston = new TeamsPlayer { TeamName = "Preston", Slug = "preston", FinkTankID = 34 };
      var qpr = new TeamsPlayer { TeamName = "QPR", Slug = "qpr", FinkTankID = 62 };
      var reading = new TeamsPlayer { TeamName = "Reading", Slug = "reading", FinkTankID = 35 };
      var scunthorpe = new TeamsPlayer { TeamName = "Scunthorpe", Slug = "scunthorpe", FinkTankID = 85 };
      var sheffieldunited = new TeamsPlayer { TeamName = "Sheffield United", Slug = "sheffield-united", FinkTankID = 37 };
      var swansea = new TeamsPlayer { TeamName = "Swansea", Slug = "swansea", FinkTankID = 88 };
      var watford = new TeamsPlayer { TeamName = "Watford", Slug = "watford", FinkTankID = 41 };
      var bournemouth = new TeamsPlayer { TeamName = "Bournemouth", Slug = "bournemouth", FinkTankID = 69 };
      var brentford = new TeamsPlayer { TeamName = "Brentford", Slug = "brentford", FinkTankID = 46 };
      var brighton = new TeamsPlayer { TeamName = "Brighton", Slug = "brighton", FinkTankID = 21 };
      var bristolrvs = new TeamsPlayer { TeamName = "Bristol Rvs", Slug = "bristol-rvs", FinkTankID = 70 };
      var carlisle = new TeamsPlayer { TeamName = "Carlisle", Slug = "carlisle", FinkTankID = 73 };
      var charlton = new TeamsPlayer { TeamName = "Charlton", Slug = "charlton", FinkTankID = 5 };
      var colchester = new TeamsPlayer { TeamName = "Colchester", Slug = "colchester", FinkTankID = 51 };
      var dagandred = new TeamsPlayer { TeamName = "Dag and Red", Slug = "dag-and-red", FinkTankID = 2005 };
      var exeter = new TeamsPlayer { TeamName = "Exeter", Slug = "exeter", FinkTankID = 75 };
      var hartlepool = new TeamsPlayer { TeamName = "Hartlepool", Slug = "hartlepool", FinkTankID = 76 };
      var huddersfield = new TeamsPlayer { TeamName = "Huddersfield", Slug = "huddersfield", FinkTankID = 53 };
      var leytonorient = new TeamsPlayer { TeamName = "Leyton Orient", Slug = "leyton-orient", FinkTankID = 79 };
      var miltonkeynes = new TeamsPlayer { TeamName = "Milton Keynes", Slug = "milton-keynes", FinkTankID = 42 };
      var nottscounty = new TeamsPlayer { TeamName = "Notts County", Slug = "notts-county", FinkTankID = 57 };
      var oldham = new TeamsPlayer { TeamName = "Oldham", Slug = "oldham", FinkTankID = 58 };
      var peterboro = new TeamsPlayer { TeamName = "Peterboro", Slug = "peterboro", FinkTankID = 59 };
      var plymouth = new TeamsPlayer { TeamName = "Plymouth", Slug = "plymouth", FinkTankID = 60 };
      var rochdale = new TeamsPlayer { TeamName = "Rochdale", Slug = "rochdale", FinkTankID = 83 };
      var sheffieldweds = new TeamsPlayer { TeamName = "Sheffield Weds", Slug = "sheffield-weds", FinkTankID = 38 };
      var southampton = new TeamsPlayer { TeamName = "Southampton", Slug = "southampton", FinkTankID = 15 };
      var swindon = new TeamsPlayer { TeamName = "Swindon", Slug = "swindon", FinkTankID = 64 };
      var tranmere = new TeamsPlayer { TeamName = "Tranmere", Slug = "tranmere", FinkTankID = 65 };
      var walsall = new TeamsPlayer { TeamName = "Walsall", Slug = "walsall", FinkTankID = 40 };
      var yeovil = new TeamsPlayer { TeamName = "Yeovil", Slug = "yeovil", FinkTankID = 96 };
      var accrington = new TeamsPlayer { TeamName = "Accrington", Slug = "accrington", FinkTankID = 97 };
      var aldershot = new TeamsPlayer { TeamName = "Aldershot", Slug = "aldershot", FinkTankID = 2000 };
      var barnet = new TeamsPlayer { TeamName = "Barnet", Slug = "barnet", FinkTankID = 92 };
      var bradford = new TeamsPlayer { TeamName = "Bradford", Slug = "bradford", FinkTankID = 20 };
      var burton = new TeamsPlayer { TeamName = "Burton", Slug = "burton", FinkTankID = 2002 };
      var bury = new TeamsPlayer { TeamName = "Bury", Slug = "bury", FinkTankID = 71 };
      var cheltenham = new TeamsPlayer { TeamName = "Cheltenham", Slug = "cheltenham", FinkTankID = 49 };
      var chesterfield = new TeamsPlayer { TeamName = "Chesterfield", Slug = "chesterfield", FinkTankID = 50 };
      var crewe = new TeamsPlayer { TeamName = "Crewe", Slug = "crewe", FinkTankID = 52 };
      var gillingham = new TeamsPlayer { TeamName = "Gillingham", Slug = "gillingham", FinkTankID = 26 };
      var hereford = new TeamsPlayer { TeamName = "Hereford", Slug = "hereford", FinkTankID = 98 };
      var lincolncity = new TeamsPlayer { TeamName = "Lincoln City", Slug = "lincoln-city", FinkTankID = 80 };
      var macclesfield = new TeamsPlayer { TeamName = "Macclesfield", Slug = "macclesfield", FinkTankID = 81 };
      var morecambe = new TeamsPlayer { TeamName = "Morecambe", Slug = "morecambe", FinkTankID = 2009 };
      var northampton = new TeamsPlayer { TeamName = "Northampton", Slug = "northampton", FinkTankID = 56 };
      var oxford = new TeamsPlayer { TeamName = "Oxford", Slug = "oxford", FinkTankID = 82 };
      var portvale = new TeamsPlayer { TeamName = "Port Vale", Slug = "port-vale", FinkTankID = 61 };
      var rotherham = new TeamsPlayer { TeamName = "Rotherham", Slug = "rotherham", FinkTankID = 36 };
      var shrewsbury = new TeamsPlayer { TeamName = "Shrewsbury", Slug = "shrewsbury", FinkTankID = 86 };
      var southend = new TeamsPlayer { TeamName = "Southend", Slug = "southend", FinkTankID = 87 };
      var stevenage = new TeamsPlayer { TeamName = "Stevenage", Slug = "stevenage", FinkTankID = 2015 };
      var stockport = new TeamsPlayer { TeamName = "Stockport", Slug = "stockport", FinkTankID = 63 };
      var torquay = new TeamsPlayer { TeamName = "Torquay", Slug = "torquay", FinkTankID = 89 };
      var wycombe = new TeamsPlayer { TeamName = "Wycombe", Slug = "wycombe", FinkTankID = 67 };
      var afcwimbledon = new TeamsPlayer { TeamName = "AFC Wimbledon", Slug = "afc-wimbledon", FinkTankID = 2029 };
      var crawleytown = new TeamsPlayer { TeamName = "Crawley Town", Slug = "crawley-town", FinkTankID = 2004 };
      var fleetwoodtown = new TeamsPlayer { TeamName = "Fleetwood Town", Slug = "fleetwood-town", FinkTankID = 2033 };
      var york = new TeamsPlayer { TeamName = "York", Slug = "york", FinkTankID = 91 };

      context.Set<TeamsPlayer>().AddOrUpdate(arsenal, astonvilla, birmingham, blackburn, blackpool, bolton, chelsea, everton, fulham, liverpool, mancity, manunited, newcastle, stoke, sunderland, tottenham, westbrom, westham, wigan, wolves, barnsley, bristolcity, burnley, cardiff, coventry, crystalpalace, derby, doncaster, hull, ipswich, leeds, leicester, middlesboro, millwall, norwich, nottmforest, portsmouth, preston, qpr, reading, scunthorpe, sheffieldunited, swansea, watford, bournemouth, brentford, brighton, bristolrvs, carlisle, charlton, colchester, dagandred, exeter, hartlepool, huddersfield, leytonorient, miltonkeynes, nottscounty, oldham, peterboro, plymouth, rochdale, sheffieldweds, southampton, swindon, tranmere, walsall, yeovil, accrington, aldershot, barnet, bradford, burton, bury, cheltenham, chesterfield, crewe, gillingham, hereford, lincolncity, macclesfield, morecambe, northampton, oxford, portvale, rotherham, shrewsbury, southend, stevenage, stockport, torquay, wycombe, afcwimbledon, crawleytown, fleetwoodtown, york);

      var ocm_manunited = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = manunited, Alias = "Man Utd" };
      var ocm_wolves = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = wolves, Alias = "Wolverhampton" };
      var ocm_middlesboro = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = middlesboro, Alias = "Middlesbrough" };
      var ocm_nottmforest = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = nottmforest, Alias = "Nottingham Forest" };
      var ocm_sheffieldunited = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = sheffieldunited, Alias = "Sheffield Utd" };
      var ocm_bristolrvs = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = bristolrvs, Alias = "Bristol Rovers" };
      var ocm_dagandred = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = dagandred, Alias = "Dagenham & Redbridge" };
      var ocm_miltonkeynes = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = miltonkeynes, Alias = "MK Dons" };
      var ocm_nottscounty = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = nottscounty, Alias = "Notts Co" };
      var ocm_peterboro = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = peterboro, Alias = "Peterborough" };
      var ocm_sheffieldweds = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = sheffieldweds, Alias = "Sheffield Wednesday" };
      var ocm_lincolncity = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = lincolncity, Alias = "Lincoln" };
      var ocm_crawleytown = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerMobi, TeamsPlayer = crawleytown, Alias = "Crawley" };

      var bb_birmingham = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = birmingham, Alias = "Birmingham" };
      var bb_blackburn = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = blackburn, Alias = "Blackburn" };
      var bb_bolton = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = bolton, Alias = "Bolton" };
      var bb_mancity = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = mancity, Alias = "Man City" };
      var bb_manunited = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = manunited, Alias = "Man Utd" };
      var bb_newcastle = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = newcastle, Alias = "Newcastle" };
      var bb_stoke = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = stoke, Alias = "Stoke" };
      var bb_tottenham = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = tottenham, Alias = "Tottenham" };
      var bb_westbrom = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = westbrom, Alias = "West Brom" };
      var bb_wigan = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = wigan, Alias = "Wigan" };
      var bb_wolves = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = wolves, Alias = "Wolverhampton" };
      var bb_cardiff = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = cardiff, Alias = "Cardiff" };
      var bb_coventry = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = coventry, Alias = "Coventry" };
      var bb_derby = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = derby, Alias = "Derby" };
      var bb_doncaster = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = doncaster, Alias = "Doncaster" };
      var bb_hull = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = hull, Alias = "Hull" };
      var bb_ipswich = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = ipswich, Alias = "Ipswich" };
      var bb_leeds = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = leeds, Alias = "Leeds" };
      var bb_leicester = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = leicester, Alias = "Leicester" };
      var bb_middlesboro = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = middlesboro, Alias = "Middlesbrough" };
      var bb_norwich = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = norwich, Alias = "Norwich" };
      var bb_nottmforest = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = nottmforest, Alias = "Nottingham Forest" };
      var bb_preston = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = preston, Alias = "Preston" };
      var bb_qpr = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = qpr, Alias = "QPR" };
      var bb_scunthorpe = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = scunthorpe, Alias = "Scunthorpe" };
      var bb_swansea = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = swansea, Alias = "Swansea" };
      var bb_bournemouth = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = bournemouth, Alias = "Bournemouth" };
      var bb_brighton = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = brighton, Alias = "Brighton" };
      var bb_bristolrvs = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = bristolrvs, Alias = "Bristol Rovers" };
      var bb_carlisle = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = carlisle, Alias = "Carlisle" };
      var bb_charlton = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = charlton, Alias = "Charlton" };
      var bb_colchester = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = colchester, Alias = "Colchester" };
      var bb_dagandred = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = dagandred, Alias = "Dagenham & Redbridge" };
      var bb_exeter = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = exeter, Alias = "Exeter" };
      var bb_hartlepool = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = hartlepool, Alias = "Hartlepool" };
      var bb_huddersfield = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = huddersfield, Alias = "Huddersfield" };
      var bb_miltonkeynes = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = miltonkeynes, Alias = "MK Dons" };
      var bb_oldham = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = oldham, Alias = "Oldham" };
      var bb_peterboro = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = peterboro, Alias = "Peterborough" };
      var bb_plymouth = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = plymouth, Alias = "Plymouth" };
      var bb_sheffieldweds = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = sheffieldweds, Alias = "Sheffield Wednesday" };
      var bb_swindon = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = swindon, Alias = "Swindon" };
      var bb_tranmere = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = tranmere, Alias = "Tranmere" };
      var bb_yeovil = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = yeovil, Alias = "Yeovil" };
      var bb_accrington = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = accrington, Alias = "Accrington" };
      var bb_aldershot = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = aldershot, Alias = "Aldershot" };
      var bb_bradford = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = bradford, Alias = "Bradford" };
      var bb_burton = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = burton, Alias = "Burton" };
      var bb_cheltenham = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = cheltenham, Alias = "Cheltenham" };
      var bb_crewe = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = crewe, Alias = "Crewe" };
      var bb_hereford = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = hereford, Alias = "Hereford" };
      var bb_lincolncity = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = lincolncity, Alias = "Lincoln" };
      var bb_macclesfield = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = macclesfield, Alias = "Macclesfield" };
      var bb_northampton = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = northampton, Alias = "Northampton" };
      var bb_oxford = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = oxford, Alias = "Oxford" };
      var bb_rotherham = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = rotherham, Alias = "Rotherham" };
      var bb_shrewsbury = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = shrewsbury, Alias = "Shrewsbury" };
      var bb_southend = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = southend, Alias = "Southend" };
      var bb_stockport = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = stockport, Alias = "Stockport" };
      var bb_torquay = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = torquay, Alias = "Torquay" };
      var bb_wycombe = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = wycombe, Alias = "Wycombe" };
      var bb_york = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = york, Alias = "York" };

      context.Set<TeamPlayerExternalSourceAlias>().AddOrUpdate(ocm_manunited, ocm_wolves, ocm_middlesboro, ocm_nottmforest, ocm_sheffieldunited, ocm_bristolrvs, ocm_dagandred, ocm_miltonkeynes, ocm_nottscounty, ocm_peterboro, ocm_sheffieldweds, ocm_lincolncity, ocm_crawleytown, bb_birmingham, bb_blackburn, bb_bolton, bb_mancity, bb_manunited, bb_newcastle, bb_stoke, bb_tottenham, bb_westbrom, bb_wigan, bb_wolves, bb_cardiff, bb_coventry, bb_derby, bb_doncaster, bb_hull, bb_ipswich, bb_leeds, bb_leicester, bb_middlesboro, bb_norwich, bb_nottmforest, bb_preston, bb_qpr, bb_scunthorpe, bb_swansea, bb_bournemouth, bb_brighton, bb_bristolrvs, bb_carlisle, bb_charlton, bb_colchester, bb_dagandred, bb_exeter, bb_hartlepool, bb_huddersfield, bb_miltonkeynes, bb_oldham, bb_peterboro, bb_plymouth, bb_sheffieldweds, bb_swindon, bb_tranmere, bb_yeovil, bb_accrington, bb_aldershot, bb_bradford, bb_burton, bb_cheltenham, bb_crewe, bb_hereford, bb_lincolncity, bb_macclesfield, bb_northampton, bb_oxford, bb_rotherham, bb_shrewsbury, bb_southend, bb_stockport, bb_torquay, bb_wycombe, bb_york);

      var ss_mancity = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = mancity, Alias = "Manchester City" };
      var ss_manunited = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = manunited, Alias = "Manchester United" };
      var ss_newcastle = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = newcastle, Alias = "Newcastle United" };
      var ss_norwich = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = norwich, Alias = "Norwich City" };
      var ss_qpr = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = qpr, Alias = "Queens Park Rangers" };
      var ss_stoke = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = stoke, Alias = "Stoke City" };
      var ss_swansea = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = swansea, Alias = "Swansea City" };
      var ss_tottenham = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = tottenham, Alias = "Tottenham Hotspur" };
      var ss_westbrom = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = westbrom, Alias = "West Bromwich Albion" };
      var ss_westham = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = westham, Alias = "West Ham United" };
      var ss_wigan = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = wigan, Alias = "Wigan Athletic" };
      var ss_birmingham = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = birmingham, Alias = "Birmingham City" };
      var ss_blackburn = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = blackburn, Alias = "Blackburn Rovers" };
      var ss_bolton = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = bolton, Alias = "Bolton Wanderers" };
      var ss_cardiff = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = cardiff, Alias = "Cardiff City" };
      var ss_charlton = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = charlton, Alias = "Charlton Athletic" };
      var ss_derby = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = derby, Alias = "Derby County" };
      var ss_huddersfield = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = huddersfield, Alias = "Huddersfield Town" };
      var ss_hull = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = hull, Alias = "Hull City" };
      var ss_ipswich = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = ipswich, Alias = "Ipswich Town" };
      var ss_leeds = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = leeds, Alias = "Leeds United" };
      var ss_leicester = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = leicester, Alias = "Leicester City" };
      var ss_middlesboro = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = middlesboro, Alias = "Middlesbrough" };
      var ss_nottmforest = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = nottmforest, Alias = "Nottingham Forest" };
      var ss_peterboro = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = peterboro, Alias = "Peterborough United" };
      var ss_sheffieldweds = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = sheffieldweds, Alias = "Sheffield Wednesday" };
      var ss_wolves = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = wolves, Alias = "Wolverhampton" };
      var ss_carlisle = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = carlisle, Alias = "Carlisle United" };
      var ss_colchester = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = colchester, Alias = "Colchester United" };
      var ss_coventry = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = coventry, Alias = "Coventry City" };
      var ss_crewe = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = crewe, Alias = "Crewe Alexandra" };
      var ss_hartlepool = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = hartlepool, Alias = "Hartlepool United" };
      var ss_miltonkeynes = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = miltonkeynes, Alias = "Milton Keynes Dons" };
      var ss_oldham = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = oldham, Alias = "Oldham Athletic" };
      var ss_preston = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = preston, Alias = "Preston North End" };
      var ss_scunthorpe = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = scunthorpe, Alias = "Scunthorpe United" };
      var ss_shrewsbury = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = shrewsbury, Alias = "Shrewsbury Town" };
      var ss_swindon = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = swindon, Alias = "Swindon Town" };
      var ss_tranmere = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = tranmere, Alias = "Tranmere Rovers" };
      var ss_yeovil = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = yeovil, Alias = "Yeovil Town" };
      var ss_accrington = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = accrington, Alias = "Accrington Stanley" };
      var ss_barnet = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = barnet, Alias = "Barnet FC" };
      var ss_bradford = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = bradford, Alias = "Bradford City" };
      var ss_bristolrvs = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = bristolrvs, Alias = "Bristol Rovers" };
      var ss_burton = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = burton, Alias = "Burton Albion" };
      var ss_cheltenham = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = cheltenham, Alias = "Cheltenham Town" };
      var ss_dagandred = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = dagandred, Alias = "Dagenham and Redbridge" };
      var ss_exeter = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = exeter, Alias = "Exeter City" };
      var ss_northampton = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = northampton, Alias = "Northampton Town" };
      var ss_oxford = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = oxford, Alias = "Oxford Utd" };
      var ss_plymouth = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = plymouth, Alias = "Plymouth Argyle" };
      var ss_rotherham = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = rotherham, Alias = "Rotherham United" };
      var ss_southend = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = southend, Alias = "Southend United" };
      var ss_torquay = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = torquay, Alias = "Torquay United" };
      var ss_wycombe = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = wycombe, Alias = "Wycombe Wanderers" };
      var ss_york = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = york, Alias = "York City" };

      context.Set<TeamPlayerExternalSourceAlias>().AddOrUpdate(ss_mancity, ss_manunited, ss_newcastle, ss_norwich, ss_qpr, ss_stoke, ss_swansea, ss_tottenham, ss_westbrom, ss_westham, ss_wigan, ss_birmingham, ss_blackburn, ss_bolton, ss_cardiff, ss_charlton, ss_derby, ss_huddersfield, ss_hull, ss_ipswich, ss_leeds, ss_leicester, ss_middlesboro, ss_nottmforest, ss_peterboro, ss_sheffieldweds, ss_wolves, ss_carlisle, ss_colchester, ss_coventry, ss_crewe, ss_hartlepool, ss_miltonkeynes, ss_oldham, ss_preston, ss_scunthorpe, ss_shrewsbury, ss_swindon, ss_tranmere, ss_yeovil, ss_accrington, ss_barnet, ss_bradford, ss_bristolrvs, ss_burton, ss_cheltenham, ss_dagandred, ss_exeter, ss_northampton, ss_oxford, ss_plymouth, ss_rotherham, ss_southend, ss_torquay, ss_wycombe, ss_york);

      SaveChanges(context);
    }

    private void SaveChanges(ValueSamuraiContext context)
    {
      try
      {
        context.SaveChanges();
      }
      catch (DbEntityValidationException ex)
      {
        StringBuilder sb = new StringBuilder();

        foreach (var failure in ex.EntityValidationErrors)
        {
          sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());

          foreach (var error in failure.ValidationErrors)
          {
            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
            sb.AppendLine();
          }
        }

        throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb.ToString());
      }
    }

  }
}
