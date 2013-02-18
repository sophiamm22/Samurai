(function() {
  var Coordinate, CoordinateBuilder, MatrixBuilder, coordBuilder, init, newCoords;

  MatrixBuilder = (function() {

    function MatrixBuilder(pa, pb, momentum) {
      this.pa = pa;
      this.pb = pb;
      this.momentum = momentum;
      this.qa = 1 - this.pa;
      this.upa = this.pa / (this.pa + Math.exp(-this.momentum) * this.qa);
      this.uqa = 1 - this.upa;
      this.dpa = this.pa / (this.pa + Math.exp(this.momentum) * this.qa);
      this.dqa = 1 - this.dpa;
    }

    MatrixBuilder.prototype.probability = function(aServes, aScore, bScore, aWinsProb, bWinsProb) {
      if (aScore === bScore) {
        if (aServes) {
          return this.pa * aWinsProb + this.qa * bWinsProb;
        } else {
          return this.qb * aWinsProb + this.qb * bWinsProb;
        }
      } else if (aScore > bScore) {
        if (aServes) {
          return this.upa * aWinsProb + this.uqa * bWinsProb;
        } else {
          return this.dqb * aWinsProb + this.dpb * bWinsProb;
        }
      } else {
        if (aServes) {
          return this.dpa * aWinsProb + this.dqa * bWinsProb;
        } else {
          return this.uqb * aWinsProb + this.upb * bWinsProb;
        }
      }
    };

    return MatrixBuilder;

  })();

  Coordinate = (function() {

    function Coordinate() {}

    Coordinate.prototype.contructor = function(order, aServes, aScore, bScore, probability) {
      this.order = order;
      this.aServes = aServes;
      this.aScore = aScore;
      this.bScore = bScore;
      this.probability = probability;
    };

    return Coordinate;

  })();

  CoordinateBuilder = (function() {

    function CoordinateBuilder(size, init) {
      this.size = size;
      this.init = init;
    }

    CoordinateBuilder.prototype.build = function(aServes) {
      var existing, i, j, order, x;
      x = new Array();
      i = 0;
      order = 0;
      while (i < (2 * this.size - 1)) {
        j = 0;
        while (j <= i) {
          existing = _.chain(this.init).find(function(coordinate) {
            return coordinate.aScore === j && coordinate.bScore === i;
          }).first().value();
          if (existing) {
            existing.order = order;
          }
          x.push(existing ? existing : new Coordinate(order, aServes, j, i - j, 0));
          order++;
          j++;
        }
      }
      i++;
      return x;
    };

    return CoordinateBuilder;

  })();

  init = [new Coordinate(0, true, 0, 0, 0.5), new Coordinate(0, true, 1, 0, 0.5)];

  coordBuilder = new CoordinateBuilder(5, init);

  newCoords = coordBuilder.build(true);

}).call(this);
