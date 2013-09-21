CREATE TABLE [dbo].[TeamPlayerExternalSourceAlias] (
    [TeamPlayerExternalSourceAliasID_pk] INT            IDENTITY (1, 1) NOT NULL,
    [ExternalSourceID_fk]                INT            NOT NULL,
    [TeamPlayerID_fk]                    INT            NOT NULL,
    [Alias]                              NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_dbo.TeamPlayerExternalSourceAlias] PRIMARY KEY CLUSTERED ([TeamPlayerExternalSourceAliasID_pk] ASC),
    CONSTRAINT [FK_dbo.TeamPlayerExternalSourceAlias_dbo.ExternalSources_ExternalSourceID_fk] FOREIGN KEY ([ExternalSourceID_fk]) REFERENCES [dbo].[ExternalSources] ([ExternalSourceID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.TeamPlayerExternalSourceAlias_dbo.TeamsPlayers_TeamPlayerID_fk] FOREIGN KEY ([TeamPlayerID_fk]) REFERENCES [dbo].[TeamsPlayers] ([TeamPlayerID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_TeamPlayerID_fk]
    ON [dbo].[TeamPlayerExternalSourceAlias]([TeamPlayerID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalSourceID_fk]
    ON [dbo].[TeamPlayerExternalSourceAlias]([ExternalSourceID_fk] ASC);

