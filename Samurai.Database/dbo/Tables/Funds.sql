CREATE TABLE [dbo].[Funds] (
    [FundID_pk]            INT             IDENTITY (1, 1) NOT NULL,
    [FundName]             NVARCHAR (MAX)  NOT NULL,
    [Bank]                 DECIMAL (18, 2) NOT NULL,
    [Turnover]             DECIMAL (18, 2) NOT NULL,
    [Revenue]              DECIMAL (18, 2) NOT NULL,
    [EdgeRequiredOverride] DECIMAL (10, 4) NOT NULL,
    [KellyMultiplier]      DECIMAL (5, 4)  NOT NULL,
    CONSTRAINT [PK_dbo.Funds] PRIMARY KEY CLUSTERED ([FundID_pk] ASC)
);

