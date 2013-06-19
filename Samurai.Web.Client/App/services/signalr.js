define(['config'],
  function (config) {
    var progress,
        signalrOddsHub = $.connection.oddsHub;

    return {
      connectHubs: connectHubs
    }

    function connectHubs(progressObservable) {
      $.connection.hub.url = config.signalrServiceName;
      $.connection.hub.start().done(function () {
        progress = progressObservable;
      });
     
      signalrOddsHub.client.reportProgress = function (appendText) {
        var progressSoFar = progress();
        progress(progressSoFar + '\n' + appendText);
      }
    }
  });
  