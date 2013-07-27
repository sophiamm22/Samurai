CREATE PROCEDURE [dbo].[sp_Get_Days_Football_Predictions]
		
		@date DATE = '20130129'
AS
	SET NOCOUNT ON
	
	SELECT
			m.MatchID_pk
		, t.TournamentName
		, YEAR(MatchDate) AS Year
		,	m.MatchDate AS MatchDate
		,	ta.Name AS TeamA
		, tb.Name AS TeamB
		, mpTeamA.MatchOutcomeProbability AS TeamAProbability
		, mpDraw.MatchOutcomeProbability AS Draw
		, mpTeamB.MatchOutcomeProbability AS TeamBProbability
		, COALESCE(CONVERT(NVARCHAR(2), so.TeamAScore) + '-' + CONVERT(NVARCHAR(2), so.TeamBScore), 'not played') AS Score

	FROM
		Matches m
		INNER JOIN TeamsPlayers ta ON m.TeamAID_fk = ta.TeamPlayerID_pk
		INNER JOIN TeamsPlayers tb ON m.TeamBID_fk = tb.TeamPlayerID_pk
		LEFT JOIN ObservedOutcomes oo ON m.MatchID_pk = oo.MatchID_fk
		LEFT JOIN ScoreOutcomes so ON so.ScoreOutcomeID_pk = oo.ScoreOutcomeID_fk
		LEFT JOIN OutcomeComments oc ON oc.OutcomeCommentID_pk = oo.OutcomeCommentID_fk
		LEFT JOIN MatchOutcomes moObs ON moObs.MatchOutcomeID_pk = so.MatchOutcomeID_fk
		INNER JOIN TournamentEvents te ON m.TournamentEventID_fk = te.TournamentEventID_pk
		INNER JOIN Tournaments t ON te.TournamentID_fk = t.TournamentID_pk
		INNER JOIN Competitions c ON t.CompetitionID = c.CompetitionID_pk
		INNER JOIN Sports s ON c.SportID_fk = s.SportID_pk
		INNER JOIN MatchOutcomeProbabilitiesInMatch mpTeamA 
			ON m.MatchID_pk = mpTeamA.MatchID_fk
			AND mpTeamA.MatchOutcomeID_fk = 3
		INNER JOIN MatchOutcomeProbabilitiesInMatch mpTeamB
			ON m.MatchID_pk = mpTeamB.MatchID_fk
			AND mpTeamB.MatchOutcomeID_fk = 2
		INNER JOIN MatchOutcomeProbabilitiesInMatch mpDraw
			ON m.MatchID_pk = mpDraw.MatchID_fk
			AND mpDraw.MatchOutcomeID_fk = 1
					
		WHERE 
			s.SportName = 'Football'
			AND CONVERT(date, m.MatchDate) = @date
		ORDER BY
				m.MatchDate