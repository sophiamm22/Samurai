class MatrixBuilder
	constructor: (@pa, @pb, @momentum) ->
		@qa = 1 - @pa
		@upa = @pa / (@pa + Math.exp(-@momentum) * @qa)
		@uqa = 1 - @upa
		@dpa = @pa / (@pa + Math.exp(@momentum) * @qa)
		@dqa = 1 - @dpa
	
	probability: (aServes, aScore, bScore, aWinsProb, bWinsProb) ->
		if aScore is bScore
			(if aServes then (@pa * aWinsProb + @qa * bWinsProb) else (@qb * aWinsProb + @qb * bWinsProb))
		else if aScore > bScore
			(if aServes then (@upa * aWinsProb + @uqa * bWinsProb) else (@dqb * aWinsProb + @dpb * bWinsProb))
		else
			(if aServes then (@dpa * aWinsProb + @dqa * bWinsProb) else (@uqb * aWinsProb + @upb * bWinsProb))
			
class Coordinate
	contructor: (@order, @aServes, @aScore, @bScore, @probability) ->
		
class CoordinateBuilder
	constructor: (@size, @init) ->
	
	build: (aServes) ->
		x = new Array()
		i = 0
		order = 0

		while i < (2 * @size - 1)
			j = 0
				
			while j <= i
				existing = _.chain(@init)
					.find((coordinate) -> coordinate.aScore is j and coordinate.bScore is i)
					.first()
					.value()
				if existing then existing.order = order
				x.push if existing then existing else new Coordinate(order, aServes, j, i - j, 0)
				order++
				j++
		i++
		x

init = [
	new Coordinate(0, true, 0, 0, 0.5),
	new Coordinate(0, true, 1, 0, 0.5)
]

coordBuilder = new CoordinateBuilder(5, init)
newCoords = coordBuilder.build true
