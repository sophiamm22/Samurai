CREATE PROCEDURE [dbo].[sp_Get_Days_Best_Odds_For_Sport]

		@sport NVARCHAR(MAX) = 'Tennis'
	,	@date DATE = '20130724'

AS
	SET NOCOUNT ON
	
	SELECT
			m.MatchID_pk
		, s.SportName
		, BestOdds.MatchOutcome
		, BestOdds.Odd
		, BestOdds.CurrentCommission
		, BestOdds.OddAfterCommission
		, BestOdds.BookmakerName
		, BestOdds.Source
		, BestOdds.ClickThroughURL
		, BestOdds.TimeStamp
		, BestOdds.Priority
	FROM
		Matches m
		INNER JOIN MatchOutcomeProbabilitiesInMatch mp ON m.MatchID_pk = mp.MatchID_fk	
		INNER JOIN MatchOutcomes mo ON mp.MatchOutcomeID_fk = mo.MatchOutcomeID_pk
		INNER JOIN TournamentEvents te ON m.TournamentEventID_fk = te.TournamentEventID_pk
		INNER JOIN Tournaments t ON te.TournamentID_fk = t.TournamentID_pk
		INNER JOIN Competitions c ON t.CompetitionID = c.CompetitionID_pk
		INNER JOIN Sports s ON c.SportID_fk = s.SportID_pk
		
		CROSS APPLY (
			SELECT TOP 1
					m2.MatchID_pk
				,	mo2.MatchOutcomeID_pk
				, mo2.MatchOutcome
				, es2.ExternalSourceID_pk
				, es2.Source
				, bm2.BookmakerName
				, bm2.BookmakerID_pk
				, bm2.CurrentCommission
				, bm2.Priority
				, moo2.MatchOutcomeOddID_pk
				, moo2.TimeStamp
				, moo2.Odd
				, (moo2.Odd - 1) * (1 - COALESCE(bm2.CurrentCommission, 0)) + 1 AS OddAfterCommission
				, 4 - 2 *(es2.ExternalSourceID_pk % 2) + es2.ExternalSourceID_pk AS SourcePriority
				, moo2.ClickThroughURL
			FROM
				Matches m2
				INNER JOIN MatchOutcomeProbabilitiesInMatch mp2 ON m2.MatchID_pk = mp2.MatchID_fk
				INNER JOIN MatchOutcomes mo2 ON mp2.MatchOutcomeID_fk = mo2.MatchOutcomeID_pk
				INNER JOIN MatchOutcomeOdds moo2 ON moo2.MatchOutcomeProbabilitiesInMatchID_fk = mp2.MatchOutcomeProbabilitiesInMatchID_pk
				INNER JOIN ExternalSources es2 ON es2.ExternalSourceID_pk = moo2.ExternalSourceID_fk
				INNER JOIN Bookmakers bm2 ON bm2.BookmakerID_pk = moo2.BookmakerID_fk
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
					ON LatestOdds.MatchID_pk = m2.MatchID_pk
					AND LatestOdds.MatchOutcomeID_pk = mo2.MatchOutcomeID_pk
					AND LatestOdds.BookmakerID_pk = bm2.BookmakerID_pk
					AND LatestOdds.ExternalSourceID_pk = es2.ExternalSourceID_pk
					AND LatestOdds.LatestTimeStamp = moo2.TimeStamp
			WHERE 
				m.MatchID_pk = m2.MatchID_pk
				AND mo.MatchOutcomeID_pk = mo2.MatchOutcomeID_pk
			ORDER BY
					(moo2.Odd - 1) * (1 - COALESCE(bm2.CurrentCommission, 0)) + 1 DESC
				, bm2.Priority
				, 4 - 2 *(es2.ExternalSourceID_pk % 2) + es2.ExternalSourceID_pk
		) AS BestOdds
	WHERE 
		s.SportName = @sport
		AND CONVERT(date, m.MatchDate) = @date
	ORDER BY
			m.MatchID_pk
		,	MatchOutcomeProbabilitiesInMatchID_pk