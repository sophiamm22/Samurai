CREATE TABLE [dbo].[Bookmakers] (
    [BookmakerID_pk]         INT             IDENTITY (1, 1) NOT NULL,
    [BookmakerName]          NVARCHAR (MAX)  NOT NULL,
    [Slug]                   NVARCHAR (MAX)  NOT NULL,
    [BookmakerURL]           NVARCHAR (MAX)  NOT NULL,
    [BookmakerNotes]         NVARCHAR (MAX)  NULL,
    [IsExchange]             BIT             NOT NULL,
    [CurrentCommission]      DECIMAL (18, 2) NULL,
    [BookmakerBalance]       DECIMAL (18, 2) NULL,
    [OddsCheckerShortID_efk] NVARCHAR (2)    NULL,
    [Priority]               INT             NOT NULL,
    CONSTRAINT [PK_dbo.Bookmakers] PRIMARY KEY CLUSTERED ([BookmakerID_pk] ASC)
);

