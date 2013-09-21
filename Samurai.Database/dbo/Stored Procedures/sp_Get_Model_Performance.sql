CREATE PROCEDURE [dbo].[sp_Get_Model_Performance]

		@sport NVARCHAR(MAX) = 'Tennis'
	,	@startDate DATE = '20130101'
	, @endDate DATE = '20131231'
	
AS
	SET NOCOUNT ON
	
	SELECT
			t.TournamentName
		, su.SurfaceName AS Surface
		, c.CompetitionName AS Series
		,	CONVERT(VARCHAR(10), m.MatchDate, 105) AS MatchDate
		,	ta.Name AS PlayerASurname
		,	ta.FirstName AS PlayerAFirstName
		, tb.Name AS PlayerBSurname
		, tb.FirstName AS PlayerBFirstName
		,	ts.PlayerAGames
		, ts.PlayerBGames
		, mo.MatchOutcome AS ExpectedMatchOutcome
		, mp.MatchOutcomeProbability
		, BestOdds.BookmakerName
		, BestOdds.Source
		, BestOdds.OddAfterCommission
		, mp.MatchOutcomeProbability * BestOdds.Odd - 1 AS Edge
		, COALESCE(moObs.MatchOutcome, 'Not recorded') AS ObservedMatchOutcome
		, so.TeamAScore
		, so.TeamBScore
		, COALESCE(oc.Comment, 'Not recorded') AS MatchComment
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
		INNER JOIN MatchOutcomeProbabilitiesInMatch mp ON m.MatchID_pk = mp.MatchID_fk
		INNER JOIN MatchOutcomes mo ON mp.MatchOutcomeID_fk = mo.MatchOutcomeID_pk
		CROSS APPLY (
			SELECT TOP 1
					mInner.MatchID_pk
				,	moInner.MatchOutcomeID_pk
				, moInner.MatchOutcome
				, esInner.ExternalSourceID_pk
				, esInner.Source
				, bmInner.BookmakerName
				, bmInner.BookmakerID_pk
				, bmInner.CurrentCommission
				, bmInner.Priority
				, mooInner.MatchOutcomeOddID_pk
				, mooInner.TimeStamp
				, mooInner.Odd
				, (mooInner.Odd - 1) * (1 - COALESCE(bmInner.CurrentCommission, 0)) + 1 AS OddAfterCommission
				, 4 - 2 *(esInner.ExternalSourceID_pk % 2) + esInner.ExternalSourceID_pk AS SourcePriority
			FROM
				Matches mInner
				INNER JOIN MatchOutcomeProbabilitiesInMatch mpInner ON mInner.MatchID_pk = mpInner.MatchID_fk
				INNER JOIN MatchOutcomes moInner ON mpInner.MatchOutcomeID_fk = moInner.MatchOutcomeID_pk
				INNER JOIN MatchOutcomeOdds mooInner ON mooInner.MatchOutcomeProbabilitiesInMatchID_fk = mpInner.MatchOutcomeProbabilitiesInMatchID_pk
				INNER JOIN ExternalSources esInner ON esInner.ExternalSourceID_pk = mooInner.ExternalSourceID_fk
				INNER JOIN Bookmakers bmInner ON bmInner.BookmakerID_pk = mooInner.BookmakerID_fk
				INNER JOIN 
				(
					SELECT 
							m3.MatchID_pk
						,	mo3.MatchOutcomeID_pk
						, mo3.MatchOutcome
						, bm3.BookmakerID_pk
						, bm3.BookmakerName
						, bm3.Priority
						, es3.ExternalSourceID_pk
						, es3.Source
						, 4 - 2 *(es3.ExternalSourceID_pk % 2) + es3.ExternalSourceID_pk AS SourcePriority
						, MAX(moo3.TimeStamp) AS LatestTimeStamp
					FROM
						Matches m3
						INNER JOIN MatchOutcomeProbabilitiesInMatch mp3 ON m3.MatchID_pk = mp3.MatchID_fk
						INNER JOIN MatchOutcomes mo3 ON mp3.MatchOutcomeID_fk = mo3.MatchOutcomeID_pk
						INNER JOIN MatchOutcomeOdds moo3 ON moo3.MatchOutcomeProbabilitiesInMatchID_fk = mp3.MatchOutcomeProbabilitiesInMatchID_pk
						INNER JOIN ExternalSources es3 ON es3.ExternalSourceID_pk = moo3.ExternalSourceID_fk
						INNER JOIN Bookmakers bm3 ON bm3.BookmakerID_pk = moo3.BookmakerID_fk
					WHERE
						bm3.Priority <> 0
					GROUP BY
							m3.MatchID_pk
						,	mo3.MatchOutcomeID_pk
						, mo3.MatchOutcome
						, bm3.BookmakerID_pk
						, bm3.BookmakerName
						, bm3.Priority	
						, es3.ExternalSourceID_pk
						, es3.Source	
					) LatestOdds 
					ON LatestOdds.MatchID_pk = mInner.MatchID_pk
					AND LatestOdds.MatchOutcomeID_pk = moInner.MatchOutcomeID_pk
					AND LatestOdds.BookmakerID_pk = bmInner.BookmakerID_pk
					AND LatestOdds.ExternalSourceID_pk = esInner.ExternalSourceID_pk
					AND LatestOdds.LatestTimeStamp = mooInner.TimeStamp
			WHERE 
				m.MatchID_pk = mInner.MatchID_pk
				AND mo.MatchOutcomeID_pk = moInner.MatchOutcomeID_pk
			ORDER BY
					(mooInner.Odd - 1) * (1 - COALESCE(bmInner.CurrentCommission, 0)) + 1 DESC
				, bmInner.Priority
				, 4 - 2 *(esInner.ExternalSourceID_pk % 2) + esInner.ExternalSourceID_pk
		) AS BestOdds
	WHERE 
		s.SportName = @sport
		AND moObs.MatchOutcome IS NOT NULL
		AND oc.Comment = 'Completed'
		AND mp.MatchOutcomeProbability * BestOdds.Odd - 1 >= 0
		AND CONVERT(date, m.MatchDate) >= @startDate
		AND CONVERT(date, m.MatchDate) <= @endDate
	ORDER BY
			m.MatchDate
		,	MatchOutcomeProbabilitiesInMatchID_pk