define(
  new breeze.JsonResultsAdapter({
    name: "samurai",

    extractResults: function (data) {
      var results = data.results;
      if (!results) throw new Error('Unable to resolve "results" property');

      return results || results.predictions;
    },

    visitNode: function (node, parseContext, nodeContext) {
      if (node.matchIdentifier && node.predictions) {
        node.predictions = [];
        return { entityType: 'FootballMatch' }
      }

      else if (node.matchIdentifier && nodel.predictionURL) {
        node.footballMatch = null;

        return { entityType: 'FootballPredictions' }
      }
    }
  }));