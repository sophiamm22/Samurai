﻿define(['durandal/system', 'durandal/plugins/router', 'services/logger', 'config', 'services/datacontext'],
    function (system, router, logger, config, datacontext) {

      var adminRoutes = ko.computed(function () {
        return router.allRoutes().filter(function (r) {
          return r.settings.admin;
        });
      });

      var shell = {
        viewAttached: viewAttached,
        activate: activate,
        adminRoutes: adminRoutes,
        router: router
      };

      return shell;

      //#region Internal Methods
      function viewAttached(view) {
        attachScrollEvent('#free-bets-button', '#free-bets');
        attachScrollEvent('#blog-button', '#free-bets', 'http://blog.valuesamurai.com');
        attachScrollEvent('#today-button', '#insert-sections');
        attachScrollEvent('#performance-button', '#insert-sections');
      }

      function attachScrollEvent(scrollFrom, scrollTo, navigateTo) {
        $(scrollFrom).click(function () {
          if (navigateTo) {
            window.open(navigateTo);
          }
          $('html, body').animate({
            scrollTop:($(scrollTo).position().top - 60)
          }, 'slow')
        });
      }

      function activate() {
        return datacontext.primeData()
          .then(boot)
          .fail(failedIntialisation);
      }

      function boot() {
        log('Fetching schedules..', null, system.getModuleId(shell), true);
        router.map(config.routes);
        return router.activate(config.startModule);
      }

      function failedIntialisation(error) {
        var msg = 'App initialisation failed: ' + error.message;
        logger.logError(msg, error, system.getModuleId(shell), true);
      }

      function log(msg, data, showToast) {
        logger.log(msg, data, system.getModuleId(shell), showToast);
      }
      //#endregion
    });