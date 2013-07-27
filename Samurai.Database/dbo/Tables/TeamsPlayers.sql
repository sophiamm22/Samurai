CREATE TABLE [dbo].[TeamsPlayers] (
    [TeamPlayerID_pk] INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (MAX) NOT NULL,
    [FirstName]       NVARCHAR (MAX) NULL,
    [Slug]            NVARCHAR (MAX) NOT NULL,
    [ExternalID_efk]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.TeamsPlayers] PRIMARY KEY CLUSTERED ([TeamPlayerID_pk] ASC)
);

