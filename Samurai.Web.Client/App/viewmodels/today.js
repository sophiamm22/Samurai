define(['services/datacontext', 'services/groupSchedule', 'durandal/plugins/router'], function (datacontext, groupSchedule, router) {

  var todaysFootballSchedules = ko.observableArray(),
      todaysTennisSchedules = ko.observableArray(),
      todaysMatchesCount = ko.computed(function () {
        return todaysFootballSchedules().length + todaysTennisSchedules().length;
      }),
      competitions = ko.computed(function () {
        return groupSchedule.schedulesToHeadings(todaysFootballSchedules(), todaysTennisSchedules());
      });
  
  var initialised = false;
  var vm = {
    title: 'Todays fixtures',
    activate: activate,
    todaysFootballSchedules: todaysFootballSchedules,
    todaysTennisSchedules: todaysTennisSchedules,
    todaysMatchesCount: todaysMatchesCount,
    competitions: competitions,
    refresh: refresh
  };
  return vm;

  function activate() {
    
    if (initialised) { return; }
    initialised = true;
    return refresh();
  }

  function refresh() {
    return datacontext.getTodaysFootballSchedule(todaysFootballSchedules)
                      .then(datacontext.getTodaysTennisSchedule(todaysTennisSchedules));
  }

});