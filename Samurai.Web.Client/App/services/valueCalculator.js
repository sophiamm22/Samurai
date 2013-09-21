define(['services/kellyCalcs', 'config'],
  function (kelly, config) {
    var ValueCalculator = function () {
      var self = this;

      self.isCalculating = ko.observable(false);
      self.progress = ko.observable();
      self.betPercentages = ko.observableArray();

      return self;
    };

    ValueCalculator.prototype = function () {
      var calculateValue = function (footballSchedules, tennisSchedules) {

        var footballBets = _.filter(footballSchedules, function (schedule) {
          return schedule.hasQualifyingBet();
        });
        var tennisBets = _.filter(tennisSchedules, function (schedule) {
          return schedule.hasQualifyingBet();
        });
        var allBets = _.union(footballBets, tennisBets);
        var calculatedBets = _.map(allBets, function (bet) {
          return { 
            id: bet.id(),
            calcBet: new kelly.CalculatedBet(bet.valueOdd(), bet.valuePrediction())
          };
        });
        var calculator;
        if (calculatedBets.length >= config.exhaustiveKellyLimit) {
          calculator = new kelly.WhitrowKelly(config.kellyMultiplier, 0, _.map(calculatedBets, function (bet) { return bet.calcBet; }));
        } else {
          calculator = new kelly.ExhaustiveKelly(config.kellyMultiplier, 0, _.map(calculatedBets, function (bet) { return bet.calcBet; }));
        }
        var betPercentages = calculator.calculateKelly();

        return _.object(_.map(calculatedBets, function (bet) { return bet.id; }), betPercentages);
      };
      return {
        calculateValue: calculateValue
      };
    }();

    return ValueCalculator;
  });