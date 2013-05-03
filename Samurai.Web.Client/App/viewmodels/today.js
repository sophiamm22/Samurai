define(['services/datacontext'], function (datacontext) {

  var todaysFootballSchedules = ko.observableArray(),
      todaysMatchesCount = ko.computed(function () {
        return todaysFootballSchedules().length;
      });


  var initialised = false;
  var vm = {
    title: 'Todays fixtures',
    activate: activate,
    todaysFootballSchedules: todaysFootballSchedules,
    todaysMatchesCount: todaysMatchesCount,
    refresh: refresh
  };
  return vm;

  function activate() {
    if (initialised) { return; }
    initialised = true;
    return refresh();
  }

  function refresh() {
    return datacontext.getTodaysFootballSchedule(todaysFootballSchedules);
  }

});