using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Collections.Generic;
using WebMatrix.WebData;
using System.Web.Security;

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
      SeedBookmakers(context);
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

    private void SeedBookmakers(ValueSamuraiContext context)
    {
      context.Set<Bookmaker>().AddOrUpdate(
        new Bookmaker { BookmakerName = "10Bet", IsExchange = false },
        new Bookmaker { BookmakerName = "32Red bet", IsExchange = false },
        new Bookmaker { BookmakerName = "888 Sport", IsExchange = false },
        new Bookmaker { BookmakerName = "Bet 365", IsExchange = false },
        new Bookmaker { BookmakerName = "Bet Victor", IsExchange = false },
        new Bookmaker { BookmakerName = "Bet770", IsExchange = false },
        new Bookmaker { BookmakerName = "BETDAQ", IsExchange = false },
        new Bookmaker { BookmakerName = "Betfair", IsExchange = true },
        new Bookmaker { BookmakerName = "Betfred", IsExchange = false },
        new Bookmaker { BookmakerName = "Betinternet", IsExchange = false },
        new Bookmaker { BookmakerName = "BetVictor", IsExchange = false },
        new Bookmaker { BookmakerName = "Blue Square", IsExchange = false },
        new Bookmaker { BookmakerName = "Bodog", IsExchange = false },
        new Bookmaker { BookmakerName = "Boylesports", IsExchange = false },
        new Bookmaker { BookmakerName = "Bwin", IsExchange = false },
        new Bookmaker { BookmakerName = "Coral", IsExchange = false },
        new Bookmaker { BookmakerName = "Corbetts", IsExchange = false },
        new Bookmaker { BookmakerName = "Ladbrokes", IsExchange = false },
        new Bookmaker { BookmakerName = "Matchbook.com", IsExchange = true },
        new Bookmaker { BookmakerName = "Paddy Power", IsExchange = false },
        new Bookmaker { BookmakerName = "Panbet", IsExchange = false },
        new Bookmaker { BookmakerName = "Pinnacle Sports", IsExchange = false },
        new Bookmaker { BookmakerName = "Sky Bet", IsExchange = false },
        new Bookmaker { BookmakerName = "Smarkets", IsExchange = true },
        new Bookmaker { BookmakerName = "Sporting Bet", IsExchange = false },
        new Bookmaker { BookmakerName = "Spreadex", IsExchange = false },
        new Bookmaker { BookmakerName = "Stan James", IsExchange = false },
        new Bookmaker { BookmakerName = "Totesport", IsExchange = false },
        new Bookmaker { BookmakerName = "WBX", IsExchange = false },
        new Bookmaker { BookmakerName = "William Hill", IsExchange = false },
        new Bookmaker { BookmakerName = "youwin", IsExchange = false }
        );
    }

    private void SeedSportsCompetitionsAndFunds(ValueSamuraiContext context)
    {
      //sports
      var football = new Sport { SportName = "Football" };
      var tennis = new Sport { SportName = "Tennis" };

      //competitions
      var premierLeague = new Competition { Sport = football, CompetitionName = "Premier League", OverroundRequired = 0.1M };
      var championship = new Competition { Sport = football, CompetitionName = "Championship", OverroundRequired = 0.1M };
      var leagueOne = new Competition { Sport = football, CompetitionName = "League One", OverroundRequired = 0.1M };
      var leagueTwo = new Competition { Sport = football, CompetitionName = "League Two", OverroundRequired = 0.1M };
      var atp = new Competition { Sport = tennis, CompetitionName = "ATP", OverroundRequired = 0.1M, GamesRequiredForBet = 50 };

      //funds
      var premierFund = new Fund { FundName = "Premier", Bank = 500M, Competitions = new List<Competition>() { premierLeague }, KellyMultiplier = 0.25M };
      var footballLeagueFund = new Fund { FundName = "Football League", Bank = 500M, Competitions = new List<Competition>() { championship, leagueOne, leagueTwo }, KellyMultiplier = 0.25M };
      var tennisFund = new Fund { FundName = "ATP", Bank = 500M, Competitions = new List<Competition>() { atp }, KellyMultiplier = 0.25M };

      //external sources
      var skysports = new ExternalSource { Source = "Sky Sports", OddsSource = false, TheoreticalOddsSource = false };
      var bestBetting = new ExternalSource { Source = "Best Bestting", OddsSource = true, TheoreticalOddsSource = false };
      var oddsCheckerMobi = new ExternalSource { Source = "Odds Checker Mobi", OddsSource = true, TheoreticalOddsSource = false };
      var oddsCheckerWeb = new ExternalSource { Source = "Odds Checker Web", OddsSource = true, TheoreticalOddsSource = false };
      var tennisDataOdds = new ExternalSource { Source = "Tennis Data Odds", OddsSource = true, TheoreticalOddsSource = true };
      var footballDataOdds = new ExternalSource { Source = "Football Data Odds", OddsSource = true, TheoreticalOddsSource = true };
      var tb365 = new ExternalSource { Source = "Tennis Betting 365", OddsSource = false, TheoreticalOddsSource = false };
      var finkTank = new ExternalSource { Source = "Fink Tank (dectech)", OddsSource = false, TheoreticalOddsSource = false };

      //match outcomes
      var teamOrPlayerAWin = new MatchOutcome { MatchOutcomeString = "TeamOrPlayerAWin" };
      var draw = new MatchOutcome { MatchOutcomeString = "Draw" };
      var teamOrPlayerBWin = new MatchOutcome { MatchOutcomeString = "TeamOrPlayerBWin" };

      //scoreOutcomes
      var scores = (
                    from scoreA in Enumerable.Range(0, 15)
                    from scoreB in Enumerable.Range(0, 15)
                    let outcome = (scoreA == scoreB ? draw : (scoreA > scoreB ? teamOrPlayerAWin : teamOrPlayerBWin))
                    select new ScoreOutcome
                    {
                      TeamAScore = scoreA,
                      TeamBScore = scoreB,
                      MatchOutcome = outcome
                    }
                   ).ToList();

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


    }

  }
}
