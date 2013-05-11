define(['services/datacontext'], function (datacontext) {
  var tennisSchedules = ko.observableArray();
  var initialised = false;
  var vm = {
    activate: activate,
    tennisSchedules: tennisSchedules,
    title: 'Tennis',
    refresh: refresh
  };
  return vm;

  function activate() {
    if (initialised) { return; }
    initialised = true;
    return refresh();
  }

  function refresh() {
    return datacontext.getTennisSchedule(tennisSchedules);
  }
   
});