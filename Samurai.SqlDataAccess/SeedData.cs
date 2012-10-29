﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess
{

  public class SeedDataDictionaries : SeedData
  {
    public IDictionary<string, Bookmaker> Bookmaker { get; set; }
    public IDictionary<string, Sport> Sport { get; set; }
    public IDictionary<string, Competition> Competition { get; set; }
    public IDictionary<string, Tournament> Tournament { get; set; }
    public IDictionary<string, Fund> Fund { get; set; }
    public IDictionary<string, ExternalSource> ExternalSource { get; set; }
    public IDictionary<string, MatchOutcome> MatchOutcome { get; set; }
    public IDictionary<string, ScoreOutcome> ScoreOutcome { get; set; }
    public IDictionary<string, TeamsPlayer> TeamsPlayer { get; set; }
    public IEnumerable<TeamPlayerExternalSourceAlias> TeamPlayerExternalSourceAlias { get; set; }

    public SeedDataDictionaries()
      : base()
    {
      Bookmaker = Bookmakers.ToDictionary(d => d.BookmakerName);
      Sport = Sports.ToDictionary(d => d.SportName);
      Competition = Competitions.ToDictionary(d => d.CompetitionName);
      Tournament = Tournaments.ToDictionary(d => d.TournamentName);
      Fund = Funds.ToDictionary(d => d.FundName);
      ExternalSource = ExternalSources.ToDictionary(d => d.Source);
      MatchOutcome = MatchOutcomes.ToDictionary(d => d.MatchOutcomeString);
      ScoreOutcome = ScoreOutcomes.ToDictionary(d => d.ToString());
      TeamsPlayer = TeamsPlayers.ToDictionary(d => d.TeamName);
      TeamPlayerExternalSourceAlias = TeamPlayerExternalSourceAliass.ToList();
    }
  }

  public class SeedData
  {
    public SeedData()
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

      //sport
      var football = new Sport { SportName = "Football" };
      var tennis = new Sport { SportName = "Tennis" };

      //competition
      var premierLeague = new Competition { Sport = football, CompetitionName = "Premier League", Slug = "premier-league", OverroundRequired = 0.1M };
      var championship = new Competition { Sport = football, CompetitionName = "Championship", Slug = "championship", OverroundRequired = 0.1M };
      var leagueOne = new Competition { Sport = football, CompetitionName = "League One", Slug = "league-one", OverroundRequired = 0.1M };
      var leagueTwo = new Competition { Sport = football, CompetitionName = "League Two", Slug = "league-two", OverroundRequired = 0.1M };
      var atp = new Competition { Sport = tennis, CompetitionName = "ATP", Slug = "atp", OverroundRequired = 0.1M, GamesRequiredForBet = 50 };

      //tournament
      var t_premierLeague = new Tournament { Competition = premierLeague, TournamentName = "Premier League", Slug = "premier-league", Location = "England" };
      var t_championship = new Tournament { Competition = championship, TournamentName = "Championship", Slug = "championship", Location = "England" };
      var t_leagueOne = new Tournament { Competition = leagueOne, TournamentName = "League One", Slug = "league-one", Location = "England" };
      var t_leagueTwo = new Tournament { Competition = leagueTwo, TournamentName = "League Two", Slug = "league-two", Location = "England" };

      var brisbane_international = new Tournament { Competition = atp, TournamentName = "Brisbane International", Slug = "brisbane-international", Location = "Australia" };
      var aircel_chennai_open = new Tournament { Competition = atp, TournamentName = "Aircel Chennai Open", Slug = "aircel-chennai-open", Location = "India" };
      var qatar_exxonmobil_open = new Tournament { Competition = atp, TournamentName = "Qatar ExxonMobil Open", Slug = "qatar-exxonmobil-open", Location = "Qatar" };
      var apia_international_sydney = new Tournament { Competition = atp, TournamentName = "Apia International Sydney", Slug = "apia-international-sydney", Location = "Australia" };
      var heineken_open = new Tournament { Competition = atp, TournamentName = "Heineken Open", Slug = "heineken-open", Location = "New Zealand" };
      var australian_open = new Tournament { Competition = atp, TournamentName = "Australian Open", Slug = "australian-open", Location = "Australia" };
      var open_sud_de_france = new Tournament { Competition = atp, TournamentName = "Open Sud de France", Slug = "open-sud-de-france", Location = "France" };
      var pbz_zagreb_indoors = new Tournament { Competition = atp, TournamentName = "PBZ Zagreb Indoors", Slug = "pbz-zagreb-indoors", Location = "Croatia" };
      var vtr_open = new Tournament { Competition = atp, TournamentName = "VTR Open", Slug = "vtr-open", Location = "Chile" };
      var abn_amro_world_tennis_tournament = new Tournament { Competition = atp, TournamentName = "ABN AMRO World Tennis Tournament", Slug = "abn-amro-world-tennis-tournament", Location = "The Netherlands" };
      var brasil_open_2012 = new Tournament { Competition = atp, TournamentName = "Brasil Open 2012", Slug = "brasil-open-2012", Location = "Brazil" };
      var sap_open = new Tournament { Competition = atp, TournamentName = "SAP Open", Slug = "sap-open", Location = "CA, U.S.A." };
      var regions_morgan_keegan_championships = new Tournament { Competition = atp, TournamentName = "Regions Morgan Keegan Championships", Slug = "regions-morgan-keegan-championships", Location = "TN, U.S.A." };
      var copa_claro = new Tournament { Competition = atp, TournamentName = "Copa Claro", Slug = "copa-claro", Location = "Argentina" };
      var open_13 = new Tournament { Competition = atp, TournamentName = "Open 13", Slug = "open-13", Location = "France" };
      var dubai_duty_free_tennis_championships = new Tournament { Competition = atp, TournamentName = "Dubai Duty Free Tennis Championships", Slug = "dubai-duty-free-tennis-championships", Location = "U.A.E." };
      var delray_beach_international_tennis_championships = new Tournament { Competition = atp, TournamentName = "Delray Beach International Tennis Championships", Slug = "delray-beach-international-tennis-championships", Location = "FL, U.S.A." };
      var abierto_mexicano_telcel = new Tournament { Competition = atp, TournamentName = "Abierto Mexicano Telcel", Slug = "abierto-mexicano-telcel", Location = "Mexico" };
      var bnp_paribas_open = new Tournament { Competition = atp, TournamentName = "BNP Paribas Open", Slug = "bnp-paribas-open", Location = "CA, U.S.A." };
      var sony_ericsson_open = new Tournament { Competition = atp, TournamentName = "Sony Ericsson Open", Slug = "sony-ericsson-open", Location = "FL, U.S.A." };
      var grand_prix_hassan_ii = new Tournament { Competition = atp, TournamentName = "Grand Prix Hassan II", Slug = "grand-prix-hassan-ii", Location = "Morocco" };
      var us_mens_clay_court_championship = new Tournament { Competition = atp, TournamentName = "US Men's Clay Court Championship", Slug = "us-mens-clay-court-championship", Location = "TX, U.S.A." };
      var monte_carlo_rolex_masters = new Tournament { Competition = atp, TournamentName = "Monte-Carlo Rolex Masters", Slug = "monte-carlo-rolex-masters", Location = "Monaco" };
      var brd_nastase_tiriac_trophy = new Tournament { Competition = atp, TournamentName = "BRD Nastase Tiriac Trophy", Slug = "brd-nastase-tiriac-trophy", Location = "Romania" };
      var barcelona_open_banc_sabadell = new Tournament { Competition = atp, TournamentName = "Barcelona Open Banc Sabadell", Slug = "barcelona-open-banc-sabadell", Location = "Spain" };
      var bmw_open = new Tournament { Competition = atp, TournamentName = "BMW Open", Slug = "bmw-open", Location = "Germany" };
      var serbia_open_2012 = new Tournament { Competition = atp, TournamentName = "Serbia Open 2012", Slug = "serbia-open-2012", Location = "Serbia" };
      var estoril_open = new Tournament { Competition = atp, TournamentName = "Estoril Open", Slug = "estoril-open", Location = "Portugal" };
      var mutua_madrid_open = new Tournament { Competition = atp, TournamentName = "Mutua Madrid Open", Slug = "mutua-madrid-open", Location = "Spain" };
      var internazionali_bnl_ditalia = new Tournament { Competition = atp, TournamentName = "Internazionali BNL d'Italia", Slug = "internazionali-bnl-ditalia", Location = "Italy" };
      var open_de_nice_cote_dazur = new Tournament { Competition = atp, TournamentName = "Open de Nice Côte d’Azur", Slug = "open-de-nice-cote-dazur", Location = "France" };
      var roland_garros = new Tournament { Competition = atp, TournamentName = "Roland Garros", Slug = "roland-garros", Location = "France" };
      var gerry_weber_open = new Tournament { Competition = atp, TournamentName = "Gerry Weber Open ", Slug = "gerry-weber-open", Location = "Germany" };
      var aegon_championships = new Tournament { Competition = atp, TournamentName = "AEGON Championships", Slug = "aegon-championships", Location = "Great Britain" };
      var unicef_open = new Tournament { Competition = atp, TournamentName = "UNICEF Open", Slug = "unicef-open", Location = "The Netherlands" };
      var aegon_international = new Tournament { Competition = atp, TournamentName = "AEGON International", Slug = "aegon-international", Location = "Great Britain" };
      var wimbledon = new Tournament { Competition = atp, TournamentName = "Wimbledon", Slug = "wimbledon", Location = "Wimbledon, Great Britain" };
      var mercedescup = new Tournament { Competition = atp, TournamentName = "MercedesCup", Slug = "mercedescup", Location = "Stuttgart, Germany" };
      var campbells_hall_of_fame_tennis_championships = new Tournament { Competition = atp, TournamentName = "Campbell’s Hall of Fame Tennis Championships", Slug = "campbells-hall-of-fame-tennis-championships", Location = "Newport, U.S.A." };
      var skistar_swedish_open = new Tournament { Competition = atp, TournamentName = "SkiStar Swedish Open", Slug = "skistar-swedish-open", Location = "Båstad, Sweden" };
      var atp_studena_croatia_open = new Tournament { Competition = atp, TournamentName = "ATP Studena Croatia Open", Slug = "atp-studena-croatia-open", Location = "Umag, Croatia" };
      var bet_at_home_open___german_tennis_championships_2012 = new Tournament { Competition = atp, TournamentName = "bet-at-home Open - German Tennis Championships 2012", Slug = "bet-at-home-open---german-tennis-championships-2012", Location = "Hamburg, Germany" };
      var atlanta_tennis_championships = new Tournament { Competition = atp, TournamentName = "Atlanta Tennis Championships", Slug = "atlanta-tennis-championships", Location = "Atlanta, U.S.A." };
      var credit_agricole_suisse_open_gstaad = new Tournament { Competition = atp, TournamentName = "Crédit Agricole Suisse Open Gstaad", Slug = "credit-agricole-suisse-open-gstaad", Location = "Gstaad, Switzerland" };
      var bet_at_home_cup_kitzbuhel = new Tournament { Competition = atp, TournamentName = "bet-at-home Cup Kitzbühel", Slug = "bet-at-home-cup-kitzbuhel", Location = "Kitzbühel, Austria" };
      var farmers_classic = new Tournament { Competition = atp, TournamentName = "Farmers Classic", Slug = "farmers-classic", Location = "Los Angeles, U.S.A.  " };
      var legg_mason_tennis_classic = new Tournament { Competition = atp, TournamentName = "Legg Mason Tennis Classic", Slug = "legg-mason-tennis-classic", Location = "Washington D.C., U.S.A." };
      var rogers_cup = new Tournament { Competition = atp, TournamentName = "Rogers Cup", Slug = "rogers-cup", Location = "Toronto, Canada" };
      var western__southern_open = new Tournament { Competition = atp, TournamentName = "Western & Southern Open", Slug = "western--southern-open", Location = "Cincinnati, U.S.A" };
      var winston_salem_open = new Tournament { Competition = atp, TournamentName = "Winston-Salem Open", Slug = "winston-salem-open", Location = "Winston Salem, U.S.A." };
      var us_open = new Tournament { Competition = atp, TournamentName = "US Open", Slug = "us-open", Location = "NY, U.S.A." };
      var moselle_open = new Tournament { Competition = atp, TournamentName = "Moselle Open", Slug = "moselle-open", Location = "Metz, France" };
      var st_petersburg_open = new Tournament { Competition = atp, TournamentName = "St. Petersburg Open", Slug = "st-petersburg-open", Location = "St. Petersburg, Russia" };
      var ptt_thailand_open = new Tournament { Competition = atp, TournamentName = "PTT Thailand Open", Slug = "ptt-thailand-open", Location = "Bangkok, Thailand" };
      var malaysian_open_kuala_lumpur = new Tournament { Competition = atp, TournamentName = "Malaysian Open, Kuala Lumpur", Slug = "malaysian-open-kuala-lumpur", Location = "Kuala Lumpur, Malaysia" };
      var china_open = new Tournament { Competition = atp, TournamentName = "China Open", Slug = "china-open", Location = "Beijing, China" };
      var rakuten_japan_open_tennis_championships = new Tournament { Competition = atp, TournamentName = "Rakuten Japan Open Tennis Championships", Slug = "rakuten-japan-open-tennis-championships", Location = "Tokyo, Japan" };
      var shanghai_rolex_masters = new Tournament { Competition = atp, TournamentName = "Shanghai Rolex Masters", Slug = "shanghai-rolex-masters", Location = "Shanghai, China" };
      var erste_bank_open = new Tournament { Competition = atp, TournamentName = "Erste Bank Open", Slug = "erste-bank-open", Location = "Vienna, Austria" };
      var if_stockholm_open = new Tournament { Competition = atp, TournamentName = "If Stockholm Open", Slug = "if-stockholm-open", Location = "Stockholm, Sweden" };
      var kremlin_cup = new Tournament { Competition = atp, TournamentName = "Kremlin Cup", Slug = "kremlin-cup", Location = "Moscow, Russia " };
      var valencia_open_500 = new Tournament { Competition = atp, TournamentName = "Valencia Open 500", Slug = "valencia-open-500", Location = "Valencia, Spain" };
      var swiss_indoors_basel = new Tournament { Competition = atp, TournamentName = "Swiss Indoors Basel", Slug = "swiss-indoors-basel", Location = "Basel, Switzerland" };
      var bnp_paribas_masters = new Tournament { Competition = atp, TournamentName = "BNP Paribas Masters", Slug = "bnp-paribas-masters", Location = "Paris, France" };

      //tournament events
      var s2012_t_premierLeague = new TournamentEvent { Tournament = t_premierLeague, EventName = "Premier League (2012/13 season)", Slug = "premier-league-2012-2013", StartDate = new DateTime(2012, 8, 18), EndDate = new DateTime(2013, 5, 19) };
      var s2012_t_championship = new TournamentEvent { Tournament = t_championship, EventName = "Championship (2012/13 season)", Slug = "championship-2012-2013", StartDate = new DateTime(2012, 8, 17), EndDate = new DateTime(2013, 5, 4) };
      var s2012_t_leagueOne = new TournamentEvent { Tournament = t_leagueOne, EventName = "League One (2012/13 season)", Slug = "league-one-2012-2013", StartDate = new DateTime(2012, 8, 18), EndDate = new DateTime(2013, 5, 4) };
      var s2012_t_leagueTwo = new TournamentEvent { Tournament = t_leagueTwo, EventName = "League Two (2012/13 season)", Slug = "league-two-2012-2013", StartDate = new DateTime(2012, 8, 18), EndDate = new DateTime(2013, 5, 4) };
      var s2012_brisbane_international = new TournamentEvent { Tournament = brisbane_international, EventName = "Brisbane International (2012)", Slug = "brisbane-international-2012-2013", StartDate = new DateTime(2012, 1, 2), EndDate = new DateTime(2012, 1, 8) };
      var s2012_aircel_chennai_open = new TournamentEvent { Tournament = aircel_chennai_open, EventName = "Aircel Chennai Open (2012)", Slug = "aircel-chennai-open-2012-2013", StartDate = new DateTime(2012, 1, 2), EndDate = new DateTime(2012, 1, 8) };
      var s2012_qatar_exxonmobil_open = new TournamentEvent { Tournament = qatar_exxonmobil_open, EventName = "Qatar ExxonMobil Open (2012)", Slug = "qatar-exxonmobil-open-2012-2013", StartDate = new DateTime(2012, 1, 2), EndDate = new DateTime(2012, 1, 8) };
      var s2012_apia_international_sydney = new TournamentEvent { Tournament = apia_international_sydney, EventName = "Apia International Sydney (2012)", Slug = "apia-international-sydney-2012-2013", StartDate = new DateTime(2012, 1, 9), EndDate = new DateTime(2012, 1, 15) };
      var s2012_heineken_open = new TournamentEvent { Tournament = heineken_open, EventName = "Heineken Open (2012)", Slug = "heineken-open-2012-2013", StartDate = new DateTime(2012, 1, 9), EndDate = new DateTime(2012, 1, 15) };
      var s2012_australian_open = new TournamentEvent { Tournament = australian_open, EventName = "Australian Open (2012)", Slug = "australian-open-2012-2013", StartDate = new DateTime(2012, 1, 16), EndDate = new DateTime(2012, 1, 29) };
      var s2012_open_sud_de_france = new TournamentEvent { Tournament = open_sud_de_france, EventName = "Open Sud de France (2012)", Slug = "open-sud-de-france-2012-2013", StartDate = new DateTime(2012, 1, 30), EndDate = new DateTime(2012, 2, 5) };
      var s2012_pbz_zagreb_indoors = new TournamentEvent { Tournament = pbz_zagreb_indoors, EventName = "PBZ Zagreb Indoors (2012)", Slug = "pbz-zagreb-indoors-2012-2013", StartDate = new DateTime(2012, 1, 30), EndDate = new DateTime(2012, 2, 5) };
      var s2012_vtr_open = new TournamentEvent { Tournament = vtr_open, EventName = "VTR Open (2012)", Slug = "vtr-open-2012-2013", StartDate = new DateTime(2012, 1, 30), EndDate = new DateTime(2012, 2, 5) };
      var s2012_abn_amro_world_tennis_tournament = new TournamentEvent { Tournament = abn_amro_world_tennis_tournament, EventName = "ABN AMRO World Tennis Tournament (2012)", Slug = "abn-amro-world-tennis-tournament-2012-2013", StartDate = new DateTime(2012, 2, 13), EndDate = new DateTime(2012, 2, 19) };
      var s2012_brasil_open_2012 = new TournamentEvent { Tournament = brasil_open_2012, EventName = "Brasil Open 2012 (2012)", Slug = "brasil-open-2012-2012-2013", StartDate = new DateTime(2012, 2, 13), EndDate = new DateTime(2012, 2, 19) };
      var s2012_sap_open = new TournamentEvent { Tournament = sap_open, EventName = "SAP Open (2012)", Slug = "sap-open-2012-2013", StartDate = new DateTime(2012, 2, 13), EndDate = new DateTime(2012, 2, 19) };
      var s2012_regions_morgan_keegan_championships = new TournamentEvent { Tournament = regions_morgan_keegan_championships, EventName = "Regions Morgan Keegan Championships (2012)", Slug = "regions-morgan-keegan-championships-2012-2013", StartDate = new DateTime(2012, 2, 20), EndDate = new DateTime(2012, 2, 26) };
      var s2012_copa_claro = new TournamentEvent { Tournament = copa_claro, EventName = "Copa Claro (2012)", Slug = "copa-claro-2012-2013", StartDate = new DateTime(2012, 2, 20), EndDate = new DateTime(2012, 2, 26) };
      var s2012_open_13 = new TournamentEvent { Tournament = open_13, EventName = "Open 13 (2012)", Slug = "open-13-2012-2013", StartDate = new DateTime(2012, 2, 20), EndDate = new DateTime(2012, 2, 26) };
      var s2012_dubai_duty_free_tennis_championships = new TournamentEvent { Tournament = dubai_duty_free_tennis_championships, EventName = "Dubai Duty Free Tennis Championships (2012)", Slug = "dubai-duty-free-tennis-championships-2012-2013", StartDate = new DateTime(2012, 2, 27), EndDate = new DateTime(2012, 3, 4) };
      var s2012_delray_beach_international_tennis_championships = new TournamentEvent { Tournament = delray_beach_international_tennis_championships, EventName = "Delray Beach International Tennis Championships (2012)", Slug = "delray-beach-international-tennis-championships-2012-2013", StartDate = new DateTime(2012, 2, 27), EndDate = new DateTime(2012, 3, 4) };
      var s2012_abierto_mexicano_telcel = new TournamentEvent { Tournament = abierto_mexicano_telcel, EventName = "Abierto Mexicano Telcel (2012)", Slug = "abierto-mexicano-telcel-2012-2013", StartDate = new DateTime(2012, 2, 27), EndDate = new DateTime(2012, 3, 4) };
      var s2012_bnp_paribas_open = new TournamentEvent { Tournament = bnp_paribas_open, EventName = "BNP Paribas Open (2012)", Slug = "bnp-paribas-open-2012-2013", StartDate = new DateTime(2012, 3, 5), EndDate = new DateTime(2012, 3, 18) };
      var s2012_sony_ericsson_open = new TournamentEvent { Tournament = sony_ericsson_open, EventName = "Sony Ericsson Open (2012)", Slug = "sony-ericsson-open-2012-2013", StartDate = new DateTime(2012, 3, 19), EndDate = new DateTime(2012, 4, 1) };
      var s2012_grand_prix_hassan_ii = new TournamentEvent { Tournament = grand_prix_hassan_ii, EventName = "Grand Prix Hassan II (2012)", Slug = "grand-prix-hassan-ii-2012-2013", StartDate = new DateTime(2012, 4, 9), EndDate = new DateTime(2012, 4, 15) };
      var s2012_us_mens_clay_court_championship = new TournamentEvent { Tournament = us_mens_clay_court_championship, EventName = "US Men's Clay Court Championship (2012)", Slug = "us-mens-clay-court-championship-2012-2013", StartDate = new DateTime(2012, 4, 9), EndDate = new DateTime(2012, 4, 15) };
      var s2012_monte_carlo_rolex_masters = new TournamentEvent { Tournament = monte_carlo_rolex_masters, EventName = "Monte-Carlo Rolex Masters (2012)", Slug = "monte-carlo-rolex-masters-2012-2013", StartDate = new DateTime(2012, 4, 16), EndDate = new DateTime(2012, 4, 22) };
      var s2012_brd_nastase_tiriac_trophy = new TournamentEvent { Tournament = brd_nastase_tiriac_trophy, EventName = "BRD Nastase Tiriac Trophy (2012)", Slug = "brd-nastase-tiriac-trophy-2012-2013", StartDate = new DateTime(2012, 4, 23), EndDate = new DateTime(2012, 4, 29) };
      var s2012_barcelona_open_banc_sabadell = new TournamentEvent { Tournament = barcelona_open_banc_sabadell, EventName = "Barcelona Open Banc Sabadell (2012)", Slug = "barcelona-open-banc-sabadell-2012-2013", StartDate = new DateTime(2012, 4, 23), EndDate = new DateTime(2012, 4, 29) };
      var s2012_bmw_open = new TournamentEvent { Tournament = bmw_open, EventName = "BMW Open (2012)", Slug = "bmw-open-2012-2013", StartDate = new DateTime(2012, 4, 30), EndDate = new DateTime(2012, 5, 6) };
      var s2012_serbia_open_2012 = new TournamentEvent { Tournament = serbia_open_2012, EventName = "Serbia Open 2012 (2012)", Slug = "serbia-open-2012-2012-2013", StartDate = new DateTime(2012, 4, 30), EndDate = new DateTime(2012, 5, 6) };
      var s2012_estoril_open = new TournamentEvent { Tournament = estoril_open, EventName = "Estoril Open (2012)", Slug = "estoril-open-2012-2013", StartDate = new DateTime(2012, 4, 30), EndDate = new DateTime(2012, 5, 6) };
      var s2012_mutua_madrid_open = new TournamentEvent { Tournament = mutua_madrid_open, EventName = "Mutua Madrid Open (2012)", Slug = "mutua-madrid-open-2012-2013", StartDate = new DateTime(2012, 5, 7), EndDate = new DateTime(2012, 5, 13) };
      var s2012_internazionali_bnl_ditalia = new TournamentEvent { Tournament = internazionali_bnl_ditalia, EventName = "Internazionali BNL d'Italia (2012)", Slug = "internazionali-bnl-ditalia-2012-2013", StartDate = new DateTime(2012, 5, 14), EndDate = new DateTime(2012, 5, 20) };
      var s2012_open_de_nice_cote_dazur = new TournamentEvent { Tournament = open_de_nice_cote_dazur, EventName = "Open de Nice Côte d’Azur (2012)", Slug = "open-de-nice-cote-dazur-2012-2013", StartDate = new DateTime(2012, 5, 21), EndDate = new DateTime(2012, 5, 27) };
      var s2012_roland_garros = new TournamentEvent { Tournament = roland_garros, EventName = "Roland Garros (2012)", Slug = "roland-garros-2012-2013", StartDate = new DateTime(2012, 5, 28), EndDate = new DateTime(2012, 6, 10) };
      var s2012_gerry_weber_open = new TournamentEvent { Tournament = gerry_weber_open, EventName = "Gerry Weber Open  (2012)", Slug = "gerry-weber-open-2012-2013", StartDate = new DateTime(2012, 6, 11), EndDate = new DateTime(2012, 6, 17) };
      var s2012_aegon_championships = new TournamentEvent { Tournament = aegon_championships, EventName = "AEGON Championships (2012)", Slug = "aegon-championships-2012-2013", StartDate = new DateTime(2012, 6, 11), EndDate = new DateTime(2012, 6, 17) };
      var s2012_unicef_open = new TournamentEvent { Tournament = unicef_open, EventName = "UNICEF Open (2012)", Slug = "unicef-open-2012-2013", StartDate = new DateTime(2012, 6, 18), EndDate = new DateTime(2012, 6, 24) };
      var s2012_aegon_international = new TournamentEvent { Tournament = aegon_international, EventName = "AEGON International (2012)", Slug = "aegon-international-2012-2013", StartDate = new DateTime(2012, 6, 18), EndDate = new DateTime(2012, 6, 24) };
      var s2012_wimbledon = new TournamentEvent { Tournament = wimbledon, EventName = "Wimbledon (2012)", Slug = "wimbledon-2012-2013", StartDate = new DateTime(2012, 6, 25), EndDate = new DateTime(2012, 7, 8) };
      var s2012_mercedescup = new TournamentEvent { Tournament = mercedescup, EventName = "MercedesCup (2012)", Slug = "mercedescup-2012-2013", StartDate = new DateTime(2012, 7, 9), EndDate = new DateTime(2012, 7, 15) };
      var s2012_campbells_hall_of_fame_tennis_championships = new TournamentEvent { Tournament = campbells_hall_of_fame_tennis_championships, EventName = "Campbell’s Hall of Fame Tennis Championships (2012)", Slug = "campbells-hall-of-fame-tennis-championships-2012-2013", StartDate = new DateTime(2012, 7, 9), EndDate = new DateTime(2012, 7, 15) };
      var s2012_skistar_swedish_open = new TournamentEvent { Tournament = skistar_swedish_open, EventName = "SkiStar Swedish Open (2012)", Slug = "skistar-swedish-open-2012-2013", StartDate = new DateTime(2012, 7, 9), EndDate = new DateTime(2012, 7, 15) };
      var s2012_atp_studena_croatia_open = new TournamentEvent { Tournament = atp_studena_croatia_open, EventName = "ATP Studena Croatia Open (2012)", Slug = "atp-studena-croatia-open-2012-2013", StartDate = new DateTime(2012, 7, 9), EndDate = new DateTime(2012, 7, 15) };
      var s2012_bet_at_home_open___german_tennis_championships_2012 = new TournamentEvent { Tournament = bet_at_home_open___german_tennis_championships_2012, EventName = "bet-at-home Open - German Tennis Championships 2012 (2012)", Slug = "bet-at-home-open---german-tennis-championships-2012-2012-2013", StartDate = new DateTime(2012, 7, 16), EndDate = new DateTime(2012, 7, 22) };
      var s2012_atlanta_tennis_championships = new TournamentEvent { Tournament = atlanta_tennis_championships, EventName = "Atlanta Tennis Championships (2012)", Slug = "atlanta-tennis-championships-2012-2013", StartDate = new DateTime(2012, 7, 16), EndDate = new DateTime(2012, 7, 22) };
      var s2012_credit_agricole_suisse_open_gstaad = new TournamentEvent { Tournament = credit_agricole_suisse_open_gstaad, EventName = "Crédit Agricole Suisse Open Gstaad (2012)", Slug = "credit-agricole-suisse-open-gstaad-2012-2013", StartDate = new DateTime(2012, 7, 16), EndDate = new DateTime(2012, 7, 22) };
      var s2012_bet_at_home_cup_kitzbuhel = new TournamentEvent { Tournament = bet_at_home_cup_kitzbuhel, EventName = "bet-at-home Cup Kitzbühel (2012)", Slug = "bet-at-home-cup-kitzbuhel-2012-2013", StartDate = new DateTime(2012, 7, 22), EndDate = new DateTime(2012, 7, 28) };
      var s2012_farmers_classic = new TournamentEvent { Tournament = farmers_classic, EventName = "Farmers Classic (2012)", Slug = "farmers-classic-2012-2013", StartDate = new DateTime(2012, 7, 23), EndDate = new DateTime(2012, 7, 29) };
      var s2012_legg_mason_tennis_classic = new TournamentEvent { Tournament = legg_mason_tennis_classic, EventName = "Legg Mason Tennis Classic (2012)", Slug = "legg-mason-tennis-classic-2012-2013", StartDate = new DateTime(2012, 7, 30), EndDate = new DateTime(2012, 8, 5) };
      var s2012_rogers_cup = new TournamentEvent { Tournament = rogers_cup, EventName = "Rogers Cup (2012)", Slug = "rogers-cup-2012-2013", StartDate = new DateTime(2012, 8, 6), EndDate = new DateTime(2012, 8, 12) };
      var s2012_western__southern_open = new TournamentEvent { Tournament = western__southern_open, EventName = "Western & Southern Open (2012)", Slug = "western--southern-open-2012-2013", StartDate = new DateTime(2012, 8, 13), EndDate = new DateTime(2012, 8, 19) };
      var s2012_winston_salem_open = new TournamentEvent { Tournament = winston_salem_open, EventName = "Winston-Salem Open (2012)", Slug = "winston-salem-open-2012-2013", StartDate = new DateTime(2012, 8, 19), EndDate = new DateTime(2012, 8, 25) };
      var s2012_us_open = new TournamentEvent { Tournament = us_open, EventName = "US Open (2012)", Slug = "us-open-2012-2013", StartDate = new DateTime(2012, 8, 27), EndDate = new DateTime(2012, 9, 9) };
      var s2012_moselle_open = new TournamentEvent { Tournament = moselle_open, EventName = "Moselle Open (2012)", Slug = "moselle-open-2012-2013", StartDate = new DateTime(2012, 9, 17), EndDate = new DateTime(2012, 9, 23) };
      var s2012_st_petersburg_open = new TournamentEvent { Tournament = st_petersburg_open, EventName = "St. Petersburg Open (2012)", Slug = "st-petersburg-open-2012-2013", StartDate = new DateTime(2012, 9, 17), EndDate = new DateTime(2012, 9, 23) };
      var s2012_ptt_thailand_open = new TournamentEvent { Tournament = ptt_thailand_open, EventName = "PTT Thailand Open (2012)", Slug = "ptt-thailand-open-2012-2013", StartDate = new DateTime(2012, 9, 24), EndDate = new DateTime(2012, 9, 30) };
      var s2012_malaysian_open_kuala_lumpur = new TournamentEvent { Tournament = malaysian_open_kuala_lumpur, EventName = "Malaysian Open, Kuala Lumpur (2012)", Slug = "malaysian-open-kuala-lumpur-2012-2013", StartDate = new DateTime(2012, 9, 24), EndDate = new DateTime(2012, 9, 30) };
      var s2012_china_open = new TournamentEvent { Tournament = china_open, EventName = "China Open (2012)", Slug = "china-open-2012-2013", StartDate = new DateTime(2012, 10, 1), EndDate = new DateTime(2012, 10, 7) };
      var s2012_rakuten_japan_open_tennis_championships = new TournamentEvent { Tournament = rakuten_japan_open_tennis_championships, EventName = "Rakuten Japan Open Tennis Championships (2012)", Slug = "rakuten-japan-open-tennis-championships-2012-2013", StartDate = new DateTime(2012, 10, 1), EndDate = new DateTime(2012, 10, 7) };
      var s2012_shanghai_rolex_masters = new TournamentEvent { Tournament = shanghai_rolex_masters, EventName = "Shanghai Rolex Masters (2012)", Slug = "shanghai-rolex-masters-2012-2013", StartDate = new DateTime(2012, 10, 7), EndDate = new DateTime(2012, 10, 14) };
      var s2012_erste_bank_open = new TournamentEvent { Tournament = erste_bank_open, EventName = "Erste Bank Open (2012)", Slug = "erste-bank-open-2012-2013", StartDate = new DateTime(2012, 10, 15), EndDate = new DateTime(2012, 10, 21) };
      var s2012_if_stockholm_open = new TournamentEvent { Tournament = if_stockholm_open, EventName = "If Stockholm Open (2012)", Slug = "if-stockholm-open-2012-2013", StartDate = new DateTime(2012, 10, 15), EndDate = new DateTime(2012, 10, 21) };
      var s2012_kremlin_cup = new TournamentEvent { Tournament = kremlin_cup, EventName = "Kremlin Cup (2012)", Slug = "kremlin-cup-2012-2013", StartDate = new DateTime(2012, 10, 15), EndDate = new DateTime(2012, 10, 21) };
      var s2012_valencia_open_500 = new TournamentEvent { Tournament = valencia_open_500, EventName = "Valencia Open 500 (2012)", Slug = "valencia-open-500-2012-2013", StartDate = new DateTime(2012, 10, 22), EndDate = new DateTime(2012, 10, 28) };
      var s2012_swiss_indoors_basel = new TournamentEvent { Tournament = swiss_indoors_basel, EventName = "Swiss Indoors Basel (2012)", Slug = "swiss-indoors-basel-2012-2013", StartDate = new DateTime(2012, 10, 22), EndDate = new DateTime(2012, 10, 28) };
      var s2012_bnp_paribas_masters = new TournamentEvent { Tournament = bnp_paribas_masters, EventName = "BNP Paribas Masters (2012)", Slug = "bnp-paribas-masters-2012-2013", StartDate = new DateTime(2012, 10, 29), EndDate = new DateTime(2012, 11, 4) };

      //funds
      var premierFund = new Fund { FundName = "Premier", Bank = 500M, Competitions = new List<Competition>() { premierLeague }, KellyMultiplier = 0.25M };
      var footballLeagueFund = new Fund { FundName = "Football League", Bank = 500M, Competitions = new List<Competition>() { championship, leagueOne, leagueTwo }, KellyMultiplier = 0.25M };
      var tennisFund = new Fund { FundName = "ATP", Bank = 500M, Competitions = new List<Competition>() { atp }, KellyMultiplier = 0.25M };

      //external source
      var valueSamurai = new ExternalSource { Source = "Value Samurai", OddsSource = false, TheoreticalOddsSource = false };
      var skySports = new ExternalSource { Source = "Sky Sports", OddsSource = false, TheoreticalOddsSource = false };
      var bestBetting = new ExternalSource { Source = "Best Betting", OddsSource = true, TheoreticalOddsSource = false };
      var oddsCheckerMobi = new ExternalSource { Source = "Odds Checker Mobi", OddsSource = true, TheoreticalOddsSource = false };
      var oddsCheckerWeb = new ExternalSource { Source = "Odds Checker Web", OddsSource = true, TheoreticalOddsSource = false };
      var tennisDataOdds = new ExternalSource { Source = "Tennis Data Odds", OddsSource = true, TheoreticalOddsSource = true };
      var footballDataOdds = new ExternalSource { Source = "Football Data Odds", OddsSource = true, TheoreticalOddsSource = true };
      var tb365 = new ExternalSource { Source = "Tennis Betting 365", OddsSource = false, TheoreticalOddsSource = false };
      var finkTank = new ExternalSource { Source = "Fink Tank (dectech)", OddsSource = false, TheoreticalOddsSource = false };

      //match outcomes
      var teamOrPlayerAWin = new MatchOutcome { MatchOutcomeString = "Home Win" };
      var draw = new MatchOutcome { MatchOutcomeString = "Draw" };
      var teamOrPlayerBWin = new MatchOutcome { MatchOutcomeString = "Away Win" };

      //score outcomes
      var scoreOutcomes = 
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
      ).ToArray();

      //teams
      var arsenal = new TeamsPlayer { TeamName = "Arsenal", Slug = "arsenal", ExternalID = "0" };
      var astonvilla = new TeamsPlayer { TeamName = "Aston Villa", Slug = "aston-villa", ExternalID = "1" };
      var birmingham = new TeamsPlayer { TeamName = "Birmingham", Slug = "birmingham", ExternalID = "2" };
      var blackburn = new TeamsPlayer { TeamName = "Blackburn", Slug = "blackburn", ExternalID = "3" };
      var blackpool = new TeamsPlayer { TeamName = "Blackpool", Slug = "blackpool", ExternalID = "45" };
      var bolton = new TeamsPlayer { TeamName = "Bolton", Slug = "bolton", ExternalID = "4" };
      var chelsea = new TeamsPlayer { TeamName = "Chelsea", Slug = "chelsea", ExternalID = "6" };
      var everton = new TeamsPlayer { TeamName = "Everton", Slug = "everton", ExternalID = "7" };
      var fulham = new TeamsPlayer { TeamName = "Fulham", Slug = "fulham", ExternalID = "8" };
      var liverpool = new TeamsPlayer { TeamName = "Liverpool", Slug = "liverpool", ExternalID = "10" };
      var mancity = new TeamsPlayer { TeamName = "Man City", Slug = "man-city", ExternalID = "11" };
      var manunited = new TeamsPlayer { TeamName = "Man United", Slug = "man-united", ExternalID = "12" };
      var newcastle = new TeamsPlayer { TeamName = "Newcastle", Slug = "newcastle", ExternalID = "14" };
      var stoke = new TeamsPlayer { TeamName = "Stoke", Slug = "stoke", ExternalID = "39" };
      var sunderland = new TeamsPlayer { TeamName = "Sunderland", Slug = "sunderland", ExternalID = "16" };
      var tottenham = new TeamsPlayer { TeamName = "Tottenham", Slug = "tottenham", ExternalID = "17" };
      var westbrom = new TeamsPlayer { TeamName = "West Brom", Slug = "west-brom", ExternalID = "18" };
      var westham = new TeamsPlayer { TeamName = "West Ham", Slug = "west-ham", ExternalID = "19" };
      var wigan = new TeamsPlayer { TeamName = "Wigan", Slug = "wigan", ExternalID = "66" };
      var wolves = new TeamsPlayer { TeamName = "Wolves", Slug = "wolves", ExternalID = "43" };
      var barnsley = new TeamsPlayer { TeamName = "Barnsley", Slug = "barnsley", ExternalID = "44" };
      var bristolcity = new TeamsPlayer { TeamName = "Bristol City", Slug = "bristol-city", ExternalID = "47" };
      var burnley = new TeamsPlayer { TeamName = "Burnley", Slug = "burnley", ExternalID = "22" };
      var cardiff = new TeamsPlayer { TeamName = "Cardiff", Slug = "cardiff", ExternalID = "48" };
      var coventry = new TeamsPlayer { TeamName = "Coventry", Slug = "coventry", ExternalID = "23" };
      var crystalpalace = new TeamsPlayer { TeamName = "Crystal Palace", Slug = "crystal-palace", ExternalID = "24" };
      var derby = new TeamsPlayer { TeamName = "Derby", Slug = "derby", ExternalID = "25" };
      var doncaster = new TeamsPlayer { TeamName = "Doncaster", Slug = "doncaster", ExternalID = "95" };
      var hull = new TeamsPlayer { TeamName = "Hull", Slug = "hull", ExternalID = "77" };
      var ipswich = new TeamsPlayer { TeamName = "Ipswich", Slug = "ipswich", ExternalID = "28" };
      var leeds = new TeamsPlayer { TeamName = "Leeds", Slug = "leeds", ExternalID = "9" };
      var leicester = new TeamsPlayer { TeamName = "Leicester", Slug = "leicester", ExternalID = "29" };
      var middlesboro = new TeamsPlayer { TeamName = "Middlesboro", Slug = "middlesboro", ExternalID = "13" };
      var millwall = new TeamsPlayer { TeamName = "Millwall", Slug = "millwall", ExternalID = "30" };
      var norwich = new TeamsPlayer { TeamName = "Norwich", Slug = "norwich", ExternalID = "31" };
      var nottmforest = new TeamsPlayer { TeamName = "Nott'm Forest", Slug = "nottm-forest", ExternalID = "32" };
      var portsmouth = new TeamsPlayer { TeamName = "Portsmouth", Slug = "portsmouth", ExternalID = "33" };
      var preston = new TeamsPlayer { TeamName = "Preston", Slug = "preston", ExternalID = "34" };
      var qpr = new TeamsPlayer { TeamName = "QPR", Slug = "qpr", ExternalID = "62" };
      var reading = new TeamsPlayer { TeamName = "Reading", Slug = "reading", ExternalID = "35" };
      var scunthorpe = new TeamsPlayer { TeamName = "Scunthorpe", Slug = "scunthorpe", ExternalID = "85" };
      var sheffieldunited = new TeamsPlayer { TeamName = "Sheffield United", Slug = "sheffield-united", ExternalID = "37" };
      var swansea = new TeamsPlayer { TeamName = "Swansea", Slug = "swansea", ExternalID = "88" };
      var watford = new TeamsPlayer { TeamName = "Watford", Slug = "watford", ExternalID = "41" };
      var bournemouth = new TeamsPlayer { TeamName = "Bournemouth", Slug = "bournemouth", ExternalID = "69" };
      var brentford = new TeamsPlayer { TeamName = "Brentford", Slug = "brentford", ExternalID = "46" };
      var brighton = new TeamsPlayer { TeamName = "Brighton", Slug = "brighton", ExternalID = "21" };
      var bristolrvs = new TeamsPlayer { TeamName = "Bristol Rvs", Slug = "bristol-rvs", ExternalID = "70" };
      var carlisle = new TeamsPlayer { TeamName = "Carlisle", Slug = "carlisle", ExternalID = "73" };
      var charlton = new TeamsPlayer { TeamName = "Charlton", Slug = "charlton", ExternalID = "5" };
      var colchester = new TeamsPlayer { TeamName = "Colchester", Slug = "colchester", ExternalID = "51" };
      var dagandred = new TeamsPlayer { TeamName = "Dag and Red", Slug = "dag-and-red", ExternalID = "2005" };
      var exeter = new TeamsPlayer { TeamName = "Exeter", Slug = "exeter", ExternalID = "75" };
      var hartlepool = new TeamsPlayer { TeamName = "Hartlepool", Slug = "hartlepool", ExternalID = "76" };
      var huddersfield = new TeamsPlayer { TeamName = "Huddersfield", Slug = "huddersfield", ExternalID = "53" };
      var leytonorient = new TeamsPlayer { TeamName = "Leyton Orient", Slug = "leyton-orient", ExternalID = "79" };
      var miltonkeynes = new TeamsPlayer { TeamName = "Milton Keynes", Slug = "milton-keynes", ExternalID = "42" };
      var nottscounty = new TeamsPlayer { TeamName = "Notts County", Slug = "notts-county", ExternalID = "57" };
      var oldham = new TeamsPlayer { TeamName = "Oldham", Slug = "oldham", ExternalID = "58" };
      var peterboro = new TeamsPlayer { TeamName = "Peterboro", Slug = "peterboro", ExternalID = "59" };
      var plymouth = new TeamsPlayer { TeamName = "Plymouth", Slug = "plymouth", ExternalID = "60" };
      var rochdale = new TeamsPlayer { TeamName = "Rochdale", Slug = "rochdale", ExternalID = "83" };
      var sheffieldweds = new TeamsPlayer { TeamName = "Sheffield Weds", Slug = "sheffield-weds", ExternalID = "38" };
      var southampton = new TeamsPlayer { TeamName = "Southampton", Slug = "southampton", ExternalID = "15" };
      var swindon = new TeamsPlayer { TeamName = "Swindon", Slug = "swindon", ExternalID = "64" };
      var tranmere = new TeamsPlayer { TeamName = "Tranmere", Slug = "tranmere", ExternalID = "65" };
      var walsall = new TeamsPlayer { TeamName = "Walsall", Slug = "walsall", ExternalID = "40" };
      var yeovil = new TeamsPlayer { TeamName = "Yeovil", Slug = "yeovil", ExternalID = "96" };
      var accrington = new TeamsPlayer { TeamName = "Accrington", Slug = "accrington", ExternalID = "97" };
      var aldershot = new TeamsPlayer { TeamName = "Aldershot", Slug = "aldershot", ExternalID = "2000" };
      var barnet = new TeamsPlayer { TeamName = "Barnet", Slug = "barnet", ExternalID = "92" };
      var bradford = new TeamsPlayer { TeamName = "Bradford", Slug = "bradford", ExternalID = "20" };
      var burton = new TeamsPlayer { TeamName = "Burton", Slug = "burton", ExternalID = "2002" };
      var bury = new TeamsPlayer { TeamName = "Bury", Slug = "bury", ExternalID = "71" };
      var cheltenham = new TeamsPlayer { TeamName = "Cheltenham", Slug = "cheltenham", ExternalID = "49" };
      var chesterfield = new TeamsPlayer { TeamName = "Chesterfield", Slug = "chesterfield", ExternalID = "50" };
      var crewe = new TeamsPlayer { TeamName = "Crewe", Slug = "crewe", ExternalID = "52" };
      var gillingham = new TeamsPlayer { TeamName = "Gillingham", Slug = "gillingham", ExternalID = "26" };
      var hereford = new TeamsPlayer { TeamName = "Hereford", Slug = "hereford", ExternalID = "98" };
      var lincolncity = new TeamsPlayer { TeamName = "Lincoln City", Slug = "lincoln-city", ExternalID = "80" };
      var macclesfield = new TeamsPlayer { TeamName = "Macclesfield", Slug = "macclesfield", ExternalID = "81" };
      var morecambe = new TeamsPlayer { TeamName = "Morecambe", Slug = "morecambe", ExternalID = "2009" };
      var northampton = new TeamsPlayer { TeamName = "Northampton", Slug = "northampton", ExternalID = "56" };
      var oxford = new TeamsPlayer { TeamName = "Oxford", Slug = "oxford", ExternalID = "82" };
      var portvale = new TeamsPlayer { TeamName = "Port Vale", Slug = "port-vale", ExternalID = "61" };
      var rotherham = new TeamsPlayer { TeamName = "Rotherham", Slug = "rotherham", ExternalID = "36" };
      var shrewsbury = new TeamsPlayer { TeamName = "Shrewsbury", Slug = "shrewsbury", ExternalID = "86" };
      var southend = new TeamsPlayer { TeamName = "Southend", Slug = "southend", ExternalID = "87" };
      var stevenage = new TeamsPlayer { TeamName = "Stevenage", Slug = "stevenage", ExternalID = "2015" };
      var stockport = new TeamsPlayer { TeamName = "Stockport", Slug = "stockport", ExternalID = "63" };
      var torquay = new TeamsPlayer { TeamName = "Torquay", Slug = "torquay", ExternalID = "89" };
      var wycombe = new TeamsPlayer { TeamName = "Wycombe", Slug = "wycombe", ExternalID = "67" };
      var afcwimbledon = new TeamsPlayer { TeamName = "AFC Wimbledon", Slug = "afc-wimbledon", ExternalID = "2029" };
      var crawleytown = new TeamsPlayer { TeamName = "Crawley Town", Slug = "crawley-town", ExternalID = "2004" };
      var fleetwoodtown = new TeamsPlayer { TeamName = "Fleetwood Town", Slug = "fleetwood-town", ExternalID = "2033" };
      var york = new TeamsPlayer { TeamName = "York", Slug = "york", ExternalID = "91" };

      //Alias
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

      var ocw_manunited = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = manunited, Alias = "Man Utd" };
      var ocw_wolves = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = wolves, Alias = "Wolverhampton" };
      var ocw_middlesboro = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = middlesboro, Alias = "Middlesbrough" };
      var ocw_nottmforest = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = nottmforest, Alias = "Nottingham Forest" };
      var ocw_sheffieldunited = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = sheffieldunited, Alias = "Sheffield Utd" };
      var ocw_bristolrvs = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = bristolrvs, Alias = "Bristol Rovers" };
      var ocw_dagandred = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = dagandred, Alias = "Dagenham & Redbridge" };
      var ocw_miltonkeynes = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = miltonkeynes, Alias = "MK Dons" };
      var ocw_nottscounty = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = nottscounty, Alias = "Notts Co" };
      var ocw_peterboro = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = peterboro, Alias = "Peterborough" };
      var ocw_sheffieldweds = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = sheffieldweds, Alias = "Sheffield Wednesday" };
      var ocw_lincolncity = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = lincolncity, Alias = "Lincoln" };
      var ocw_crawleytown = new TeamPlayerExternalSourceAlias { ExternalSource = oddsCheckerWeb, TeamsPlayer = crawleytown, Alias = "Crawley" };


      var bb_birmingham = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = birmingham, Alias = "Birmingham City" };
      var bb_blackburn = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = blackburn, Alias = "Blackburn Rovers" };
      var bb_bolton = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = bolton, Alias = "Bolton Wanderers" };
      var bb_mancity = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = mancity, Alias = "Manchester City" };
      var bb_manunited = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = manunited, Alias = "Manchester United" };
      var bb_newcastle = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = newcastle, Alias = "Newcastle United" };
      var bb_stoke = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = stoke, Alias = "Stoke City" };
      var bb_tottenham = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = tottenham, Alias = "Tottenham Hotspur" };
      var bb_westbrom = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = westbrom, Alias = "West Bromwich Albion" };
      var bb_wigan = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = wigan, Alias = "Wigan Athletic" };
      var bb_wolves = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = wolves, Alias = "Wolverhampton Wanderers" };
      var bb_cardiff = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = cardiff, Alias = "Cardiff City" };
      var bb_coventry = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = coventry, Alias = "Coventry City" };
      var bb_derby = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = derby, Alias = "Derby County" };
      var bb_doncaster = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = doncaster, Alias = "Doncaster Rovers" };
      var bb_hull = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = hull, Alias = "Hull City" };
      var bb_ipswich = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = ipswich, Alias = "Ipswich Town" };
      var bb_leeds = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = leeds, Alias = "Leeds United" };
      var bb_leicester = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = leicester, Alias = "Leicester City" };
      var bb_middlesboro = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = middlesboro, Alias = "Middlesbrough" };
      var bb_norwich = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = norwich, Alias = "Norwich City" };
      var bb_nottmforest = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = nottmforest, Alias = "Nottingham Forest" };
      var bb_preston = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = preston, Alias = "Preston North End" };
      var bb_qpr = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = qpr, Alias = "Queens Park Rangers" };
      var bb_scunthorpe = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = scunthorpe, Alias = "Scunthorpe United" };
      var bb_swansea = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = swansea, Alias = "Swansea City" };
      var bb_bournemouth = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = bournemouth, Alias = "AFC Bournemouth" };
      var bb_brighton = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = brighton, Alias = "Brighton & Hove Albion" };
      var bb_bristolrvs = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = bristolrvs, Alias = "Bristol Rovers" };
      var bb_carlisle = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = carlisle, Alias = "Carlisle United" };
      var bb_charlton = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = charlton, Alias = "Charlton Athletic" };
      var bb_colchester = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = colchester, Alias = "Colchester United" };
      var bb_dagandred = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = dagandred, Alias = "Dagenham & Redbridge" };
      var bb_exeter = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = exeter, Alias = "Exeter City" };
      var bb_hartlepool = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = hartlepool, Alias = "Hartlepool United" };
      var bb_huddersfield = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = huddersfield, Alias = "Huddersfield Town" };
      var bb_miltonkeynes = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = miltonkeynes, Alias = "Milton Keynes Dons" };
      var bb_oldham = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = oldham, Alias = "Oldham Athletic" };
      var bb_peterboro = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = peterboro, Alias = "Peterborough United" };
      var bb_plymouth = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = plymouth, Alias = "Plymouth Argyle" };
      var bb_sheffieldweds = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = sheffieldweds, Alias = "Sheffield Wednesday" };
      var bb_swindon = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = swindon, Alias = "Swindon Town" };
      var bb_tranmere = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = tranmere, Alias = "Tranmere Rovers" };
      var bb_yeovil = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = yeovil, Alias = "Yeovil Town" };
      var bb_accrington = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = accrington, Alias = "Accrington Stanley" };
      var bb_aldershot = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = aldershot, Alias = "Aldershot Town" };
      var bb_bradford = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = bradford, Alias = "Bradford City" };
      var bb_burton = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = burton, Alias = "Burton Albion" };
      var bb_cheltenham = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = cheltenham, Alias = "Cheltenham Town" };
      var bb_crewe = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = crewe, Alias = "Crewe Alexandra" };
      var bb_hereford = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = hereford, Alias = "Hereford United" };
      var bb_lincolncity = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = lincolncity, Alias = "Lincoln City" };
      var bb_macclesfield = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = macclesfield, Alias = "Macclesfield Town" };
      var bb_northampton = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = northampton, Alias = "Northampton Town" };
      var bb_oxford = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = oxford, Alias = "Oxford United" };
      var bb_rotherham = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = rotherham, Alias = "Rotherham United" };
      var bb_shrewsbury = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = shrewsbury, Alias = "Shrewsbury Town" };
      var bb_southend = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = southend, Alias = "Southend United" };
      var bb_stockport = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = stockport, Alias = "Stockport County" };
      var bb_torquay = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = torquay, Alias = "Torquay United" };
      var bb_wycombe = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = wycombe, Alias = "Wycombe Wanderers" };
      var bb_york = new TeamPlayerExternalSourceAlias { ExternalSource = bestBetting, TeamsPlayer = york, Alias = "York City" };

      var ss_astonvilla = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = astonvilla, Alias = "A Villa" };
      var ss_afcwimbledon = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = afcwimbledon, Alias = "AFC W'don" };
      var ss_bournemouth = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = bournemouth, Alias = "Bournemth" };
      var ss_bristolcity = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = bristolcity, Alias = "Bristol C" };
      var ss_bristolrvs = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = bristolrvs, Alias = "Bristol R" };
      var ss_burton = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = burton, Alias = "Burton Alb" };
      var ss_crystalpalace = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = crystalpalace, Alias = "C Palace" };
      var ss_chesterfield = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = chesterfield, Alias = "Chesterfld" };
      var ss_crawleytown = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = crawleytown, Alias = "Crawley" };
      var ss_dagandred = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = dagandred, Alias = "Dag + Red" };
      var ss_fleetwoodtown = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = fleetwoodtown, Alias = "Fleetwood" };
      var ss_huddersfield = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = huddersfield, Alias = "Huddersfld" };
      var ss_hull = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = hull, Alias = "Hull City" };
      var ss_leytonorient = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = leytonorient, Alias = "Leyton Or" };
      var ss_manunited = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = manunited, Alias = "Man Utd" };
      var ss_middlesboro = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = middlesboro, Alias = "Middlesbro'" };
      var ss_miltonkeynes = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = miltonkeynes, Alias = "MK Dons" };
      var ss_nottmforest = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = nottmforest, Alias = "N Forest" };
      var ss_northampton = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = northampton, Alias = "Northamptn" };
      var ss_nottscounty = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = nottscounty, Alias = "Notts Co" };
      var ss_oxford = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = oxford, Alias = "Oxford Utd" };
      var ss_sheffieldunited = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = sheffieldunited, Alias = "Sheff Utd" };
      var ss_sheffieldweds = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = sheffieldweds, Alias = "Sheff Wed" };
      var ss_southampton = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = southampton, Alias = "Southamptn" };
      var ss_stoke = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = stoke, Alias = "Stoke City" };
      var ss_york = new TeamPlayerExternalSourceAlias { ExternalSource = skySports, TeamsPlayer = york, Alias = "York City" };

      Bookmakers = new Bookmaker[] { b10bet, b32red_bet, b888_sport, bbet_365, bbet_victor, bbet770, bbetdaq, bbetfair, bbetfred, bbetinternet, bbetvictor, bblue_square, bbodog, bboylesports, bbwin, bcoral, bcorbetts, bladbrokes, bmatchbook_com, bpaddy_power, bpanbet, bpinnacle_sports, bsky_bet, bsmarkets, bsporting_bet, bspreadex, bstan_james, btotesport, bwbx, bwilliam_hill, byouwin };
      Sports = new Sport[] { football, tennis };
      Competitions = new Competition[] { premierLeague, championship, leagueOne, leagueTwo, atp };
      Tournaments = new Tournament[] { t_premierLeague, t_championship, t_leagueOne, t_leagueTwo, brisbane_international, aircel_chennai_open, qatar_exxonmobil_open, apia_international_sydney, heineken_open, australian_open, open_sud_de_france, pbz_zagreb_indoors, vtr_open, abn_amro_world_tennis_tournament, brasil_open_2012, sap_open, regions_morgan_keegan_championships, copa_claro, open_13, dubai_duty_free_tennis_championships, delray_beach_international_tennis_championships, abierto_mexicano_telcel, bnp_paribas_open, sony_ericsson_open, grand_prix_hassan_ii, us_mens_clay_court_championship, monte_carlo_rolex_masters, brd_nastase_tiriac_trophy, barcelona_open_banc_sabadell, bmw_open, serbia_open_2012, estoril_open, mutua_madrid_open, internazionali_bnl_ditalia, open_de_nice_cote_dazur, roland_garros, gerry_weber_open, aegon_championships, unicef_open, aegon_international, wimbledon, mercedescup, campbells_hall_of_fame_tennis_championships, skistar_swedish_open, atp_studena_croatia_open, bet_at_home_open___german_tennis_championships_2012, atlanta_tennis_championships, credit_agricole_suisse_open_gstaad, bet_at_home_cup_kitzbuhel, farmers_classic, legg_mason_tennis_classic, rogers_cup, western__southern_open, winston_salem_open, us_open, moselle_open, st_petersburg_open, ptt_thailand_open, malaysian_open_kuala_lumpur, china_open, rakuten_japan_open_tennis_championships, shanghai_rolex_masters, erste_bank_open, if_stockholm_open, kremlin_cup, valencia_open_500, swiss_indoors_basel, bnp_paribas_masters };
      TournamentEvents = new TournamentEvent[] { s2012_t_premierLeague, s2012_t_championship, s2012_t_leagueOne, s2012_t_leagueTwo, s2012_brisbane_international, s2012_aircel_chennai_open, s2012_qatar_exxonmobil_open, s2012_apia_international_sydney, s2012_heineken_open, s2012_australian_open, s2012_open_sud_de_france, s2012_pbz_zagreb_indoors, s2012_vtr_open, s2012_abn_amro_world_tennis_tournament, s2012_brasil_open_2012, s2012_sap_open, s2012_regions_morgan_keegan_championships, s2012_copa_claro, s2012_open_13, s2012_dubai_duty_free_tennis_championships, s2012_delray_beach_international_tennis_championships, s2012_abierto_mexicano_telcel, s2012_bnp_paribas_open, s2012_sony_ericsson_open, s2012_grand_prix_hassan_ii, s2012_us_mens_clay_court_championship, s2012_monte_carlo_rolex_masters, s2012_brd_nastase_tiriac_trophy, s2012_barcelona_open_banc_sabadell, s2012_bmw_open, s2012_serbia_open_2012, s2012_estoril_open, s2012_mutua_madrid_open, s2012_internazionali_bnl_ditalia, s2012_open_de_nice_cote_dazur, s2012_roland_garros, s2012_gerry_weber_open, s2012_aegon_championships, s2012_unicef_open, s2012_aegon_international, s2012_wimbledon, s2012_mercedescup, s2012_campbells_hall_of_fame_tennis_championships, s2012_skistar_swedish_open, s2012_atp_studena_croatia_open, s2012_bet_at_home_open___german_tennis_championships_2012, s2012_atlanta_tennis_championships, s2012_credit_agricole_suisse_open_gstaad, s2012_bet_at_home_cup_kitzbuhel, s2012_farmers_classic, s2012_legg_mason_tennis_classic, s2012_rogers_cup, s2012_western__southern_open, s2012_winston_salem_open, s2012_us_open, s2012_moselle_open, s2012_st_petersburg_open, s2012_ptt_thailand_open, s2012_malaysian_open_kuala_lumpur, s2012_china_open, s2012_rakuten_japan_open_tennis_championships, s2012_shanghai_rolex_masters, s2012_erste_bank_open, s2012_if_stockholm_open, s2012_kremlin_cup, s2012_valencia_open_500, s2012_swiss_indoors_basel, s2012_bnp_paribas_masters };
      Funds = new Fund[] { premierFund, footballLeagueFund, tennisFund };
      ExternalSources = new ExternalSource[] { valueSamurai, skySports, bestBetting, oddsCheckerMobi, oddsCheckerWeb, tennisDataOdds, footballDataOdds, tb365, finkTank };
      MatchOutcomes = new MatchOutcome[] { teamOrPlayerAWin, draw, teamOrPlayerBWin };
      ScoreOutcomes = scoreOutcomes;
      TeamsPlayers = new TeamsPlayer[] { arsenal, astonvilla, birmingham, blackburn, blackpool, bolton, chelsea, everton, fulham, liverpool, mancity, manunited, newcastle, stoke, sunderland, tottenham, westbrom, westham, wigan, wolves, barnsley, bristolcity, burnley, cardiff, coventry, crystalpalace, derby, doncaster, hull, ipswich, leeds, leicester, middlesboro, millwall, norwich, nottmforest, portsmouth, preston, qpr, reading, scunthorpe, sheffieldunited, swansea, watford, bournemouth, brentford, brighton, bristolrvs, carlisle, charlton, colchester, dagandred, exeter, hartlepool, huddersfield, leytonorient, miltonkeynes, nottscounty, oldham, peterboro, plymouth, rochdale, sheffieldweds, southampton, swindon, tranmere, walsall, yeovil, accrington, aldershot, barnet, bradford, burton, bury, cheltenham, chesterfield, crewe, gillingham, hereford, lincolncity, macclesfield, morecambe, northampton, oxford, portvale, rotherham, shrewsbury, southend, stevenage, stockport, torquay, wycombe, afcwimbledon, crawleytown, fleetwoodtown, york };
      TeamPlayerExternalSourceAliass = new TeamPlayerExternalSourceAlias[] { ocw_manunited, ocw_wolves, ocw_middlesboro, ocw_nottmforest, ocw_sheffieldunited, ocw_bristolrvs, ocw_dagandred, ocw_miltonkeynes, ocw_nottscounty, ocw_peterboro, ocw_sheffieldweds, ocw_lincolncity, ocw_crawleytown, ocm_manunited, ocm_wolves, ocm_middlesboro, ocm_nottmforest, ocm_sheffieldunited, ocm_bristolrvs, ocm_dagandred, ocm_miltonkeynes, ocm_nottscounty, ocm_peterboro, ocm_sheffieldweds, ocm_lincolncity, ocm_crawleytown, bb_birmingham, bb_blackburn, bb_bolton, bb_mancity, bb_manunited, bb_newcastle, bb_stoke, bb_tottenham, bb_westbrom, bb_wigan, bb_wolves, bb_cardiff, bb_coventry, bb_derby, bb_doncaster, bb_hull, bb_ipswich, bb_leeds, bb_leicester, bb_middlesboro, bb_norwich, bb_nottmforest, bb_preston, bb_qpr, bb_scunthorpe, bb_swansea, bb_bournemouth, bb_brighton, bb_bristolrvs, bb_carlisle, bb_charlton, bb_colchester, bb_dagandred, bb_exeter, bb_hartlepool, bb_huddersfield, bb_miltonkeynes, bb_oldham, bb_peterboro, bb_plymouth, bb_sheffieldweds, bb_swindon, bb_tranmere, bb_yeovil, bb_accrington, bb_aldershot, bb_bradford, bb_burton, bb_cheltenham, bb_crewe, bb_hereford, bb_lincolncity, bb_macclesfield, bb_northampton, bb_oxford, bb_rotherham, bb_shrewsbury, bb_southend, bb_stockport, bb_torquay, bb_wycombe, bb_york, ss_astonvilla, ss_afcwimbledon, ss_bournemouth, ss_bristolcity, ss_bristolrvs, ss_burton, ss_crystalpalace, ss_chesterfield, ss_crawleytown, ss_dagandred, ss_fleetwoodtown, ss_huddersfield, ss_hull, ss_leytonorient, ss_manunited, ss_middlesboro, ss_miltonkeynes, ss_nottmforest, ss_northampton, ss_nottscounty, ss_oxford, ss_sheffieldunited, ss_sheffieldweds, ss_southampton, ss_stoke, ss_york };
    }

    public Bookmaker[] Bookmakers { get; set; }
    public Sport[] Sports { get; set; }
    public Competition[] Competitions { get; set; }
    public Tournament[] Tournaments { get; set; }
    public TournamentEvent[] TournamentEvents { get; set; }
    public Fund[] Funds { get; set; }
    public ExternalSource[] ExternalSources { get; set; }
    public MatchOutcome[] MatchOutcomes { get; set; }
    public ScoreOutcome[] ScoreOutcomes { get; set; }
    public TeamsPlayer[] TeamsPlayers { get; set; }
    public TeamPlayerExternalSourceAlias[] TeamPlayerExternalSourceAliass { get; set; }

  }
}
