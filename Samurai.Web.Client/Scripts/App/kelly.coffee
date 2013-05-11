require ['jquery'], ($) ->
    $ ->
        Array::sum = () ->
          if this.length > 0
            this.reduce (x, y) -> x + y
          else
            0
        
        class CalculatedBet
            constructor: (@odds, @prob)->
                @edge = @odds * @prob - 1
        
        class Kelly
            constructor: (@kellyMultiplier, @minEdge, @calculatedBets={}) ->
                @realKellyStakes
            
            @singleKellyStakes: (calculatedBets, kellyMultiplier) ->
                calculatedBets.map (bet) -> 
                    if bet.edge >= @minEdge 
                        Math.max(
                            (Math.pow(((bet.odds - 1) * bet.prob), kellyMultiplier) - Math.pow((1 - bet.prob), kellyMultiplier)) / (Math.pow((bet.win * bet.prob), kellyMultiplier) + (bet.odds - 1) * Math.pow((1 - bet.prob), kellyMultiplier))
                            ,0) 
                    else 
                        0
        
        class WhitrowKelly extends Kelly
            trials              = 200
            runs                = 200
            learningSteps       = 300
            noBets              = @calculatedBets.length
            singleKellyStakes   = normaliseKelly(@singleKellyStakes(@calculatedBets, 1))
            
            normaliseKelly = singleKellyStakes ->
                totalStakes = singleKellyStakes.sum()
                singleKellyStakes.map (bet) ->
                    if totalStakes > 1 then bet / totalStakes else bet
            
            learningRate = (slopePrev, slope, ratePrevious) ->
                ratePrevious * (if Math.sign(slopePrev) is Math.sign(slop) then 1.05 else 0.95)
            
            slopes = (t, b) ->
                jointPlusMinus      = {}
                rs                  = {}
                growthContribution  = {}
                zOver               = {}
                slope               = {}
                
                for trial in [0...t]
                    z   = {}
                    zx  = {}
                    
                    for b in [0...noBets]
                        r = Math.random()
                        bet = @calculatedBets[b]
                        if r < bet.prob
                            z[b]    = bet.odds - 1
                            zx[b]   = (bet.odds - 1) * @realKellyStakes[b]
                        else
                            z[b]    = -1.0
                            zx[b]   = -@realKellyStakes[b]
                
                jointPlusMinus[trial]       = zx.sum()
                rs[trial]                   = jointPlusMinus[trial] + 1
                growthContribution[trial]   = Math.log(rs[trial]) / trials
                for b in [0...noBets]
                    slope[b] = (zOver.transpose())[b].sum()
                
                slope 
            
            calculateKelly: -> 
                for run in [0...runs]
                    lastRate    = {}
                    rate        = {}
                    step        = {}
                    update      = {}
                    proposed    = {}
                    lastSlope   = {}
                    slope       = slopes(trials, noBets)
                    
                    for b in [0...noBets]
                        rate[b]             = if not run then 1 else learningRate(lastSlope[b], slope[b], lastSlope[b]) #check this!
                        adjustedKellyStake  = singleKellyStakes[b]
                        step[b]             = singleKellyStakes[b] - adjustedKellyStake
                        update[b]           = Math.sign(slope[b]) * rate[b] * step[b]
                        proposed[b]         = adjustedKellyStake + update[b]
                        totalProposedStake  = proposed.sum()
                        rescaleFactor       = if totalProposedStake > 1 then 0.99 / totalProposedStake else 1
                    
                    for b in [0...noBets]
                        @realKellyStakes[b] = if proposed[b] < 0 then 0 else (proposed[b] * rescaleFactor)  
                    lastRate = rate
                    
                @realKellyStakes
                    
        class ExhaustiveKelly extends Kelly
        
            bitImp = (a, b) ->
                ~a | b
            
            toRadix = (n, radix) ->
              HexN = ""
              Q = Math.floor(Math.abs(n))
              R = undefined
              loop
                R = Q % radix
                HexN = "0123456789abcdefghijklmnopqrstuvwxyz".charAt(R) + HexN
                Q = (Q - R) / radix
                break  if Q is 0
              (if (n < 0) then "-" + HexN else HexN)

            parlaySize = (n) ->
                toRadix(n, 2).reduce (t, s) -> t or 0 + s
                
                
            calculateKelly: ->
                singles             = calculatedBets.length
                bets                = Math.pow(2,  singles)
                singleKellyStakes   = {}
                parlayNames         = {}
                parlayMaps          = {}
                @realKellyStakes    = {}
                
                singleKellyStakes = @singleKellyStakes @calculatedBets, @kellyMultiplier 
            
                for i in [1..bets]
                    parlayMaps[parlaySize(i) - 1].push i
                
                for s in [singles..1] by -1
                    limit = parlayMaps[s - 1].length
                    for i in [0...limit]
                        parlayNumber = parlayMap[s - 1][i]
                        @realKellyStakes[parlayNumber] = 1
                        parlayNames[parlayNumber] = ""
                        
                        for k in [0...singles]
                            if Math.pow(bitImp(2, k), parlayNumber) is -1
                                @realKellyStakes[parlayNumber] *= singleKellyStakes[k]
                                parlayNames[parlayNumber] += ((k + 1) + "+")
                        
                        for ss in [(s + 1)..singles]
                            ssLimit = parlayMaps[ss - 1].length
                            for ii in [0...ssLimit]
                                pp = parlayMaps[ss - 1][ii]
                                if bitImp parlayNumber , pp is -1
                                    @realKellyStakes[parlayNumber] -= @realKellyStakes[pp]
                
                @realKellyStakes