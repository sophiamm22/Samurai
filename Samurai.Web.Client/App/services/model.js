﻿define(['config'],
  function (config) {
    var DT = breeze.DataType;
    var AutoGeneratedKeyType = breeze.AutoGeneratedKeyType;
    var nulloDate = new Date(1900, 0, 1);

    var model = {
      configureMetadataStore: configureMetadataStore
    };

    return model;

    function configureMetadataStore(metadataStore) {
      entityTypeInitialiser(metadataStore);
      registerEntityTypeConstructors(metadataStore);
    }

    //#region Private variables
    function entityTypeInitialiser(metadataStore) {
      metadataStore.addEntityType({
        shortName: 'FootballMatch',
        namespace: 'Samurai',
        dataProperties: {
          matchIdentifier: { dataType: DT.String },
          id: { dataType: DT.Int64, isPartOfKey: true },
          league: { dataType: DT.String },
          season: { dataType: DT.String },
          matchDate: { dataType: DT.DateTime },
          homeTeam: { dataType: DT.String },
          awayTeam: { dataType: DT.String },
          scoreLine: { dataType: DT.String },
          iktsGameWeek: { dataType: DT.Int64 },
        },
        navigationProperties: {
          predictions: {
            entityTypeName: 'FootballPrediction',
            isScalar: true,
            associationName: 'FootballMatch_FootballPrediction'
          },
          odds: {
            entityTypeName: 'FootballOdd',
            isScalar: false,
            associationName: 'FootballMatch_FootballOdd'
          }
        }
      });

      metadataStore.addEntityType({
        shortName: 'FootballPrediction',
        namespace: 'Samurai',
        dataProperties: {
          matchId: { dataType: DT.Int32, isPartOfKey: true },
          matchIdentifier: { dataType: DT.String },
          predictionURL: { dataType: DT.String },
          probabilities: { dataType: DT.Undefined }
        },
        navigationProperties: {
          footballMatch: {
            entityTypeName: 'FootballMatch', isScalar: true,
            associationName: 'FootballMatch_FootballPrediction',
            foreignKeyNames: ['matchId']
          }
        }
      });

      metadataStore.addEntityType({
        shortName: 'FootballOdd',
        namespace: 'Samurai',
        dataProperties: {
          matchId: { dataType: DT.Int32, isPartOfKey: true },
          sport: { dataType: DT.String },
          outcome: { dataType: DT.String, isPartOfKey: true },
          oddsBeforeCommission: { dataType: DT.Decimal },
          commissionPct: { dataType: DT.Decimal },
          decimalOdd: { dataType: DT.Decimal },
          timeStamp: { dataType: DT.DateTime },
          bookmaker: { dataType: DT.String },
          oddsSource: { dataType: DT.String },
          clickThroughURL: { dataType: DT.String },
          priority: { dataType: DT.Int32 }
        },
        navigationProperties: {
          footballMatch: {
            entityTypeName: 'FootballMatch', isScalar: true,
            associationName: 'FootballMatch_FootballOdd',
            foreignKeyNames: ['matchId']
          }
        }
      });

      metadataStore.addEntityType({
        shortName: 'TennisMatch',
        namespace: 'Samurai',
        dataProperties: {
          matchIdentifier: { dataType: DT.String },
          id: { dataType: DT.Int64, isPartOfKey: true },
          tournament: { dataType: DT.String },
          year: { dataType: DT.String },
          matchDate: { dataType: DT.DateTime },

          playerAFirstName: { dataType: DT.String },
          playerASurname: { dataType: DT.String },
          playerBFirstName: { dataType: DT.String },
          playerBSurname: { dataType: DT.String },

          scoreLine: { dataType: DT.String },
        },
        navigationProperties: {
          predictions: {
            entityTypeName: 'TennisPrediction',
            isScalar: true,
            associationName: 'TennisMatch_TennisPrediction'
          },
          odds: {
            entityTypeName: 'TennisOdd',
            isScalar: false,
            associationName: 'TennisMatch_TennisOdd'
          }
        }
      });

      metadataStore.addEntityType({
        shortName: 'TennisPrediction',
        namespace: 'Samurai',
        dataProperties: {
          matchId: { dataType: DT.Int32, isPartOfKey: true },
          sport: { dataType: DT.String },
          matchIdentifier: { dataType: DT.String },
          predictionURL: { dataType: DT.String },
          playerAGames: { dataType: DT.Double },
          playerBGames: { dataType: DT.Double },
          ePoints: { dataType: DT.Double },
          eGames: { dataType: DT.Double },
          eSets: { dataType: DT.Double },
          probabilities: { dataType: DT.Undefined },
          scoreLineProbabilties: { dataType: DT.Undefined }
        },
        navigationProperties: {
          tennisMatch: {
            entityTypeName: 'TennisMatch', isScalar: true,
            associationName: 'TennisMatch_TennisPrediction',
            foreignKeyNames: ['matchId']
          }
        }
      });

      metadataStore.addEntityType({
        shortName: 'TennisOdd',
        namespace: 'Samurai',
        dataProperties: {
          matchId: { dataType: DT.Int32, isPartOfKey: true },
          sport: { dataType: DT.String },
          outcome: { dataType: DT.String, isPartOfKey: true },
          oddsBeforeCommission: { dataType: DT.Decimal },
          commissionPct: { dataType: DT.Decimal },
          decimalOdd: { dataType: DT.Decimal },
          timeStamp: { dataType: DT.DateTime },
          bookmaker: { dataType: DT.String },
          oddsSource: { dataType: DT.String },
          clickThroughURL: { dataType: DT.String },
          priority: { dataType: DT.Int32 }
        },
        navigationProperties: {
          tennisMatch: {
            entityTypeName: 'TennisMatch', isScalar: true,
            associationName: 'TennisMatch_TennisOdd',
            foreignKeyNames: ['matchId']
          }
        }
      });

    }

    function registerEntityTypeConstructors(metadataStore) {

      metadataStore.registerEntityTypeCtor('TennisMatch', null, tennisMatchInitialiser);
      metadataStore.registerEntityTypeCtor('FootballMatch', null, footballMatchIntitialiser);
      metadataStore.registerEntityTypeCtor('TennisOdds', null, tennisOddsIntialiser);
      metadataStore.registerEntityTypeCtor('FootballOdds', null, footballOddsIntialiser);
    }

    function footballMatchIntitialiser(footballMatch) {

      footballMatch.sport = ko.computed(function () {
        return 'Football'; //temporary.  needs to come from the API IMHO
      });

      footballMatch.tournament = ko.computed(function () {
        //so that we can compare both tennis and football with the same functions
        var league = footballMatch.league();
        return league;
      });

      footballMatch.matchTime = ko.computed(function () {
        var start = footballMatch.matchDate();
        var value = ((start - nulloDate) === 0) ?
                        ' --- ' :
                        (start && moment.utc(start).isValid()) ?
                            moment.utc(start).format('DD-MM-YY HH:mm') :
                            ' --- ';
        return value;
      });

      footballMatch.homeWinProbDbl = ko.computed(function () {
        var probs = footballMatch.predictions().probabilities();
        return probs ? probs['homeWin'] : 0.0;
      });

      footballMatch.drawProbDbl = ko.computed(function () {
        var probs = footballMatch.predictions().probabilities();
        return probs ? probs['draw'] : 0.0;
      });

      footballMatch.awayWinProbDbl = ko.computed(function () {
        var probs = footballMatch.predictions().probabilities();
        return probs ? probs['awayWin'] : 0.0;
      });

      footballMatch.homeWinProb = ko.computed(function () {
        var probs = footballMatch.predictions().probabilities();
        return probs && ((100 * probs['homeWin']).toFixed(0) + '%');
      });

      footballMatch.drawProb = ko.computed(function () {
        var probs = footballMatch.predictions().probabilities();
        return probs && ((100 * probs['draw']).toFixed(0) + '%');
      });

      footballMatch.awayWinProb = ko.computed(function () {
        var probs = footballMatch.predictions().probabilities();
        return probs && ((100 * probs['awayWin']).toFixed(0) + '%');
      });

      footballMatch.homeWinOddsDbl = ko.computed(function () {
        var odds = footballMatch.odds();
        return odds.length == 0 ? 1.01 : odds[0].decimalOdd();
      });

      footballMatch.drawOddsDbl = ko.computed(function () {
        var odds = footballMatch.odds();
        return odds.length == 0 ? 1.01 : odds[1].decimalOdd();
      });

      footballMatch.awayWinOddsDbl = ko.computed(function () {
        var odds = footballMatch.odds();
        return odds.length == 0 ? 1.01 : odds[2].decimalOdd();
      });

      footballMatch.homeWinOdds = ko.computed(function () {
        var odds = footballMatch.odds();
        return odds.length == 0 ? '-' : (odds[0].decimalOdd()).toFixed(2);
      });

      footballMatch.drawOdds = ko.computed(function () {
        var odds = footballMatch.odds();
        return odds.length == 0 ? '-' : (odds[1].decimalOdd()).toFixed(2);
      });

      footballMatch.awayWinOdds = ko.computed(function () {
        var odds = footballMatch.odds();
        return odds.length == 0 ? '-' : (odds[2].decimalOdd()).toFixed(2);
      });

      footballMatch.homeWinEdge = ko.computed(function () {
        return footballMatch.homeWinOddsDbl() * footballMatch.homeWinProbDbl() - 1;
      });

      footballMatch.drawEdge = ko.computed(function () {
        return footballMatch.drawOddsDbl() * footballMatch.drawProbDbl() - 1;
      });

      footballMatch.awayWinEdge = ko.computed(function () {
        return footballMatch.awayWinOddsDbl() * footballMatch.awayWinProbDbl() - 1;
      });

      footballMatch.homeWinClickThrough = ko.computed(function () {
        var odds = footballMatch.odds();
        return odds.length == 0 ? '/' : odds[0].clickThroughURL();
      });

      footballMatch.drawClickThrough = ko.computed(function () {
        var odds = footballMatch.odds();
        return odds.length == 0 ? '/' : odds[1].clickThroughURL();
      });

      footballMatch.awayWinClickThrough = ko.computed(function () {
        var odds = footballMatch.odds();
        return odds.length == 0 ? '/' : odds[2].clickThroughURL();
      });

      footballMatch.homeWinClickThroughLocation = ko.computed(function () {
        var odds = footballMatch.odds();
        return odds.length == 0 ? 'No odds' : (odds[0].bookmaker() + ' (' + odds[0].oddsSource() + ')');
      });

      footballMatch.drawClickThroughLocation = ko.computed(function () {
        var odds = footballMatch.odds();
        return odds.length == 0 ? 'No odds' : (odds[1].bookmaker() + ' (' + odds[1].oddsSource() + ')');
      });

      footballMatch.awayWinClickThroughLocation = ko.computed(function () {
        var odds = footballMatch.odds();
        return odds.length == 0 ? 'No odds' : (odds[2].bookmaker() + ' (' + odds[2].oddsSource() + ')');
      });

      footballMatch.hasQualifyingBet = ko.computed(function () {
        var bestBet = _.max([footballMatch.homeWinEdge(), footballMatch.drawEdge(), footballMatch.awayWinEdge()]);
        return bestBet > 0.1; //need to have this stored somewhere
      });

      footballMatch.valueOdd = ko.computed(function () {
        if (footballMatch.hasQualifyingBet()) {
          return footballMatch.homeWinOddsDbl();
        }
      });

      footballMatch.valuePrediction = ko.computed(function () {
        if (footballMatch.hasQualifyingBet()) {
          return footballMatch.homeWinProbDbl();
        }
      });

      footballMatch.valueOdd = ko.computed(function () {
        if (footballMatch.hasQualifyingBet()) {
          var bestBet = _.max([footballMatch.homeWinEdge(), footballMatch.drawEdge(), footballMatch.awayWinEdge()]);

          if (bestBet === footballMatch.homeWinEdge()) { return footballMatch.homeWinOddsDbl(); }
          if (bestBet === footballMatch.drawEdge()) { return footballMatch.drawOddsDbl(); }
          if (bestBet === footballMatch.awayWinEdge()) { return footballMatch.awayWinOddsDbl(); }
          return 1.01;
        }
      });

      footballMatch.valuePrediction = ko.computed(function () {
        if (footballMatch.hasQualifyingBet()) {
          var bestBet = _.max([footballMatch.homeWinEdge(), footballMatch.awayWinEdge()]);

          if (bestBet === footballMatch.homeWinEdge()) { return footballMatch.homeWinProbDbl(); }
          if (bestBet === footballMatch.DrawEdge()) { return footballMatch.drawProbDbl(); }
          if (bestBet === footballMatch.awayWinEdge()) { return footballMatch.homeWinProbDbl(); }
          return 0.0;
        }
      });
    }

    function footballOddsIntialiser(footballOdds) {

    }

    function tennisMatchInitialiser(tennisMatch) {

      tennisMatch.sport = ko.computed(function () {
        return 'Tennis'; //temporary.  needs to come from the API IMHO
      });

      tennisMatch.hasOdds = ko.computed(function () {
        if (tennisMatch.hasOwnProperty('odds') && tennisMatch.odds()) {
          var odds = tennisMatch.odds();

        }
      });

      tennisMatch.homeTeam = ko.computed(function () {
        return tennisMatch.playerASurname() + ', ' + tennisMatch.playerAFirstName();
      });

      tennisMatch.awayTeam = ko.computed(function () {
        return tennisMatch.playerBSurname() + ', ' + tennisMatch.playerBFirstName();
      });

      tennisMatch.matchTime = ko.computed(function () {
        var start = tennisMatch.matchDate();
        var value = ((start - nulloDate) === 0) ?
                        ' --- ' :
                        (start && moment.utc(start).isValid()) ?
                            moment.utc(start).format('DD-MM-YY HH:mm') :
                            ' --- ';
        return value;                      
      });

      tennisMatch.playerAWinProb = ko.computed(function () {
        var probs = tennisMatch.predictions().probabilities();
        return probs && ((100 * probs['homeWin']).toFixed(0) + '%');
      });

      tennisMatch.playerBWinProb = ko.computed(function () {
        var probs = tennisMatch.predictions().probabilities();
        return probs && ((100 * probs['awayWin']).toFixed(0) + '%');
      });

      tennisMatch.playerAWinProbDbl = ko.computed(function () {
        var probs = tennisMatch.predictions().probabilities();
        return probs ? probs['homeWin'] : 0.0;
      });

      tennisMatch.playerBWinProbDbl = ko.computed(function () {
        var probs = tennisMatch.predictions().probabilities();
        return probs ? probs['awayWin'] : 0.0;
      });

      tennisMatch.homeWinOddsDbl = ko.computed(function () {
        var odds = tennisMatch.odds();
        return odds.length == 0 ? 1.01 : odds[0].decimalOdd();
      });

      tennisMatch.awayWinOddsDbl = ko.computed(function () {
        var odds = tennisMatch.odds();
        return odds.length == 0 ? 1.01 : odds[1].decimalOdd();
      });

      tennisMatch.homeWinOdds = ko.computed(function () {
        var odds = tennisMatch.odds();
        return odds.length == 0 ? '-' : (odds[0].decimalOdd()).toFixed(2);
      });

      tennisMatch.awayWinOdds = ko.computed(function () {
        var odds = tennisMatch.odds();
        return odds.length == 0 ? '-' : (odds[1].decimalOdd()).toFixed(2);
      });

      tennisMatch.homeWinEdge = ko.computed(function () {
        return tennisMatch.homeWinOddsDbl() * tennisMatch.playerAWinProbDbl() - 1;
      });

      tennisMatch.awayWinEdge = ko.computed(function () {
        return tennisMatch.awayWinOddsDbl() * tennisMatch.playerBWinProbDbl() - 1;
      });

      tennisMatch.homeWinClickThrough = ko.computed(function () {
        var odds = tennisMatch.odds();
        return odds.length == 0 ? '/' : odds[0].clickThroughURL();
      });

      tennisMatch.awayWinClickThrough = ko.computed(function () {
        var odds = tennisMatch.odds();
        return odds.length == 0 ? '/' : odds[1].clickThroughURL();
      });

      tennisMatch.homeWinClickThroughLocation = ko.computed(function () {
        var odds = tennisMatch.odds();
        return odds.length == 0 ? 'No odds' : (odds[0].bookmaker() + ' (' + odds[0].oddsSource() + ')');
      });

      tennisMatch.awayWinClickThroughLocation = ko.computed(function () {
        var odds = tennisMatch.odds();
        return odds.length == 0 ? 'No odds' : (odds[1].bookmaker() + ' (' + odds[1].oddsSource() + ')');
      });

      tennisMatch.hasQualifyingBet = ko.computed(function () {
        if (tennisMatch.predictions().playerAGames() < 50 || tennisMatch.predictions().playerBGames() < 50) { return false; }
        var bestBet = _.max([tennisMatch.homeWinEdge(), tennisMatch.awayWinEdge()]);
        return bestBet > 0.1; //need to have this stored somewhere      
      });

      tennisMatch.valueOdd = ko.computed(function () {
        if (tennisMatch.hasQualifyingBet()) {
          var bestBet = _.max([tennisMatch.homeWinEdge(), tennisMatch.awayWinEdge()]);

          if (bestBet === tennisMatch.homeWinEdge()) { return tennisMatch.homeWinOddsDbl(); }
          if (bestBet === tennisMatch.awayWinEdge()) { return tennisMatch.awayWinOddsDbl(); }
          return 1.01;
        }
      });

      tennisMatch.valuePrediction = ko.computed(function () {
        if (tennisMatch.hasQualifyingBet()) {
          var bestBet = _.max([tennisMatch.homeWinEdge(), tennisMatch.awayWinEdge()]);

          if (bestBet === tennisMatch.homeWinEdge()) { return tennisMatch.playerAWinProbDbl(); }
          if (bestBet === tennisMatch.awayWinEdge()) { return tennisMatch.playerBWinProbDbl(); }
          return 0.00;
        }
      });
    }

    function tennisOddsIntialiser(tennisOdds) {




    }


    //#endregion

  });