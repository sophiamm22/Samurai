CREATE TABLE [dbo].[BookmakerExternalSourceAlias] (
    [BookmakerExternalSourceAlias_pk] INT            IDENTITY (1, 1) NOT NULL,
    [ExternalSourceID_fk]             INT            NOT NULL,
    [BookmakerID_fk]                  INT            NOT NULL,
    [Alias]                           NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_dbo.BookmakerExternalSourceAlias] PRIMARY KEY CLUSTERED ([BookmakerExternalSourceAlias_pk] ASC),
    CONSTRAINT [FK_dbo.BookmakerExternalSourceAlias_dbo.Bookmakers_BookmakerID_fk] FOREIGN KEY ([BookmakerID_fk]) REFERENCES [dbo].[Bookmakers] ([BookmakerID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.BookmakerExternalSourceAlias_dbo.ExternalSources_ExternalSourceID_fk] FOREIGN KEY ([ExternalSourceID_fk]) REFERENCES [dbo].[ExternalSources] ([ExternalSourceID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_BookmakerID_fk]
    ON [dbo].[BookmakerExternalSourceAlias]([BookmakerID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalSourceID_fk]
    ON [dbo].[BookmakerExternalSourceAlias]([ExternalSourceID_fk] ASC);

