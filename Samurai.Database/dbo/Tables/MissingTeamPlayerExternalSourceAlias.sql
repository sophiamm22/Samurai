CREATE TABLE [dbo].[MissingTeamPlayerExternalSourceAlias] (
    [MissingTeamPlayerExternalSourceAliasID_pk] INT            IDENTITY (1, 1) NOT NULL,
    [TeamPlayer]                                NVARCHAR (MAX) NOT NULL,
    [ExternalSourceID_fk]                       INT            NOT NULL,
    [TournamentID_fk]                           INT            NOT NULL,
    CONSTRAINT [PK_MissingTeamPlayerExternalSourceAlias] PRIMARY KEY CLUSTERED ([MissingTeamPlayerExternalSourceAliasID_pk] ASC),
    CONSTRAINT [FK_MissingTeamPlayerExternalSourceAlias_ExternalSources] FOREIGN KEY ([ExternalSourceID_fk]) REFERENCES [dbo].[ExternalSources] ([ExternalSourceID_pk]),
    CONSTRAINT [FK_MissingTeamPlayerExternalSourceAlias_Tournaments] FOREIGN KEY ([TournamentID_fk]) REFERENCES [dbo].[Tournaments] ([TournamentID_pk])
);

