define('services/kellyCalcs',
  function () { 
    return  (function () {
      var CalculatedBet, ExhaustiveKelly, Kelly, WhitrowKelly,
        __hasProp = {}.hasOwnProperty,
        __extends = function (child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

      CalculatedBet = (function () {

        function CalculatedBet(odds, prob) {
          this.odds = odds;
          this.prob = prob;
          this.edge = this.odds * this.prob - 1;
        }

        return CalculatedBet;

      })();

      Kelly = (function () {

        function Kelly() { }

        Kelly.singleKellyStakes = function (calculatedBets, kellyMultiplier) {
          if (calculatedBets) {
            return calculatedBets.map(function (bet) {
              if (bet.edge >= this.minEdge) {
                return Math.max((Math.pow((bet.odds - 1) * bet.prob, kellyMultiplier) - Math.pow(1 - bet.prob, kellyMultiplier)) / (Math.pow(bet.win * bet.prob, kellyMultiplier) + (bet.odds - 1) * Math.pow(1 - bet.prob, kellyMultiplier)), 0);
              } else {
                return 0;
              }
            });
          }
        };

        return Kelly;

      })();

      WhitrowKelly = (function (_super) {
        var learningSteps, runs, trials;

        __extends(WhitrowKelly, _super);

        function WhitrowKelly(kellyMultiplier, minEdge, calculatedBets) {
          this.kellyMultiplier = kellyMultiplier;
          this.minEdge = minEdge;
          this.calculatedBets = calculatedBets != null ? calculatedBets : [];
          this.noBets = this.calculatedBets.length;
          this.singleKellyStakes;
          this.realKellyStakes;
        }

        trials = 200;

        runs = 200;

        learningSteps = 300;

        WhitrowKelly.prototype.calcSingleKellyStakes = function (calculatedBets, kellyMultiplier, minEdge) {
          return calculatedBets.map(function (bet) {
            if (bet.edge >= minEdge) {
              return Math.max((Math.pow((bet.odds - 1) * bet.prob, kellyMultiplier) - Math.pow(1 - bet.prob, kellyMultiplier)) / (Math.pow((bet.odds - 1) * bet.prob, kellyMultiplier) + (bet.odds - 1) * Math.pow(1 - bet.prob, kellyMultiplier)), 0);
            } else {
              return 0;
            }
          });
        };

        WhitrowKelly.prototype.sum = function (list) {
          if (list.length === 0) {
            return 0;
          } else {
            return list.reduce(function (a, b) {
              return a + b;
            });
          }
        };

        WhitrowKelly.prototype.sign = function (n) {
          return n && n / Math.abs(n);
        };

        WhitrowKelly.prototype.normaliseKelly = function (singKellyStakes) {
          var totalStakes;
          totalStakes = this.sum(singKellyStakes);
          return singKellyStakes.map(function (bet) {
            if (totalStakes > 1) {
              return bet / totalStakes;
            } else {
              return bet;
            }
          });
        };

        WhitrowKelly.prototype.learningRate = function (slopePrev, slope, ratePrevious) {
          if (!ratePrevious) {
            ratePrevious = 0;
          }
          return ratePrevious * (this.sign(slopePrev) === this.sign(slope) ? 1.05 : 0.95);
        };

        WhitrowKelly.prototype.slopes = function (t, b) {
          var bet, growthContribution, jointPlusMinus, r, rs, slope, trial, z, zOver, zx, _i, _j, _k, _l, _m, _ref, _ref1, _ref2;
          jointPlusMinus = [];
          rs = [];
          growthContribution = [];
          zOver = [];
          slope = [];
          for (trial = _i = 0; _i < t; trial = _i += 1) {
            z = [];
            zx = [];
            for (b = _j = 0, _ref = this.noBets; _j < _ref; b = _j += 1) {
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
            jointPlusMinus[trial] = this.sum(zx);
            rs[trial] = jointPlusMinus[trial] + 1;
            growthContribution[trial] = Math.log(rs[trial]) / trials;
            for (b = _k = 0, _ref1 = this.noBets; _k < _ref1; b = _k += 1) {
              if (!zOver[trial]) {
                zOver[trial] = [];
              }
              zOver[trial][b] = z[b] / (trials * rs[trial]);
            }
          }
          for (b = _l = 0, _ref2 = this.noBets; _l < _ref2; b = _l += 1) {
            slope[b] = 0;
            for (t = _m = 0; _m < trials; t = _m += 1) {
              slope[b] += zOver[t][b];
            }
          }
          return slope;
        };

        WhitrowKelly.prototype.calculateKelly = function () {
          var b, lastRate, lastSlope, proposed, rate, rescaleFactor, run, slope, step, totalProposedStake, update, _i, _j, _k, _l, _ref, _ref1, _ref2;
          this.singleKellyStakes = this.calcSingleKellyStakes(this.calculatedBets, 1, 0);
          this.realKellyStakes = this.normaliseKelly(this.singleKellyStakes);
          lastRate = [];
          rate = [];
          step = [];
          update = [];
          proposed = [];
          lastSlope = [];
          for (run = _i = 0; _i < runs; run = _i += 1) {
            slope = this.slopes(trials, this.noBets);
            for (b = _j = 0, _ref = this.noBets; _j < _ref; b = _j += 1) {
              rate[b] = run === 0 ? 1 : this.learningRate(lastSlope[b], slope[b], lastSlope[b]);
              step[b] = this.singleKellyStakes[b] - this.realKellyStakes[b];
              update[b] = this.sign(slope[b]) * rate[b] * step[b];
              proposed[b] = this.realKellyStakes[b] + update[b];
            }
            totalProposedStake = this.sum(proposed);
            rescaleFactor = totalProposedStake > 1 ? 0.99 / totalProposedStake : 1;
            for (b = _k = 0, _ref1 = this.noBets; _k < _ref1; b = _k += 1) {
              this.realKellyStakes[b] = proposed[b] < 0 ? 0 : proposed[b] * rescaleFactor;
            }
            lastRate = rate;
            lastSlope = slope;
          }
          if (this.kellyMultiplier !== 1) {
            for (b = _l = 0, _ref2 = this.noBets; _l < _ref2; b = _l += 1) {
              this.realKellyStakes[b] = this.kellyMultiplier * this.realKellyStakes[b];
            }
          }
          return this.realKellyStakes;
        };

        return WhitrowKelly;

      })(Kelly);

      ExhaustiveKelly = (function (_super) {

        __extends(ExhaustiveKelly, _super);

        function ExhaustiveKelly(kellyMultiplier, minEdge, calculatedBets) {
          this.kellyMultiplier = kellyMultiplier;
          this.minEdge = minEdge;
          this.calculatedBets = calculatedBets != null ? calculatedBets : [];
          this.realKellyStakes;
        }

        ExhaustiveKelly.prototype.calcSingleKellyStakes = function (calculatedBets, kellyMultiplier, minEdge) {
          if (calculatedBets) {
            return calculatedBets.map(function (bet) {
              if (!minEdge || bet.edge >= minEdge) {
                return Math.max((Math.pow((bet.odds - 1) * bet.prob, kellyMultiplier) - Math.pow(1 - bet.prob, kellyMultiplier)) / (Math.pow((bet.odds - 1) * bet.prob, kellyMultiplier) + (bet.odds - 1) * Math.pow(1 - bet.prob, kellyMultiplier)), 0);
              } else {
                return 0;
              }
            });
          }
        };

        ExhaustiveKelly.prototype.bitImp = function (a, b) {
          return ~a | b;
        };

        ExhaustiveKelly.prototype.toRadix = function (n, radix) {
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

        ExhaustiveKelly.prototype.parlaySize = function (n) {
          return this.toRadix(n, 2).split('').reduce(function (t, s) {
            return parseInt(t) + parseInt(s);
          });
        };

        ExhaustiveKelly.prototype.calculateKelly = function () {
          var bets, i, ii, k, limit, pSize, parlayMaps, parlayNames, parlayNumber, pp, realKellyStakes, s, singleKellyStakes, singles, ss, ssLimit, _i, _j, _k, _l, _m, _n, _o, _ref;
          singles = this.calculatedBets.length;
          bets = Math.pow(2, singles) - 1;
          singleKellyStakes = [];
          parlayNames = [];
          parlayMaps = [];
          realKellyStakes = [];
          singleKellyStakes = this.calcSingleKellyStakes(this.calculatedBets, this.kellyMultiplier, this.minEdge);
          for (i = _i = 0; _i < singles; i = _i += 1) {
            parlayMaps[i] = [];
          }
          for (i = _j = 1; _j <= bets; i = _j += 1) {
            pSize = parseInt(this.parlaySize(i));
            parlayMaps[pSize - 1].push(i);
          }
          for (s = _k = singles; _k >= 1; s = _k += -1) {
            limit = parlayMaps[s - 1].length;
            for (i = _l = 0; _l < limit; i = _l += 1) {
              parlayNumber = parlayMaps[s - 1][i];
              realKellyStakes[parlayNumber] = 1;
              parlayNames[parlayNumber] = "";
              for (k = _m = 0; _m < singles; k = _m += 1) {
                if (this.bitImp(Math.pow(2, k), parlayNumber) === -1) {
                  realKellyStakes[parlayNumber] *= singleKellyStakes[k];
                  parlayNames[parlayNumber] += (k + 1) + "+";
                }
              }
              for (ss = _n = _ref = s + 1; _n <= singles; ss = _n += 1) {
                ssLimit = parlayMaps[ss - 1].length;
                for (ii = _o = 0; _o < ssLimit; ii = _o += 1) {
                  pp = parlayMaps[ss - 1][ii];
                  if (this.bitImp(parlayNumber, pp) === -1) {
                    realKellyStakes[parlayNumber] -= realKellyStakes[pp];
                  }
                }
              }
            }
          }
          var ret = _.filter(_.zip(parlayNames, realKellyStakes), function (kelly) { return kelly[0] && kelly[0].length === 2; });
          return _.map(ret, function (kelly) { return kelly[1]; });
        };

        return ExhaustiveKelly;

      })(Kelly);
      return {
        Kelly: Kelly,
        CalculatedBet: CalculatedBet,
        ExhaustiveKelly: ExhaustiveKelly,
        WhitrowKelly: WhitrowKelly
      }
    })()
  });