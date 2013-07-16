define(['services/logger'],

  function (logger) {
    var initialised = false;




    var vm = {
      activate: activate,
      title: 'Performance'
    };

    return vm;

    //#region Internal Methods
    function activate() {
      if (!initialised) {

        initialised = true;
      }
      return true;
    }

    function initiliseDC() {
      d3.csv("Performance.csv", function (data) {
        var dateFormat = d3.time.format("%d/%m/$Y");
        var numberFormat = d3.format(".2f");

        data.forEach(function (e) {
          e.dd = dateFormat.parse(e.MatchDate);
          e.month = d3.time.month(e.dd);
        });
        var perf = crossfilter(data);
        var all = perf.groupAll();
        var profitByMonth = perf.dimension(function (d) { return d3.time.month(d.dd); });
        var profitByMonthGroup = profitByMonth.group().reduceSum(function (d) { return d.FixedProfit; });
      });
    }

    //#endregion
});