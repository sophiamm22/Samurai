CREATE TABLE [dbo].[Matches] (
    [MatchID_pk]           INT      IDENTITY (1, 1) NOT NULL,
    [TournamentEventID_fk] INT      NOT NULL,
    [MatchDate]            DATETIME NOT NULL,
    [TeamAID_fk]           INT      NOT NULL,
    [TeamBID_fk]           INT      NOT NULL,
    [EligibleForBetting]   BIT      NOT NULL,
    [IKTSGameWeek]         INT      NULL,
    [InPlay]               BIT      DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.Matches] PRIMARY KEY CLUSTERED ([MatchID_pk] ASC),
    CONSTRAINT [FK_dbo.Matches_dbo.TeamsPlayers_TeamAID_fk] FOREIGN KEY ([TeamAID_fk]) REFERENCES [dbo].[TeamsPlayers] ([TeamPlayerID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.Matches_dbo.TeamsPlayers_TeamBID_fk] FOREIGN KEY ([TeamBID_fk]) REFERENCES [dbo].[TeamsPlayers] ([TeamPlayerID_pk]),
    CONSTRAINT [FK_dbo.Matches_dbo.TournamentEvents_TournamentEventID_fk] FOREIGN KEY ([TournamentEventID_fk]) REFERENCES [dbo].[TournamentEvents] ([TournamentEventID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_TeamAID_fk]
    ON [dbo].[Matches]([TeamAID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TeamBID_fk]
    ON [dbo].[Matches]([TeamBID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TournamentEventID_fk]
    ON [dbo].[Matches]([TournamentEventID_fk] ASC);

