(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  require(['jquery'], function($) {
    return $(function() {
      var CalculatedBet, ExhaustiveKelly, Kelly, WhitrowKelly;
      Array.prototype.sum = function() {
        if (this.length > 0) {
          return this.reduce(function(x, y) {
            return x + y;
          });
        } else {
          return 0;
        }
      };
      CalculatedBet = (function() {

        function CalculatedBet(odds, prob) {
          this.odds = odds;
          this.prob = prob;
          this.edge = this.odds * this.prob - 1;
        }

        return CalculatedBet;

      })();
      Kelly = (function() {

        function Kelly(kellyMultiplier, minEdge, calculatedBets) {
          this.kellyMultiplier = kellyMultiplier;
          this.minEdge = minEdge;
          this.calculatedBets = calculatedBets != null ? calculatedBets : {};
          this.realKellyStakes;
        }

        Kelly.singleKellyStakes = function(calculatedBets, kellyMultiplier) {
          return calculatedBets.map(function(bet) {
            if (bet.edge >= this.minEdge) {
              return Math.max((Math.pow((bet.odds - 1) * bet.prob, kellyMultiplier) - Math.pow(1 - bet.prob, kellyMultiplier)) / (Math.pow(bet.win * bet.prob, kellyMultiplier) + (bet.odds - 1) * Math.pow(1 - bet.prob, kellyMultiplier)), 0);
            } else {
              return 0;
            }
          });
        };

        return Kelly;

      })();
      WhitrowKelly = (function(_super) {
        var learningRate, learningSteps, noBets, normaliseKelly, runs, singleKellyStakes, slopes, trials;

        __extends(WhitrowKelly, _super);

        function WhitrowKelly() {
          return WhitrowKelly.__super__.constructor.apply(this, arguments);
        }

        trials = 200;

        runs = 200;

        learningSteps = 300;

        noBets = WhitrowKelly.calculatedBets.length;

        singleKellyStakes = normaliseKelly(WhitrowKelly.singleKellyStakes(WhitrowKelly.calculatedBets, 1));

        normaliseKelly = singleKellyStakes(function() {
          var totalStakes;
          totalStakes = singleKellyStakes.sum();
          return singleKellyStakes.map(function(bet) {
            if (totalStakes > 1) {
              return bet / totalStakes;
            } else {
              return bet;
            }
          });
        });

        learningRate = function(slopePrev, slope, ratePrevious) {
          return ratePrevious * (Math.sign(slopePrev) === Math.sign(slop) ? 1.05 : 0.95);
        };

        slopes = function(t, b) {
          var bet, growthContribution, jointPlusMinus, r, rs, slope, trial, z, zOver, zx, _i, _j, _k;
          jointPlusMinus = {};
          rs = {};
          growthContribution = {};
          zOver = {};
          slope = {};
          for (trial = _i = 0; 0 <= t ? _i < t : _i > t; trial = 0 <= t ? ++_i : --_i) {
            z = {};
            zx = {};
            for (b = _j = 0; 0 <= noBets ? _j < noBets : _j > noBets; b = 0 <= noBets ? ++_j : --_j) {
              r = Math.random();
              bet = this.calculatedBets[b];
              if (r < bet.prob) {
                z[b] = bet.odds - 1;
                zx[b] = (bet.odds - 1) * this.realKellyStakes[b];
              } else {
                z[b] = -1.0;
                zx[b] = -this.realKellyStakes[b];
              }
            }
          }
          jointPlusMinus[trial] = zx.sum();
          rs[trial] = jointPlusMinus[trial] + 1;
          growthContribution[trial] = Math.log(rs[trial]) / trials;
          for (b = _k = 0; 0 <= noBets ? _k < noBets : _k > noBets; b = 0 <= noBets ? ++_k : --_k) {
            slope[b] = (zOver.transpose())[b].sum();
          }
          return slope;
        };

        WhitrowKelly.prototype.calculateKelly = function() {
          var adjustedKellyStake, b, lastRate, lastSlope, proposed, rate, rescaleFactor, run, slope, step, totalProposedStake, update, _i, _j, _k;
          for (run = _i = 0; 0 <= runs ? _i < runs : _i > runs; run = 0 <= runs ? ++_i : --_i) {
            lastRate = {};
            rate = {};
            step = {};
            update = {};
            proposed = {};
            lastSlope = {};
            slope = slopes(trials, noBets);
            for (b = _j = 0; 0 <= noBets ? _j < noBets : _j > noBets; b = 0 <= noBets ? ++_j : --_j) {
              rate[b] = !run ? 1 : learningRate(lastSlope[b], slope[b], lastSlope[b]);
              adjustedKellyStake = singleKellyStakes[b];
              step[b] = singleKellyStakes[b] - adjustedKellyStake;
              update[b] = Math.sign(slope[b]) * rate[b] * step[b];
              proposed[b] = adjustedKellyStake + update[b];
              totalProposedStake = proposed.sum();
              rescaleFactor = totalProposedStake > 1 ? 0.99 / totalProposedStake : 1;
            }
            for (b = _k = 0; 0 <= noBets ? _k < noBets : _k > noBets; b = 0 <= noBets ? ++_k : --_k) {
              this.realKellyStakes[b] = proposed[b] < 0 ? 0 : proposed[b] * rescaleFactor;
            }
            lastRate = rate;
          }
          return this.realKellyStakes;
        };

        return WhitrowKelly;

      })(Kelly);
      return ExhaustiveKelly = (function(_super) {
        var bitImp, parlaySize, toRadix;

        __extends(ExhaustiveKelly, _super);

        function ExhaustiveKelly() {
          return ExhaustiveKelly.__super__.constructor.apply(this, arguments);
        }

        bitImp = function(a, b) {
          return ~a | b;
        };

        toRadix = function(n, radix) {
          var HexN, Q, R;
          HexN = "";
          Q = Math.floor(Math.abs(n));
          R = void 0;
          while (true) {
            R = Q % radix;
            HexN = "0123456789abcdefghijklmnopqrstuvwxyz".charAt(R) + HexN;
            Q = (Q - R) / radix;
            if (Q === 0) {
              break;
            }
          }
          if (n < 0) {
            return "-" + HexN;
          } else {
            return HexN;
          }
        };

        parlaySize = function(n) {
          return toRadix(n, 2).reduce(function(t, s) {
            return t || 0 + s;
          });
        };

        ExhaustiveKelly.prototype.calculateKelly = function() {
          var bets, i, ii, k, limit, parlayMaps, parlayNames, parlayNumber, pp, s, singleKellyStakes, singles, ss, ssLimit, _i, _j, _k, _l, _m, _n, _ref;
          singles = calculatedBets.length;
          bets = Math.pow(2, singles);
          singleKellyStakes = {};
          parlayNames = {};
          parlayMaps = {};
          this.realKellyStakes = {};
          singleKellyStakes = this.singleKellyStakes(this.calculatedBets, this.kellyMultiplier);
          for (i = _i = 1; 1 <= bets ? _i <= bets : _i >= bets; i = 1 <= bets ? ++_i : --_i) {
            parlayMaps[parlaySize(i) - 1].push(i);
          }
          for (s = _j = singles; _j >= 1; s = _j += -1) {
            limit = parlayMaps[s - 1].length;
            for (i = _k = 0; 0 <= limit ? _k < limit : _k > limit; i = 0 <= limit ? ++_k : --_k) {
              parlayNumber = parlayMap[s - 1][i];
              this.realKellyStakes[parlayNumber] = 1;
              parlayNames[parlayNumber] = "";
              for (k = _l = 0; 0 <= singles ? _l < singles : _l > singles; k = 0 <= singles ? ++_l : --_l) {
                if (Math.pow(bitImp(2, k), parlayNumber) === -1) {
                  this.realKellyStakes[parlayNumber] *= singleKellyStakes[k];
                  parlayNames[parlayNumber] += (k + 1) + "+";
                }
              }
              for (ss = _m = _ref = s + 1; _ref <= singles ? _m <= singles : _m >= singles; ss = _ref <= singles ? ++_m : --_m) {
                ssLimit = parlayMaps[ss - 1].length;
                for (ii = _n = 0; 0 <= ssLimit ? _n < ssLimit : _n > ssLimit; ii = 0 <= ssLimit ? ++_n : --_n) {
                  pp = parlayMaps[ss - 1][ii];
                  if (bitImp(parlayNumber, pp === -1)) {
                    this.realKellyStakes[parlayNumber] -= this.realKellyStakes[pp];
                  }
                }
              }
            }
          }
          return this.realKellyStakes;
        };

        return ExhaustiveKelly;

      })(Kelly);
    });
  });

}).call(this);
