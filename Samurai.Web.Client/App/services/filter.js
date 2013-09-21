define(['services/utils', 'config'],
  function (utils, config) {
    var BetFilter = function () {
      var self = this;
      self.valueBetsOnly = ko.observable(false);
      self.sportSlug = ko.observable();
      self.tournamentSlug = ko.observable();
      self.searchText = ko.observable().extend({ throttle: config.throttle });
      return self;
    };

    BetFilter.prototype = function () {
      var searchTest = function (searchText, match) {
        if (!searchText) return true;
        var srch = utils.regExEscape(searchText.toLowerCase());
        if (match.homeTeam().toLowerCase().search(srch) !== -1) return true;
        if (match.awayTeam().toLowerCase().search(srch) !== -1) return true;
        if (match.tournament().toLowerCase().search(srch) !== -1) return true;
        return false;
      },
      valueBetTest = function (valueBetsOnly, match) {
        if (valueBetsOnly) {
          var isValueBet = match.hasQualifyingBet();
          return isValueBet;
        }
        return true;
      },
      modelTest = function (sportSlug, tournamentSlug, match) {
        if (sportSlug && sportSlug.length !== 0 && sportSlug !== utils.convertToSlug(match.sport()) ||
            tournamentSlug && tournamentSlug.length !==0 && tournamentSlug !== utils.convertToSlug(match.tournament())) return false;
        return true;
      },
      predicate = function (self, match) {
        var isMatch = searchTest(self.searchText(), match)
            && valueBetTest(self.valueBetsOnly(), match)
            && modelTest(self.sportSlug(), self.tournamentSlug(), match);
        return isMatch;
      };
      return {
        predicate: predicate
      };
    }();

    return BetFilter;
  });