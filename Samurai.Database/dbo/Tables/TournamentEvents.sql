CREATE TABLE [dbo].[TournamentEvents] (
    [TournamentEventID_pk] INT            IDENTITY (1, 1) NOT NULL,
    [TournamentID_fk]      INT            NOT NULL,
    [EventName]            NVARCHAR (MAX) NOT NULL,
    [Slug]                 NVARCHAR (MAX) NOT NULL,
    [StartDate]            DATETIME       NOT NULL,
    [EndDate]              DATETIME       NULL,
    [TournamentInProgress] BIT            NOT NULL,
    [TournamentCompleted]  BIT            NOT NULL,
    [SurfaceID_fk]         INT            NULL,
    CONSTRAINT [PK_dbo.TournamentEvents] PRIMARY KEY CLUSTERED ([TournamentEventID_pk] ASC),
    CONSTRAINT [FK_dbo.TournamentEvents_dbo.Surfaces_SurfaceID] FOREIGN KEY ([SurfaceID_fk]) REFERENCES [dbo].[Surfaces] ([SurfaceID_pk]),
    CONSTRAINT [FK_dbo.TournamentEvents_dbo.Tournaments_TournamentID_fk] FOREIGN KEY ([TournamentID_fk]) REFERENCES [dbo].[Tournaments] ([TournamentID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_SurfaceID]
    ON [dbo].[TournamentEvents]([SurfaceID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TournamentID_fk]
    ON [dbo].[TournamentEvents]([TournamentID_fk] ASC);

