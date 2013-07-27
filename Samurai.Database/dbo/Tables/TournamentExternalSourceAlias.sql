CREATE TABLE [dbo].[TournamentExternalSourceAlias] (
    [TournamentExternalSourceAlias_pk] INT            IDENTITY (1, 1) NOT NULL,
    [ExternalSourceID_fk]              INT            NOT NULL,
    [TournamentID_fk]                  INT            NOT NULL,
    [Alias]                            NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_dbo.TournamentExternalSourceAlias] PRIMARY KEY CLUSTERED ([TournamentExternalSourceAlias_pk] ASC),
    CONSTRAINT [FK_dbo.TournamentExternalSourceAlias_dbo.ExternalSources_ExternalSourceID_fk] FOREIGN KEY ([ExternalSourceID_fk]) REFERENCES [dbo].[ExternalSources] ([ExternalSourceID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.TournamentExternalSourceAlias_dbo.Tournaments_TournamentID_fk] FOREIGN KEY ([TournamentID_fk]) REFERENCES [dbo].[Tournaments] ([TournamentID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_TournamentID_fk]
    ON [dbo].[TournamentExternalSourceAlias]([TournamentID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalSourceID_fk]
    ON [dbo].[TournamentExternalSourceAlias]([ExternalSourceID_fk] ASC);

