CREATE TABLE [dbo].[CompetitionsInFunds] (
    [FundID_fk]        INT NOT NULL,
    [CompetitionID_fk] INT NOT NULL,
    CONSTRAINT [PK_dbo.CompetitionsInFunds] PRIMARY KEY CLUSTERED ([FundID_fk] ASC, [CompetitionID_fk] ASC),
    CONSTRAINT [FK_dbo.CompetitionsInFunds_dbo.Competitions_CompetitionID_fk] FOREIGN KEY ([CompetitionID_fk]) REFERENCES [dbo].[Competitions] ([CompetitionID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.CompetitionsInFunds_dbo.Funds_FundID_fk] FOREIGN KEY ([FundID_fk]) REFERENCES [dbo].[Funds] ([FundID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CompetitionID_fk]
    ON [dbo].[CompetitionsInFunds]([CompetitionID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FundID_fk]
    ON [dbo].[CompetitionsInFunds]([FundID_fk] ASC);

