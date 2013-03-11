define(['services/logger'], function (logger) {
  var vm = {
    activate: activate,
    title: 'Football'
  };

  return vm;

  //#region Internal Methods
  function activate() {
    logger.log('Football View Activated', null, 'football', true);
    return true;
  }
  //#endregion
});