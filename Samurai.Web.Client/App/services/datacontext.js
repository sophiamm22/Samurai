define(['services/logger', 'durandal/system', 'services/model', 'config'],
  function (logger, system, model, config) {

    var EntityQuery = breeze.EntityQuery,
        manager = configureBreezeManager();

    var getTodaysFootballSchedule = function (footballScheduleObservable, options) {

      var query = EntityQuery
        .from('fixtures/todays-football-schedule')
        .toType('FootballMatch');

      if (!options.forceRefresh) {
        var f = manager.executeQueryLocally(query);
        if (f.length > 0) {
          filterResults(footballScheduleObservable, f, options.filter);
          //footballScheduleObservable(s);
          return Q.resolve();
        }
      }

      return manager.executeQuery(query)
        .then(querySucceeded)
        .fail(queryFailed);
      
      function querySucceeded(data) {
        if (footballScheduleObservable) {
          filterResults(footballScheduleObservable, data.results, options.filter);
          //footballScheduleObservable(data.results);
        }
        log('Retrieved todays football schedule from remote data source',
          data, true);
      }
    };

    var getTodaysLatestFootballOdds = function (footballOddsObservable, options) {

      var query = EntityQuery
        .from('odds/todays-football-odds')
        .toType('FootballOdd');

      if (!options.forceRefresh) {
        var o = manager.executeQueryLocally(query);
        if (o.length > 0) {
          footballOddsObservable(o);
          return Q.resolve();
        }
      }

      return manager.executeQuery(query)
        .then(querySucceeded)
        .fail(queryFailed);

      function querySucceeded(data) {
        if (footballOddsObservable) {
          footballOddsObservable(data.results);
        }
        log('Retrieved todays football odds from remote data source',
          data, true);
      }
    };

    var getTodaysTennisSchedule = function (tennisScheduleObservable, options) {

      var query = EntityQuery
        .from('fixtures/todays-tennis-schedule')
        .toType('TennisMatch');

      if (!options.forceRefresh) {
        var t = manager.executeQueryLocally(query);
        if (t.length > 0) {
          filterResults(tennisScheduleObservable, t, options.filter);
          //tennisScheduleObservable(t);
          return Q.resolve();
        }
      }

      return manager.executeQuery(query)
        .then(querySucceeded)
        .fail(queryFailed);

      function querySucceeded(data) {
        if (tennisScheduleObservable) {
          filterResults(tennisScheduleObservable, data.results, options.filter);
          //tennisScheduleObservable(data.results);
        }
        log('Retrieved todays tennis schedule from remote data source',
          data, true);
      }
    };

    var getTodaysLatestTennisOdds = function (tennisOddsObservable, options) {

      var query = EntityQuery
        .from('odds/todays-tennis-odds')
        .toType('TennisOdd');

      if (!options.forceRefresh) {
        var o = manager.executeQueryLocally(query);
        if (o.length > 0) {
          tennisOddsObservable(o);
          return Q.resolve();
        }
      }

      return manager.executeQuery(query)
        .then(querySucceeded)
        .fail(queryFailed);

      function querySucceeded(data) {
        if (tennisOddsObservable) {
          tennisOddsObservable(data.results);
        }
        log('Retrieved todays tennis odds from remote data source',
          data, true);
      }
    };

    var primeData = function () {
      return Q.all([]);
    };

    var datacontext = {
      getTodaysFootballSchedule: getTodaysFootballSchedule,
      getTodaysLatestFootballOdds: getTodaysLatestFootballOdds,
      getTodaysTennisSchedule: getTodaysTennisSchedule,
      getTodaysLatestTennisOdds: getTodaysLatestTennisOdds,
      primeData: primeData
    };

    return datacontext;

    //#region Internal methods
    function getLocal(resource){
      var query = EntityQuery.from(resource);
      return manager.executeQueryLocally(query);
    }

    function filterResults(observable, list, filter) {
      var filteredList = _.filter(list, function (item) {
        var match = filter.predicate(filter, item);
        return match;
      });
      observable(filteredList);
    }


    function queryFailed(error) {
      var msg = 'Error retreiving data. ' + error.message;
      logger.log(msg,
        error,
        system.getModuleId(datacontext),
        true);
    }

    function configureBreezeManager() {

      var jsonResultsAdapter = new breeze.JsonResultsAdapter({
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
      });


      var dataService = new breeze.DataService({
        serviceName: config.remoteServiceName,
        hasServerMetadata: false,
        jsonResultsAdapter: jsonResultsAdapter
      });

      var mgr = new breeze.EntityManager({ dataService: dataService });
      model.configureMetadataStore(mgr.metadataStore);
      return mgr;
    }

    function log(msg, data, showToast) {
      logger.log(
        msg,
        data,
        system.getModuleId(datacontext),
        showToast);
    }
    //#endregion
  });
