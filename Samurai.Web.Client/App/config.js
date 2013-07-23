﻿define(function () {
  toastr.options.timeOut = 4000;
  toastr.options.positionClass = 'toaster-bottom-right';

  var routes = [{
    url: 'today',
    moduleId: 'viewmodels/today',
    name: 'Today',
    visible: true,
    caption: 'Today'
  }, {
    url: 'performance',
    moduleId: 'viewmodels/performance',
    name: 'Performance',
    visible: true
  }, {
    url: 'admin',
    moduleId: 'viewmodels/admin',
    name: 'Admin',
    visible: true,
    settings: { admin: true }
  }, {
    url: 'today/competition/:competition',
    moduleId: 'viewmodels/today',
    name: 'Today'
  }, {
    url: 'today/sport/:sport',
    moduleId: 'viewmodels/today',
    name: 'Today'
  }];

  var startModule = 'today';

  var remoteServiceName = 'http://localhost:3600/api';

  var signalrServiceName = 'http://localhost:3600/signalr';

  var hashes = {
    today: '#/today',
    todayByCompetition: '#/today/competition',
    todayBySport: '#/today/sport'
  };

  var throttle = 400;

  var minTennisGames = 70;
  var minEdgeTennis = 0.2;
  var kellyMultiplier = 0.25;
  var exhaustiveKellyLimit = 7;
  var startBank = 1000;

  return {
    routes: routes,
    startModule: startModule,
    remoteServiceName: remoteServiceName,
    hashes: hashes,
    throttle: throttle,
    signalrServiceName: signalrServiceName,
    minTennisGames: minTennisGames,
    minEdgeTennis: minEdgeTennis,
    kellyMultiplier: kellyMultiplier,
    exhaustiveKellyLimit: exhaustiveKellyLimit,
    startBank: startBank
  };

});
