﻿define(function () {
  toastr.options.timeOut = 4000;
  toastr.options.positionClass = 'toast-bottom-right';

  var routes = [{
    url: 'today',
    moduleId: 'viewmodels/today',
    name: 'Today',
    visible: true,
    caption: 'Today',
    settings: {
      id:'today-button'
    }
  }, {
    url: 'performance',
    moduleId: 'viewmodels/performance',
    name: 'Performance',
    visible: true,
    caption: 'Performance',
    settings: {
      id: 'performance-button'
    }
  }, {
    url: 'free-bets',
    moduleId: 'viewmodels/today',
    name: 'Free Bets',
    visible: true,
    caption: 'Free Bets',
    settings: {
      id:'free-bets-button'
    }
  },
  //{
  //  url: 'admin',
  //  moduleId: 'viewmodels/admin',
  //  name: 'Admin',
  //  visible: true,
  //  settings: { admin: true }
  //},
  {
    url: 'today/competition/:competition',
    moduleId: 'viewmodels/today',
    name: 'Today'
  }, {
    url: 'today/sport/:sport',
    moduleId: 'viewmodels/today',
    name: 'Today'
  }];

  var startModule = 'today';

  var remoteServiceName = location.hostname.match('localhost') ? 'http://localhost:3600/api' : 'http://samuraiapi.apphb.com/api';
  var signalrServiceName = location.hostname.match('localhost') ? 'http://localhost:3600/signalr' : 'http://samuraiapi.apphb.com/signalr';

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
