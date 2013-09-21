CREATE TABLE [dbo].[BettingPAndL] (
    [BettingPAndLID_pk]        INT             IDENTITY (1, 1) NOT NULL,
    [MatchOutcomeOddID_fk]     INT             NOT NULL,
    [Cancelled]                BIT             NOT NULL,
    [Overround]                FLOAT (53)      NOT NULL,
    [Stake]                    DECIMAL (18, 2) NULL,
    [CommisionPaid]            DECIMAL (18, 2) NULL,
    [ProfitLossAfterCommision] DECIMAL (18, 2) NULL,
    [OddsTakenOverride]        DECIMAL (10, 4) NULL,
    CONSTRAINT [PK_dbo.BettingPAndL] PRIMARY KEY CLUSTERED ([BettingPAndLID_pk] ASC),
    CONSTRAINT [FK_dbo.BettingPAndL_dbo.MatchOutcomeOdds_MatchOutcomeOddID_fk] FOREIGN KEY ([MatchOutcomeOddID_fk]) REFERENCES [dbo].[MatchOutcomeOdds] ([MatchOutcomeOddID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_MatchOutcomeOddID_fk]
    ON [dbo].[BettingPAndL]([MatchOutcomeOddID_fk] ASC);

