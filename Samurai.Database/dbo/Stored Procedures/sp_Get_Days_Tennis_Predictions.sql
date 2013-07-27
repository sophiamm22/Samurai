CREATE PROCEDURE [dbo].[sp_Get_Days_Tennis_Predictions]
		
		@date DATE = '20130724'
AS
	SET NOCOUNT ON
		
	SELECT
			m.MatchID_pk
		, t.TournamentName
		, YEAR(MatchDate) AS Year
		, su.SurfaceName AS Surface
		, c.CompetitionName AS Series
		,	m.MatchDate
		,	ta.Name AS PlayerASurname
		,	ta.FirstName AS PlayerAFirstName
		, tb.Name AS PlayerBSurname
		, tb.FirstName AS PlayerBFirstName
		,	ts.PlayerAGames
		, ts.PlayerBGames
		, mpPlayerA.MatchOutcomeProbability AS PlayerAProbability
		, mpPlayerB.MatchOutcomeProbability AS PlayerBProbability
		, COALESCE(CONVERT(NVARCHAR(2), so.TeamAScore) + '-' + CONVERT(NVARCHAR(2), so.TeamBScore), 'not played') AS Score
		, [3-0]
		, [3-1]
		, [3-2]
		, [2-3]
		, [1-3]
		, [0-3]
		, [2-0]
		, [2-1]
		, [1-2]
		, [0-2]
	FROM
		Matches m
		INNER JOIN TeamsPlayers ta ON m.TeamAID_fk = ta.TeamPlayerID_pk
		INNER JOIN TeamsPlayers tb ON m.TeamBID_fk = tb.TeamPlayerID_pk
		LEFT JOIN TennisPredictionStats ts ON m.MatchID_pk = ts.MatchID_fk
		LEFT JOIN ObservedOutcomes oo ON m.MatchID_pk = oo.MatchID_fk
		LEFT JOIN ScoreOutcomes so ON so.ScoreOutcomeID_pk = oo.ScoreOutcomeID_fk
		LEFT JOIN OutcomeComments oc ON oc.OutcomeCommentID_pk = oo.OutcomeCommentID_fk
		LEFT JOIN MatchOutcomes moObs ON moObs.MatchOutcomeID_pk = so.MatchOutcomeID_fk
		INNER JOIN TournamentEvents te ON m.TournamentEventID_fk = te.TournamentEventID_pk
		INNER JOIN Surfaces su ON te.SurfaceID_fk = su.SurfaceID_pk
		INNER JOIN Tournaments t ON te.TournamentID_fk = t.TournamentID_pk
		INNER JOIN Competitions c ON t.CompetitionID = c.CompetitionID_pk
		INNER JOIN Sports s ON c.SportID_fk = s.SportID_pk
		INNER JOIN MatchOutcomeProbabilitiesInMatch mpPlayerA 
			ON m.MatchID_pk = mpPlayerA.MatchID_fk
			AND mpPlayerA.MatchOutcomeID_fk = 3
		INNER JOIN MatchOutcomeProbabilitiesInMatch mpPlayerB
			ON m.MatchID_pk = mpPlayerB.MatchID_fk
			AND mpPlayerB.MatchOutcomeID_fk = 2
		INNER JOIN
			(
				SELECT
						MatchID_fk
					,[3-0]
					,[3-1]
					,[3-2]
					,[2-3]
					,[1-3]
					,[0-3]
					,[2-0]
					,[2-1]
					,[1-2]
					,[0-2]
				FROM
				(
					SELECT
							CONVERT(NVARCHAR(2), TeamAScore) + '-' + CONVERT(NVARCHAR(2), TeamBScore) AS Score
						,	ScoreOutcomeProbability
						, MatchID_fk
					FROM
						ScoreOutcomeProbabilitiesInMatch sp
						INNER JOIN ScoreOutcomes so ON sp.ScoreOutcomeID_fk = so.ScoreOutcomeID_pk
						INNER JOIN Matches m ON m.MatchID_pk = sp.MatchID_fk
					WHERE
						CONVERT(date, m.MatchDate) = @date
				 ) ps
				PIVOT 
				(
					SUM(ScoreOutcomeProbability)
					FOR Score IN ([3-0],[3-1],[3-2],[2-3],[1-3],[0-3],[2-0],[2-1],[1-2],[0-2])
				) AS piv
			) AS ScoreLineProbabilities
			ON m.MatchID_pk = ScoreLineProbabilities.MatchID_fk
			
		WHERE 
			s.SportName = 'Tennis'
			AND CONVERT(date, m.MatchDate) = @date
		ORDER BY
				m.MatchDate