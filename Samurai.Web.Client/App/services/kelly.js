(function() {
  var CalculatedBet, ExhaustiveKelly, Kelly, WhitrowKelly,
    __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  CalculatedBet = (function() {

    function CalculatedBet(odds, prob) {
      this.odds = odds;
      this.prob = prob;
      this.edge = this.odds * this.prob - 1;
    }

    return CalculatedBet;

  })();

  Kelly = (function() {

    function Kelly() {}

    Kelly.singleKellyStakes = function(calculatedBets, kellyMultiplier) {
      if (calculatedBets) {
        return calculatedBets.map(function(bet) {
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

  WhitrowKelly = (function(_super) {
    var learningSteps, runs, trials;

    __extends(WhitrowKelly, _super);

    function WhitrowKelly(kellyMultiplier, minEdge, calculatedBets) {
      this.kellyMultiplier = kellyMultiplier;
      this.minEdge = minEdge;
      this.calculatedBets = calculatedBets != null ? calculatedBets : {};
      this.realKellyStakes;
    }

    trials = 200;

    runs = 200;

    learningSteps = 300;

    WhitrowKelly.calcSingleKellyStakes = function(calculatedBets, kellyMultiplier, minEdge) {
      return calculatedBets.map(function(bet) {
        if (bet.edge >= minEdge) {
          return Math.max((Math.pow((bet.odds - 1) * bet.prob, kellyMultiplier) - Math.pow(1 - bet.prob, kellyMultiplier)) / (Math.pow(bet.win * bet.prob, kellyMultiplier) + (bet.odds - 1) * Math.pow(1 - bet.prob, kellyMultiplier)), 0);
        } else {
          return 0;
        }
      });
    };

    WhitrowKelly.prototype.sum = function(list) {
      if (list.length === 0) {
        return 0;
      } else {
        return list.reduce(function(a, b) {
          return a + b;
        });
      }
    };

    WhitrowKelly.prototype.normaliseKelly = function(singKellyStakes) {
      var totalStakes;
      totalStakes = sum(singKellyStakes);
      return singKellyStakes.map(function(bet) {
        if (totalStakes > 1) {
          return bet / totalStakes;
        } else {
          return bet;
        }
      });
    };

    WhitrowKelly.prototype.noBets = function() {
      if (this.calculatedBets) {
        return this.calculatedBets.length;
      }
    };

    WhitrowKelly.prototype.singleKellyStakes = function() {
      return this.normaliseKelly(this.calcSingleKellyStakes(this.calculatedBets, 1));
    };

    WhitrowKelly.prototype.learningRate = function(slopePrev, slope, ratePrevious) {
      return ratePrevious * (Math.sign(slopePrev) === Math.sign(slop) ? 1.05 : 0.95);
    };

    WhitrowKelly.prototype.slopes = function(t, b) {
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
      jointPlusMinus[trial] = sum(zx);
      rs[trial] = jointPlusMinus[trial] + 1;
      growthContribution[trial] = Math.log(rs[trial]) / trials;
      for (b = _k = 0; 0 <= noBets ? _k < noBets : _k > noBets; b = 0 <= noBets ? ++_k : --_k) {
        slope[b] = sum((zOver.transpose())[b]);
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
          totalProposedStake = sum(proposed);
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

  ExhaustiveKelly = (function(_super) {

    __extends(ExhaustiveKelly, _super);

    function ExhaustiveKelly(kellyMultiplier, minEdge, calculatedBets) {
      this.kellyMultiplier = kellyMultiplier;
      this.minEdge = minEdge;
      this.calculatedBets = calculatedBets != null ? calculatedBets : {};
      this.realKellyStakes;
    }

    ExhaustiveKelly.prototype.calcSingleKellyStakes = function(calculatedBets, kellyMultiplier, minEdge) {
      if (calculatedBets) {
        return calculatedBets.map(function(bet) {
          if (bet.edge >= minEdge) {
            return Math.max((Math.pow((bet.odds - 1) * bet.prob, kellyMultiplier) - Math.pow(1 - bet.prob, kellyMultiplier)) / (Math.pow((bet.odds - 1) * bet.prob, kellyMultiplier) + (bet.odds - 1) * Math.pow(1 - bet.prob, kellyMultiplier)), 0);
          } else {
            return 0;
          }
        });
      }
    };

    ExhaustiveKelly.prototype.bitImp = function(a, b) {
      return ~a | b;
    };

    ExhaustiveKelly.prototype.toRadix = function(n, radix) {
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

    ExhaustiveKelly.prototype.parlaySize = function(n) {
      return this.toRadix(n, 2).split('').reduce(function(t, s) {
        return t || 0 + s;
      });
    };

    ExhaustiveKelly.prototype.calculateKelly = function() {
      var bets, i, ii, k, limit, pSize, parlayMaps, parlayNames, pp, realKellyStakes, s, singleKellyStakes, singles, ss, _i, _j, _k, _l, _m, _n, _o, _ref;
      singles = calculatedBets.length;
      bets = Math.pow(2, singles);
      singleKellyStakes = {};
      parlayNames = {};
      parlayMaps = [];
      realKellyStakes = {};
      singleKellyStakes = this.calcSingleKellyStakes(this.calculatedBets, this.kellyMultiplier, this.minEdge);
      for (i = _i = 1; _i <= singles; i = _i += 1) {
        parlayMaps[i] = [];
      }
      for (i = _j = 1; _j <= bets; i = _j += 1) {
        pSize = parseInt(this.parlaySize(i));
        if (!parlayMaps[pSize - 1]) {
          parlayMaps[pSize - 1] = [];
        }
        parlayMaps[pSize - 1].push(i);
      }
      for (s = _k = singles; _k >= 1; s = _k += -1) {
        limit = parlayMaps[s - 1].length;
        for (i = _l = 0; _l < limit; i = _l += 1) {
          this.parlayNumber = this.parlayMap[s - 1][i];
          this.realKellyStakes[parlayNumber] = 1;
          this.parlayNames[parlayNumber] = "";
          for (k = _m = 0; _m <= singles; k = _m += 1) {
            if (Math.pow(this.bitImp(2, k), this.parlayNumber) === -1) {
              this.realKellyStakes[parlayNumber] *= this.singleKellyStakes[k];
              this.parlayNames[this.parlayNumber] += (k + 1) + "+";
            }
          }
          for (ss = _n = _ref = s + 1; _n <= singles; ss = _n += 1) {
            this.ssLimit = this.parlayMaps[ss - 1].length;
            for (ii = _o = 0; _o < ssLimit; ii = _o += 1) {
              pp = parlayMaps[ss - 1][ii];
              if (this.bitImp(parlayNumber, pp === -1)) {
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

}).call(this);
