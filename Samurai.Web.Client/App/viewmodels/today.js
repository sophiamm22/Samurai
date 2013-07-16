define(['services/datacontext', 'services/groupSchedule', 'durandal/plugins/router', 'services/filter', 'services/valueCalculator'], 
  function (datacontext, groupSchedule, router, BetFilter, ValueCalculator) {

    var todaysFootballSchedules = ko.observableArray(),
        todaysTennisSchedules = ko.observableArray(),
        todaysFootballOdds = ko.observableArray(),
        todaysTennisOdds = ko.observableArray(),
        betFilter = new BetFilter(),
        valueCalculator = new ValueCalculator(),
        hasFootballOdds = ko.observable(true),
        hasTennisOdds = ko.observable(true),
        initialised = false;

        todaysMatchesCount = ko.computed(function () {
          return todaysFootballSchedules().length + todaysTennisSchedules().length;
        }),

        competitions = ko.computed(function () {
          return groupSchedule.schedulesToHeadings(todaysFootballSchedules(), todaysTennisSchedules());
        }),
        
        calculateValue = function(){
          var betPercentages = valueCalculator.calculateValue(todaysFootballSchedules(), todaysTennisSchedules());
          var schedules = _.union(todaysFootballSchedules(), todaysTennisSchedules());
          schedules.forEach(function (schedule) {
            if (betPercentages[schedule.id()]) {
              var betPercentage = betPercentages[schedule.id()];
              schedule.percentBet(betPercentage);
            }
          });
        },

        clearFilter = function () {
          betFilter.searchText('');
        },

        activate = function () {
          if (!initialised) {
            addFilterSubscriptions();
            initialised = true;
          }
          return getSchedulesAndOdds(dataOptions(false));
        },
        selectGrouping = function (sportOrCompetition) {
          betFilter.sportSlug(sportOrCompetition.sportSlug);
          betFilter.tournamentSlug(sportOrCompetition.competitionSlug);
        },
        refresh = function () {
          return getSchedulesAndOdds(dataOptions(true));
        },
        addFilterSubscriptions = function () {
          betFilter.valueBetsOnly.subscribe(onFilterChange);
          betFilter.sportSlug.subscribe(onFilterChange);
          betFilter.tournamentSlug.subscribe(onFilterChange);
          betFilter.searchText.subscribe(onFilterChange);
        },
        onFilterChange = function () {
          return activate();
        },
        dataOptions = function (force) {
          return {
            filter: betFilter,
            forceRefresh: force
          };
        },
        initialised = false,
        haveChecked = false;
        

  var vm = {
    title: 'Todays fixtures',
    activate: activate,
    todaysFootballSchedules: todaysFootballSchedules,
    todaysTennisSchedules: todaysTennisSchedules,
    todaysFootballOdds: todaysFootballOdds,
    todaysTennisOdds: todaysTennisOdds,
    todaysMatchesCount: todaysMatchesCount,
    competitions: competitions,
    refresh: refresh,
    betFilter: betFilter,
    clearFilter: clearFilter,
    selectGrouping: selectGrouping
  };
  return vm;

  function getSchedulesAndOdds(options) {
    return Q.resolve()
                      .then(function () {
                        if (hasFootballOdds()) {
                          return datacontext.getTodaysFootballSchedule(todaysFootballSchedules, options);
                        } else {
                          return Q.resolve();
                        }
                      })
                      .then(function () {
                        if (hasTennisOdds()) {
                          return datacontext.getTodaysTennisSchedule(todaysTennisSchedules, options);
                        } else {
                          return Q.resolve();
                        }
                      })
                      .then(function() { 
                        if (hasFootballOdds()) {
                          return datacontext.getTodaysLatestFootballOdds(todaysFootballOdds, options);
                        } else {
                          return Q.resolve();
                        }
                      })
                      .then(function() {
                        if (hasTennisOdds()) { 
                          return datacontext.getTodaysLatestTennisOdds(todaysTennisOdds, options);
                        } else {
                          return Q.resolve();
                        }
                      })
                      .fin(function () {
                        if (!haveChecked) {
                          hasFootballOdds(todaysFootballSchedules().length > 0);
                          hasTennisOdds(todaysTennisSchedules().length > 0);
                          haveChecked = true;
                        }
                      });;
  };

});