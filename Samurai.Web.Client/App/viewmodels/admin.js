define(['services/signalr'],
  function (signalr) {
    var scheduleDateString = ko.observable(moment().format('YYYY-MM-DD')),
        sports = ko.observableArray(['tennis', 'football']),
        sport = ko.observable('tennis'),
        fixtureOptions = ko.observableArray(['fixtures', 'results']),
        fixtureOption = ko.observable('fixtures'),
        showProgress = ko.observable(false),
        progressReport = ko.observable(''),
        initialised = false,
        updateFixturesResultsOdds = function () {
          signalr.fetchTennisSchedules(scheduleDateString());
          showProgress(true);
        },
        activate = function () {
          if (!initialised) {
            signalr.connectHubs();
            initialised = true;
          }
          addSignalRSubscriptions();
          return Q.resolve();
        },
        addSignalRSubscriptions = function () {
          signalr.progress.subscribe(function (newValue) {
            progressReport(newValue);
          });
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
      activate: activate
    };
    return vm;
  });