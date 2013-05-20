define(['services/logger', 'durandal/system', 'services/model', 'config', 'services/jsonResultsAdapter'],
  function (logger, system, model, config, jsonResultsAdapter) {

    var EntityQuery = breeze.EntityQuery,
        manager = configureBreezeManager();

    var getTodaysFootballSchedule = function (footballScheduleObservable) {
      var query = EntityQuery.from('fixtures/todays-football-schedule');

      return manager.executeQuery(query)
        .then(querySucceeded)
        .fail(queryFailed);
      
      function querySucceeded(data) {
        if (footballScheduleObservable) {
          footballScheduleObservable(data.results);
        }
        log('Retrieved todays football schedule from remote data source',
          data, true);
      }
    };

    var getTodaysLatestFootballOdds = function (footballOddsObservable) {
      //footballScheduleObservable.isLoading = true;

      var query = EntityQuery.from('odds/todays-football-odds');

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

    var getTodaysTennisSchedule = function (tennisScheduleObservable) {
      var query = EntityQuery.from('fixtures/todays-tennis-schedule');

      return manager.executeQuery(query)
        .then(querySucceeded)
        .fail(queryFailed);

      function querySucceeded(data) {
        if (tennisScheduleObservable) {
          tennisScheduleObservable(data.results);
        }
        log('Retrieved todays tennis schedule from remote data source',
          data, true);
      }
    };

    var getTodaysLatestTennisOdds = function (tennisOddsObservable) {
      var query = EntityQuery.from('odds/todays-tennis-odds');

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
    function queryFailed(error) {
      var msg = 'Error retreiving data. ' + error.message;
      logger.log(msg,
        error,
        system.getModuleId(datacontext),
        true);
    }

    function configureBreezeManager() {
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
