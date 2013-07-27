define(['services/logger', 'config'],

  function (logger, config) {
    var initialised = false;

    var dailyBubbleChart = null;
    var profitOrLossChart = null;
    var betsBySurfaceChart = null;
    var betsBySeriesChart = null; 
    var edgeChart = null; 
    var gamesPlayedChart = null; 

    var vm = {
      activate: activate,
      title: 'Performance',
      viewAttached: viewAttached,
      dailyBubbleChart: dailyBubbleChart,
      profitOrLossChart: profitOrLossChart,
      betsBySurfaceChart: betsBySurfaceChart,
      betsBySeriesChart: betsBySeriesChart, 
      edgeChart: edgeChart,
      gamesPlayedChart: gamesPlayedChart     
    };

    return vm;

    //#region Internal Methods
    function activate() {
      if (!initialised) {

        initialised = true;
      }
      return true;
    }

    function viewAttached(view) {
      dailyBubbleChart = dc.bubbleChart('#daily-bubble-chart', 'group');
      profitOrLossChart = dc.pieChart('#profit-or-loss-chart', 'group');
      betsBySurfaceChart = dc.rowChart('#bets-by-surface-chart', 'group');
      betsBySeriesChart = dc.rowChart('#bets-by-series-chart', 'group');
      edgeChart = dc.barChart('#edge-chart', 'group');
      gamesPlayedChart = dc.barChart('#games-played-chart', 'group');

      d3.csv("content/Performance.csv", function (data) {
        var dateFormat = d3.time.format("%d-%m-%Y");
        var numberFormat = d3.format(".2f");
        var index = 1;

        data.forEach(function (e) {
          e.day = dateFormat.parse(e.MatchDate);
          e.stake = (config.kellyMultiplier * config.startBank * (e.MatchOutcomeProbability * e.OddAfterCommission - 1) / (e.OddAfterCommission - 1));
          e.flatProfit = (e.stake * (e.ExpectedMatchOutcome == e.ObservedMatchOutcome ? (e.OddAfterCommission - 1) : -1));
          e.odds = 1 * e.OddAfterCommission;
          e.identifier = index;

          //mimic a knockout observable tennis schedule
          e.id = function () { return e.identifier; };
          e.hasQualifyingBet = function () { return true; };
          e.valueOdd = function () { return e.odds; };
          e.valuePrediction = function () { return e.MatchOutcomeProbability; };

          index++;
        });
        var perf = crossfilter(data);
        var all = perf.groupAll();

        //#region Dimensions and Groups
        //All dimension
        var allDimension = perf.dimension(function (d) {
          return d.identifier;
        });

        //By edge
        var edge = perf.dimension(function (d) {
          var x = Math.round((d.MatchOutcomeProbability * d.OddAfterCommission - 1) * 100);
          return x;
        });
        var edgeGroup = edge.group();

        //By games played
        var gamesPlayed = perf.dimension(function (d) {
          return Math.min(1 * d.PlayerAGames, 1 * d.PlayerBGames);
        });
        var gamesPlayedGroup = gamesPlayed.group();

        //By day
        var betsByDay = perf.dimension(function (d) {
          return d3.time.day(d.day);
        });
        var betsByDayGroup = betsByDay.group().reduce(
            function (p, v) {
              ++p.bets;
              p.stake += v.stake;
              p.profit += v.flatProfit;
              p.totalOdd += v.odds;
              p.avOdd = p.bets == 0 ? 0.0 : (p.totalOdd / p.bets);

              var betWon = v.ExpectedMatchOutcome == v.ObservedMatchOutcome;
              var playerBetOn = v.ExpectedMatchOutcome == 'Home Win' ? v.PlayerASurname : v.PlayerBSurname;
              var playerBetAgainst = v.ExpectedMatchOutcome == 'Home Win' ? v.PlayerBSurname : v.PlayerASurname;

              p.matches[playerBetOn + ' to bt. ' + playerBetAgainst] = playerBetOn + ' to bt. ' + playerBetAgainst + ' @ ' + v.TournamentName + (betWon ? ' WON ' : ' LOST ') + ' ($' + numberFormat(v.flatProfit) + ')';

              return p;
            },
            function (p, v) {
              --p.bets;
              p.stake -= v.stake;
              p.profit -= v.flatProfit;
              p.totalOdd -= v.odds;
              p.avOdd = p.bets == 0 ? 0.0 : (p.totalOdd / p.bets);

              var playerBetOn = v.ExpectedMatchOutcome == 'Home Win' ? v.PlayerASurname : v.PlayerBSurname;
              var playerBetAgainst = v.ExpectedMatchOutcome == 'Home Win' ? v.PlayerBSurname : v.PlayerASurname;

              delete p.matches[playerBetOn + ' to bt. ' + playerBetAgainst];

              return p;
            },
            function () {
              return {
                bets: 0,
                stake: 0,
                profit: 0,
                totalOdd: 0.0,
                avOdd: 0.0,
                matches: {},
                dailyKellyData: {}
              };
            });

        //By surface
        var betsBySurface = perf.dimension(function (d) {
          switch (d.Surface) {
            case 'Hard':
              return '0.Hard';
            case 'Clay':
              return '1.Clay';
            case 'Grass':
              return '2.Grass';
          }
        });
        var betsBySurfaceGroup = betsBySurface.group();
        //Profit or loss
        var profitOrLoss = perf.dimension(function (d) {
          return d.flatProfit >= 0 ? 'Profit' : 'Loss';
        });
        var profitOrLossGroup = profitOrLoss.group();
        //By series
        var betsBySeries = perf.dimension(function (d) {
          switch (d.Series) {
            case 'Grand Slam':
              return '0.Grand Slam';
            case 'Masters':
              return '1.Masters';
            case 'ATP':
              return '2.ATP'
          }
          return d.Series;
        });
        var betsBySeriesGroup = betsBySeries.group();
        //#endregion

        //#region Charts
        dailyBubbleChart.width(1200)
            .height(300)
            .margins({
              top: 10,
              right: 50,
              bottom: 30,
              left: 100
            })
            .dimension(betsByDay)
            .group(betsByDayGroup)
            .transitionDuration(1500)
            .colors(["#a60000", "#ff0000", "#ff4040", "#ff7373", "#67e667", "#39e639", "#00cc00"])
            .colorDomain([-400, 400])
            .colorAccessor(function (d, i) {
              return d.value.profit;
            })
            .keyAccessor(function (p) {
              return p.key;
            })
            .valueAccessor(function (p) {
              return p.value.profit;
            })
            .radiusValueAccessor(function (p) {
              return p.value.stake;
            })
            .maxBubbleRelativeSize(.02)
            .x(d3.scale.linear().domain([new Date(2012, 12, 01), new Date(2013, 07, 31)]))
            .y(d3.scale.linear().domain([-200, 400]))
            .r(d3.scale.linear().domain([0, 200]))
            .elasticY(true)
            .xAxisPadding(300)
            .yAxisPadding(50)
            .renderHorizontalGridLines(true)
            .renderVerticalGridLines(true)
            .renderLabel(true)
            .renderTitle(true)
            .label(function (p) {
              return numberFormat(p.value.profit);
            })
            .title(function (p) {
              var matches = '';
              for (var match in p.value.matches) {
                matches += p.value.matches[match] + '\n';
              }
              return matches;
            })
            .renderlet(function (d) {
              getRoi(d);
            });
        dailyBubbleChart.xAxis().tickFormat(function (s) {
          return (new Date(s)).toLocaleDateString();
        });
        dailyBubbleChart.yAxis().tickFormat(function (s) {
          return '$' + s;
        })


        profitOrLossChart.width(160)
            .height(180)
            .radius(80)
            .dimension(profitOrLoss)
            .colors(['#ff7373', "#67e667"])
            .group(profitOrLossGroup)
            .label(function (d) {
              if (profitOrLossChart.hasFilter() && !profitOrLossChart.hasFilter(d.data.key)) {
                return d.data.key + "(0%)";
              }
              return d.data.key + " (" + Math.floor(d.data.value / all.value() * 100) + "%)";
            })
            .title(function (d) {
              return d.data.value + (d.data.key == 'Profit' ? ' winning' : ' losing') + ' bets';
            });

        betsBySurfaceChart.width(160)
            .height(180)
            .margins({
              top: 20,
              left: 10,
              right: 10,
              bottom: 20
            })
            .group(betsBySurfaceGroup)
            .dimension(betsBySurface)
            .colors(['#6baed6', 'darkorange', 'lightgreen'])
            .label(function (d) {
              return d.key.split('.')[1];
            })
            .title(function (d) {
              return (d.value + ' bets on ' + d.key.split('.')[1]).toLowerCase();
            })
            .elasticX(true)
            .xAxis().ticks(4);

        betsBySeriesChart.width(160)
            .height(180)
            .margins({
              top: 20,
              left: 10,
              right: 10,
              bottom: 20
            })
            .group(betsBySeriesGroup)
            .dimension(betsBySeries)
            .colors(['lightblue', 'gold', 'silver'])
            .label(function (d) {
              return d.key.split('.')[1];
            })
            .title(function (d) {
              return d.value + ' bets on the ' + d.key.split('.')[1] + ' tour';
            })
            .elasticX(true)
            .xAxis().ticks(4)

        edgeChart.width(290)
            .height(180)
            .margins({
              top: 10,
              right: 50,
              bottom: 30,
              left: 40
            })
            .dimension(edge)
            .group(edgeGroup)
            .elasticY(true)
            .centerBar(true)
            .gap(1)
            .round(dc.round.floor)
            .x(d3.scale.linear().domain([0, 120]))
            .renderHorizontalGridLines(true)
            .filter([20, 120])
            .xAxis()
            .tickFormat(function (v) {
              return v;
            });

        gamesPlayedChart.width(290)
            .height(180)
            .margins({
              top: 10,
              right: 50,
              bottom: 30,
              left: 40
            })
            .dimension(gamesPlayed)
            .group(gamesPlayedGroup)
            .elasticY(true)
            .centerBar(true)
            .gap(1)
            .round(dc.round.floor)
            .x(d3.scale.linear().domain([0, 120]))
            .renderHorizontalGridLines(true)
            .filter([70, 120])
            .xAxis()
            .tickFormat(function (v) {
              return v;
            });

        dc.dataTable('.dc-data-table', 'group')
            .dimension(allDimension)
            .group(function (d) {
              var text = d.TournamentName + ' (' + d.Series + ' - ' + d.Surface.toLowerCase() + ' court)';
              return text;
            })
            .size(100)
            .columns([
                function (d) {
                  return d.day.toLocaleDateString();
                },
                function (d) {
                  var playerBetOn = d.ExpectedMatchOutcome == 'Home Win' ? d.PlayerASurname : d.PlayerBSurname;
                  var playerBetAgainst = d.ExpectedMatchOutcome == 'Home Win' ? d.PlayerBSurname : d.PlayerASurname;

                  return playerBetOn + ' to bt. ' + playerBetAgainst;
                },
                function (d) {
                  return (100 * d.MatchOutcomeProbability).toFixed(2) + '%';
                },
                function (d) {
                  return d.OddAfterCommission;
                },
                function (d) {
                  return d.BookmakerName;
                },
                function (d) {
                  return (100 * (d.MatchOutcomeProbability * d.OddAfterCommission - 1)).toFixed(2) + '%';
                },
                function (d) {
                  return numberWithCommas(1000 * 0.25 * (d.MatchOutcomeProbability * d.OddAfterCommission - 1) / (d.OddAfterCommission - 1));
                },
                function (d) {
                  return d.ObservedMatchOutcome == d.ExpectedMatchOutcome ? "Won" : "Lost";
                },
                function (d) {
                  return numberWithCommas(d.stake * (d.ExpectedMatchOutcome == d.ObservedMatchOutcome ? (d.OddAfterCommission - 1) : -1));
                }
            ])
            .sortBy(function (d) {
              return d.day;
            })
            .order(d3.ascending)
            .renderlet(function (table) {
              table.selectAll(".dc-table-group").classed("info", true);
            });
        //#endregion

        dc.filterAll();
        dc.renderAll("group");
      });
    }

    function getRoi(d) {
      var stakes = 0;
      var profit = 0;
      var bets = 0;
      d.group().all().forEach(function (d) {
        stakes += d.value.stake;
        profit += d.value.profit;
        bets += d.value.bets;
      });
      $('#outcome').fadeOut(function () {
        $(this).text(bets + ' bets, totalling $' + numberWithCommas(stakes.toFixed(2)) + ', returned a ' + (profit >= 0 ? 'profit' : 'loss') + ' of $' + numberWithCommas(profit.toFixed(2)) + ' giving an ROI of ' + (stakes == 0 ? '0' : (profit * 100 / stakes).toFixed(2)) + '%')
      }).fadeIn();
    }

    function numberWithCommas(n) {
      var parts = n.toString().split(".");
      return parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",") + (parts[1] ? "." + parts[1].substring(0, 2) : "");
    }

    //#endregion
});