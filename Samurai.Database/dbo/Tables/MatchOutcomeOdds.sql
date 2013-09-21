CREATE TABLE [dbo].[MatchOutcomeOdds] (
    [MatchOutcomeOddID_pk]                  INT             IDENTITY (1, 1) NOT NULL,
    [ExternalSourceID_fk]                   INT             NOT NULL,
    [MatchOutcomeProbabilitiesInMatchID_fk] INT             NOT NULL,
    [BookmakerID_fk]                        INT             NOT NULL,
    [TimeStamp]                             DATETIME        NOT NULL,
    [Odd]                                   DECIMAL (10, 4) NOT NULL,
    [ClickThroughURL]                       NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_dbo.MatchOutcomeOdds] PRIMARY KEY CLUSTERED ([MatchOutcomeOddID_pk] ASC),
    CONSTRAINT [FK_dbo.MatchOutcomeOdds_dbo.Bookmakers_BookmakerID_fk] FOREIGN KEY ([BookmakerID_fk]) REFERENCES [dbo].[Bookmakers] ([BookmakerID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MatchOutcomeOdds_dbo.ExternalSources_ExternalSourceID_fk] FOREIGN KEY ([ExternalSourceID_fk]) REFERENCES [dbo].[ExternalSources] ([ExternalSourceID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MatchOutcomeOdds_dbo.MatchOutcomeProbabilitiesInMatch_MatchOutcomeProbabilitiesInMatchID_fk] FOREIGN KEY ([MatchOutcomeProbabilitiesInMatchID_fk]) REFERENCES [dbo].[MatchOutcomeProbabilitiesInMatch] ([MatchOutcomeProbabilitiesInMatchID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_MatchOutcomeProbabilitiesInMatchID_fk]
    ON [dbo].[MatchOutcomeOdds]([MatchOutcomeProbabilitiesInMatchID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalSourceID_fk]
    ON [dbo].[MatchOutcomeOdds]([ExternalSourceID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BookmakerID_fk]
    ON [dbo].[MatchOutcomeOdds]([BookmakerID_fk] ASC);

