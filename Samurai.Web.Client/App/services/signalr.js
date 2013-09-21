define(['services/datacontext', 'config'],
  function (datacontext, config) {
    var progress = ko.observable(),
        missingTournamentCouponURLs = ko.observable(),
        missingTeamPlayerAlias = ko.observable(),
        signalrOddsHub = undefined;

    return {
      progress: progress,
      missingTournamentCouponURLs: missingTournamentCouponURLs,
      missingTeamPlayerAlias: missingTeamPlayerAlias,
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
      signalrOddsHub.client.getMissingTournamentCouponURLs = getMissingTournamentCouponURLs;
      signalrOddsHub.client.getMissingTeamPlayerAlias = getMissingTeamPlayerAlias;
    }

    function reportProgress(appendText) {
      var progressSoFar = progress();
      progress(appendText + '\n' + progressSoFar);
    }

    function fetchTennisSchedules(dateString) {
      signalrOddsHub.server.fetchTennisSchedules(dateString)
      reportProgress('Sent request for tennis schedules');
    }

    function getMissingTournamentCouponURLs() {
      //do stuff
      reportProgress('This would return a list of missing tournament coupon URLs');
    }

    function getMissingTeamPlayerAlias() {
      //do stuff
      reportProgress('This would return a list of missing team or player alias');
    }

  });
  