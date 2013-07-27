CREATE TABLE [dbo].[MatchOutcomes] (
    [MatchOutcomeID_pk] INT           IDENTITY (1, 1) NOT NULL,
    [MatchOutcome]      NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_dbo.MatchOutcomes] PRIMARY KEY CLUSTERED ([MatchOutcomeID_pk] ASC)
);

