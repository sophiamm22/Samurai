define(['config'],
  function (config) {
    var progress = ko.observable(),
        signalrOddsHub = undefined;

    return {
      progress: progress,
      connectHubs: connectHubs,
      reportProgress: reportProgress,
      fetchTennisSchedules: fetchTennisSchedules
    };

    function connectHubs() {
      signalrOddsHub = $.connection.oddsHub;
      $.connection.hub.url = config.signalrServiceName;
      $.connection.hub
        .start()
        .done(function () { progress('Connected to hub!'); })
        .fail(function () { progress('Failed to connect to hub!'); });

      initFunctions();
    }

    function initFunctions () {
      signalrOddsHub.client.reportProgress = reportProgress;
      
    }

    function reportProgress(appendText) {
      var progressSoFar = progress();
      progress(progressSoFar + '\n' + appendText);
    }

    function fetchTennisSchedules(dateString) {
      reportProgress(signalrOddsHub.server.fetchTennisSchedules(dateString));
    }


  });
  