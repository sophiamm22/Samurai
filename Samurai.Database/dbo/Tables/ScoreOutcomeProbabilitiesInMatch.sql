CREATE TABLE [dbo].[ScoreOutcomeProbabilitiesInMatch] (
    [ScoreOutcomeProbabilitiesInMatchID_pk] INT             IDENTITY (1, 1) NOT NULL,
    [MatchID_fk]                            INT             NOT NULL,
    [ScoreOutcomeID_fk]                     INT             NOT NULL,
    [ScoreOutcomeProbability]               DECIMAL (10, 9) NOT NULL,
    CONSTRAINT [PK_dbo.ScoreOutcomeProbabilitiesInMatch] PRIMARY KEY CLUSTERED ([ScoreOutcomeProbabilitiesInMatchID_pk] ASC),
    CONSTRAINT [FK_dbo.ScoreOutcomeProbabilitiesInMatch_dbo.Matches_MatchID_fk] FOREIGN KEY ([MatchID_fk]) REFERENCES [dbo].[Matches] ([MatchID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ScoreOutcomeProbabilitiesInMatch_dbo.ScoreOutcomes_ScoreOutcomeID_fk] FOREIGN KEY ([ScoreOutcomeID_fk]) REFERENCES [dbo].[ScoreOutcomes] ([ScoreOutcomeID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ScoreOutcomeID_fk]
    ON [dbo].[ScoreOutcomeProbabilitiesInMatch]([ScoreOutcomeID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MatchID_fk]
    ON [dbo].[ScoreOutcomeProbabilitiesInMatch]([MatchID_fk] ASC);

