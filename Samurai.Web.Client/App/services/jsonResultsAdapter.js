define('services/jsonResultsAdapter',

  new breeze.JsonResultsAdapter({
    name: "samurai",

    extractResults: function (data) {
      var results = data.results;
      if (!results) throw new Error('Unable to resolve "results" property');

      return results;
    },

    visitNode: function (node, parseContext, nodeContext) {
      if (node.hasOwnProperty('matchIdentifier') && node.hasOwnProperty('homeTeam') && node.hasOwnProperty('awayTeam')) {
        return { entityType: 'FootballMatch' };
      }
      else if (node.hasOwnProperty('probabilities') && node.hasOwnProperty('playerAGames') && node.hasOwnProperty('playerBGames')) {
        return { entityType: 'TennisPrediction' };
      }
      else if (node.hasOwnProperty('probabilities')) {
        return { entityType: 'FootballPrediction' };
      }
      else if (node.matchIdentifier && node.hasOwnProperty('playerAFirstName') && node.hasOwnProperty('playerBFirstName')) {
        return { entityType: 'TennisMatch' };
      }
      else if (node.hasOwnProperty('decimalOdd') && node.sport == 'Football') {
        return { entityType: 'FootballOdd' };
      }
      else if (node.hasOwnProperty('decimalOdd') && node.sport == 'Tennis') {
        return { entityType: 'TennisOdd' };
      }
      //else if (node.hasOwnProperty('decimalOdd') && node.sport == 'Football'){
      //  return { entityType: 'FootballOdd' };
      //}
      //else if (node.hasOwnProperty('decimalOdd') && node.sport == 'Tennis') {
      //  return { entityType: 'TennisOdd' };
      //}
    }
  })

);