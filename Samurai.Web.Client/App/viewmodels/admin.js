define(['services/datacontext', 'services/signalr'],
  function (datacontext, signalr) {
    var scheduleDateString = ko.observable(moment().format('YYYY-MM-DD')),
        sports = ko.observableArray(['tennis', 'football']),
        sport = ko.observable('tennis'),
        fixtureOptions = ko.observableArray(['fixtures', 'results']),
        fixtureOption = ko.observable('fixtures'),
        showProgress = ko.observable(false),
        progressReport = ko.observable(''),
        initialised = false,
        updateFixturesResultsOdds = function () {

        },
        activate = function () {
          if (!initialised) {
            signalr.connectHubs(progressReport);
            initialised = true;
          }
          return Q.resolve();
        };
    var vm = {
      title: 'Admin',
      scheduleDateString: scheduleDateString,
      sports: sports,
      sport: sport,
      fixtureOptions: fixtureOptions,
      fixtureOption: fixtureOption,
      showProgress: showProgress,
      progressReport: progressReport,
      updateFixturesResultsOdds: updateFixturesResultsOdds,
      activate: activate,
      showProgress: showProgress
    };
    return vm;
  });