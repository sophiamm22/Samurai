﻿define(function () {
  toastr.options.timeOut = 4000;
  toastr.options.positionClass = 'toaster-bottom-right';

  var routes = [{
    url: 'today',
    moduleId: 'viewmodels/today',
    name: 'Today',
    visible: true
  }, {
    url: 'performance',
    moduleId: 'viewmodels/performance',
    name: 'Performance',
    visible: true
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

  var hashes = {
    today: '#/today',
    todayByCompetition: '#/today/competition',
    todayBySport: '#/today/sport'
  };

  return {
    routes: routes,
    startModule: startModule,
    remoteServiceName: remoteServiceName,
    hashes: hashes
  };

});
