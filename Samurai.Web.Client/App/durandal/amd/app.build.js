{
  "name": "durandal/amd/almond-custom",
  "inlineText": true,
  "stubModules": [
    "durandal/amd/text"
  ],
  "paths": {
    "text": "durandal/amd/text"
  },
  "baseUrl": "C:\\Git\\Samurai\\Samurai.Web.Client\\App",
  "mainConfigFile": "C:\\Git\\Samurai\\Samurai.Web.Client\\App\\main.js",
  "include": [
    "config",
    "main",
    "durandal/app",
    "durandal/composition",
    "durandal/events",
    "durandal/http",
    "text!durandal/messageBox.html",
    "durandal/messageBox",
    "durandal/modalDialog",
    "durandal/system",
    "durandal/viewEngine",
    "durandal/viewLocator",
    "durandal/viewModel",
    "durandal/viewModelBinder",
    "durandal/widget",
    "durandal/plugins/router",
    "durandal/transitions/entrance",
    "services/datacontext",
    "services/filter",
    "services/groupSchedule",
    "services/jsonResultsAdapter",
    "services/kelly",
    "services/kelly.min",
    "services/kellyCalcs",
    "services/logger",
    "services/model",
    "services/signalr",
    "services/utils",
    "services/valueCalculator",
    "viewmodels/admin",
    "viewmodels/home",
    "viewmodels/performance",
    "viewmodels/shell",
    "viewmodels/today",
    "text!views/admin.html",
    "text!views/footer.html",
    "text!views/home.html",
    "text!views/nav.html",
    "text!views/performance.html",
    "text!views/shell.html",
    "text!views/today.html"
  ],
  "exclude": [],
  "keepBuildDir": true,
  "optimize": "uglify2",
  "out": "C:\\Git\\Samurai\\Samurai.Web.Client\\App\\main-built.js",
  "pragmas": {
    "build": true
  },
  "wrap": true,
  "insertRequire": [
    "main"
  ]
}