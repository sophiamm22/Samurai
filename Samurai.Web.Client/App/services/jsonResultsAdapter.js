﻿﻿define(
  new breeze.JsonResultsAdapter({
    name: "samurai",

    extractResults: function (data) {
      var results = data.results;
      if (!results) throw new Error('Unable to resolve "results" property');

      return results;// && results.predictions;
    },

    visitNode: function (node, parseContext, nodeContext) {
      if (node.matchIdentifier && node.homeTeam && node.awayTeam) {
        //node.predictions = {};
        return { entityType: 'FootballMatch' };
      }
      else if (node.probabilities && node.playerAGames && node.playerBGames) {
        return { entityType: 'TennisPrediction' };
      }
      else if (node.probabilities) {
        return { entityType: 'FootballPrediction' };
      }
      else if (node.matchIdentifier && node.playerAFirstName && node.playerBFirstName) {
        //node.predictions = {};
        return { entityType: 'TennisMatch' };
      }
    }
  }));