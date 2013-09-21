CREATE TABLE [dbo].[MissingBookmakerExternalSourceAlias] (
    [MissingBookmakerExternalSourceAliasID_pk] INT            IDENTITY (1, 1) NOT NULL,
    [ExternalSourceID_fk]                      INT            NOT NULL,
    [Bookmaker]                                NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_MissingBookmakerExternalSourceAlias] PRIMARY KEY CLUSTERED ([MissingBookmakerExternalSourceAliasID_pk] ASC),
    CONSTRAINT [FK_MissingBookmakerExternalSourceAlias_ExternalSources] FOREIGN KEY ([ExternalSourceID_fk]) REFERENCES [dbo].[ExternalSources] ([ExternalSourceID_pk])
);

