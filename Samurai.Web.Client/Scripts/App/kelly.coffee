class CalculatedBet
    constructor: (@odds, @prob)->
        @edge = @odds * @prob - 1

class Kelly    
    @singleKellyStakes: (calculatedBets, kellyMultiplier) ->
        if calculatedBets then calculatedBets.map (bet) -> 
            if bet.edge >= @minEdge 
                Math.max(
                    (Math.pow(((bet.odds - 1) * bet.prob), kellyMultiplier) - Math.pow((1 - bet.prob), kellyMultiplier)) / (Math.pow((bet.win * bet.prob), kellyMultiplier) + (bet.odds - 1) * Math.pow((1 - bet.prob), kellyMultiplier))
                    ,0) 
            else 
                0

class WhitrowKelly extends Kelly

    constructor: (@kellyMultiplier, @minEdge, @calculatedBets=[]) ->
        @noBets = @calculatedBets.length
        @singleKellyStakes
        @realKellyStakes

    trials              = 200
    runs                = 200
    learningSteps       = 300

    calcSingleKellyStakes : (calculatedBets, kellyMultiplier, minEdge) ->
        calculatedBets.map (bet) -> 
            if bet.edge >= minEdge 
                Math.max(
                    (Math.pow(((bet.odds - 1) * bet.prob), kellyMultiplier) - Math.pow((1 - bet.prob), kellyMultiplier)) / (Math.pow(((bet.odds - 1) * bet.prob), kellyMultiplier) + (bet.odds - 1) * Math.pow((1 - bet.prob), kellyMultiplier))
                    ,0) 
            else 
                0
    
    sum : (list) ->
        (if list.length is 0 then 0 else list.reduce((a, b) ->
          a + b
        ))

    sign : (n) ->
        n && n / Math.abs(n)

    normaliseKelly : (singKellyStakes) ->
        totalStakes = @sum singKellyStakes
        singKellyStakes.map (bet) ->
            if totalStakes > 1 then bet / totalStakes else bet
    
    learningRate : (slopePrev, slope, ratePrevious) ->
        ratePrevious = 0 unless ratePrevious
        ratePrevious * (if @sign(slopePrev) is @sign(slope) then 1.05 else 0.95)

    slopes : (t, b) ->
        jointPlusMinus      = []
        rs                  = []
        growthContribution  = []
        zOver               = []
        slope               = []
        
        for trial in [0...t] by 1
            z   = []
            zx  = []
            
            for b in [0...@noBets] by 1
                r = Math.random()
                bet = @calculatedBets[b]
                if r < bet.prob
                    z[b]    = bet.odds - 1
                    zx[b]   = (bet.odds - 1) * @realKellyStakes[b]
                else
                    z[b]    = -1.0
                    zx[b]   = -@realKellyStakes[b]
        
            jointPlusMinus[trial]       = @sum zx
            rs[trial]                   = jointPlusMinus[trial] + 1
            growthContribution[trial]   = Math.log(rs[trial]) / trials
            for b in [0...@noBets] by 1
                zOver[trial] = [] unless zOver[trial]
                zOver[trial][b] = z[b] / (trials * rs[trial])
        for b in [0...@noBets] by 1
            slope[b] = 0
            for t in [0...trials] by 1
                slope[b] += zOver[t][b]
        
        slope 
    
    calculateKelly : -> 
        @singleKellyStakes = @calcSingleKellyStakes(@calculatedBets, 1, 0)
        @realKellyStakes = @normaliseKelly(@singleKellyStakes)
        lastRate    = []
        rate        = []
        step        = []
        update      = []
        proposed    = []
        lastSlope   = []
        for run in [0...runs] by 1

            slope       = @slopes(trials, @noBets)
            
            for b in [0...@noBets] by 1
                rate[b]             = if run is 0 then 1 else @learningRate(lastSlope[b], slope[b], lastSlope[b]) 
                step[b]             = @singleKellyStakes[b] - @realKellyStakes[b]
                update[b]           = @sign(slope[b]) * rate[b] * step[b]
                proposed[b]         = @realKellyStakes[b] + update[b]
            
            totalProposedStake  = @sum proposed
            rescaleFactor       = if totalProposedStake > 1 then (0.99 / totalProposedStake) else 1
            
            for b in [0...@noBets] by 1
                @realKellyStakes[b] = if proposed[b] < 0 then 0 else (proposed[b] * rescaleFactor)  
            
            lastRate = rate
            lastSlope = slope

        if @kellyMultiplier isnt 1
            for b in [0...@noBets] by 1
                @realKellyStakes[b] = @kellyMultiplier * @realKellyStakes[b] 

        @realKellyStakes
            
class ExhaustiveKelly extends Kelly
    constructor : (@kellyMultiplier, @minEdge, @calculatedBets=[]) ->
        @realKellyStakes
    
    calcSingleKellyStakes: (calculatedBets, kellyMultiplier, minEdge) ->
        if calculatedBets then calculatedBets.map (bet) -> 
            if not minEdge or bet.edge >= minEdge 
                Math.max(
                    (Math.pow(((bet.odds - 1) * bet.prob), kellyMultiplier) - Math.pow((1 - bet.prob), kellyMultiplier)) / (Math.pow(((bet.odds - 1) * bet.prob), kellyMultiplier) + (bet.odds - 1) * Math.pow((1 - bet.prob), kellyMultiplier))
                    ,0) 
            else 
                0
    
    bitImp : (a, b) ->
        ~a | b
    
    toRadix : (n, radix) ->
      HexN = ""
      Q = Math.floor(Math.abs(n))
      R = undefined
      loop
        R = Q % radix
        HexN = "0123456789abcdefghijklmnopqrstuvwxyz".charAt(R) + HexN
        Q = (Q - R) / radix
        break  if Q is 0
      (if (n < 0) then "-" + HexN else HexN)

    parlaySize : (n) ->
        @toRadix(n, 2).split('').reduce (t, s) -> parseInt(t) + parseInt(s)
            
    calculateKelly : ->
        singles             = calculatedBets.length
        bets                = Math.pow(2,  singles) - 1
        singleKellyStakes   = []
        parlayNames         = []
        parlayMaps          = []
        realKellyStakes    = []
        
        singleKellyStakes = @calcSingleKellyStakes @calculatedBets, @kellyMultiplier, @minEdge
    
        for i in [0...singles] by 1
            parlayMaps[i] = []
        
        for i in [1..bets] by 1
            pSize = parseInt(@parlaySize(i))
            parlayMaps[pSize - 1].push i
        
        for s in [singles..1] by -1
            limit = parlayMaps[s - 1].length
            for i in [0...limit] by 1
                parlayNumber = parlayMaps[s - 1][i]
                realKellyStakes[parlayNumber] = 1
                parlayNames[parlayNumber] = ""
                
                for k in [0...singles] by 1
                    if @bitImp(Math.pow(2, k), parlayNumber) is -1
                        realKellyStakes[parlayNumber] *= singleKellyStakes[k]
                        parlayNames[parlayNumber] += ((k + 1) + "+")
                
                for ss in [(s + 1)..singles] by 1
                    ssLimit = parlayMaps[ss - 1].length
                    for ii in [0...ssLimit] by 1
                        pp = parlayMaps[ss - 1][ii]
                        if @bitImp(parlayNumber, pp) is -1
                            realKellyStakes[parlayNumber] -= realKellyStakes[pp]        
        realKellyStakes.slice(0, singles)