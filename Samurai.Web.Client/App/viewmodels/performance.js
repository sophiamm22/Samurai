define(['services/logger'], function (logger) {
  var vm = {
    activate: activate,
    title: 'Performance'
  };

  return vm;

  //#region Internal Methods
  function activate() {
    logger.log('Performance View Activated', null, 'performance', true);
    return true;
  }
  //#endregion
});