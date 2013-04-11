define(['services/logger', 'durandal/system', 'services/model', 'config'],
  function (logger, system, model, config) {

    var EntityQuery = breeze.EntityQuery,
        manager = configureBreezeManager();

    var getTennisSchedule = function (tennisScheduleObservable) {
      var query = EntityQuery.from('TennisSchedules')
        .orderBy('matchDate');

      return manager.executeQuery(query)
        .then(querySucceeded)
        .fail(queryFailed);
      
      function querySucceeded(data) {
        if (tennisScheduleObservable) {
          tennisScheduleObservable(data.results);
        }
        log('Retrieved tennis schedules from remote data source',
          data, true);
      }
    };

    var primeData = function () {
      return Q.all([getTennisSchedule()]);
    };

    var datacontext = {
      getTennisSchedule: getTennisSchedule,
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
      breeze.NamingConvention.camelCase.setAsDefault();
      var mgr = new breeze.EntityManager(config.remoteServiceName);
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
