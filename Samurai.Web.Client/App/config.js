define(function () {
  toastr.options.timeOut = 4000;
  toastr.options.positionClass = 'toaster-bottom-right';

  var routes = [{
    url: 'today',
    moduleId: 'viewmodels/today',
    name: 'Today',
    visible: true
  }, {
    url: 'home',
    moduleId: 'viewmodels/home',
    name: 'Home',
    visible: true
  }, {
    url: 'football',
    moduleId: 'viewmodels/football',
    name: 'Football',
    visible: true
  }, {
    url: 'tennis',
    moduleId: 'viewmodels/tennis',
    name: 'Tennis',
    visible: true
  }];

  var startModule = 'home';

  var remoteServiceName = 'http://localhost:3600/api';

  return {
    routes: routes,
    startModule: startModule,
    remoteServiceName: remoteServiceName
  };

});
