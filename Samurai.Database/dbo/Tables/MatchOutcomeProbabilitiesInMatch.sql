CREATE TABLE [dbo].[MatchOutcomeProbabilitiesInMatch] (
    [MatchOutcomeProbabilitiesInMatchID_pk] INT             IDENTITY (1, 1) NOT NULL,
    [MatchID_fk]                            INT             NOT NULL,
    [MatchOutcomeID_fk]                     INT             NOT NULL,
    [MatchOutcomeProbability]               DECIMAL (10, 9) NOT NULL,
    CONSTRAINT [PK_dbo.MatchOutcomeProbabilitiesInMatch] PRIMARY KEY CLUSTERED ([MatchOutcomeProbabilitiesInMatchID_pk] ASC),
    CONSTRAINT [FK_dbo.MatchOutcomeProbabilitiesInMatch_dbo.Matches_MatchID_fk] FOREIGN KEY ([MatchID_fk]) REFERENCES [dbo].[Matches] ([MatchID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MatchOutcomeProbabilitiesInMatch_dbo.MatchOutcomes_MatchOutcomeID_fk] FOREIGN KEY ([MatchOutcomeID_fk]) REFERENCES [dbo].[MatchOutcomes] ([MatchOutcomeID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_MatchOutcomeID_fk]
    ON [dbo].[MatchOutcomeProbabilitiesInMatch]([MatchOutcomeID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MatchID_fk]
    ON [dbo].[MatchOutcomeProbabilitiesInMatch]([MatchID_fk] ASC);

