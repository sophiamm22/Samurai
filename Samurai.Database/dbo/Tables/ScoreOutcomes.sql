CREATE TABLE [dbo].[ScoreOutcomes] (
    [ScoreOutcomeID_pk] INT IDENTITY (1, 1) NOT NULL,
    [MatchOutcomeID_fk] INT NOT NULL,
    [TeamAScore]        INT NOT NULL,
    [TeamBScore]        INT NOT NULL,
    CONSTRAINT [PK_dbo.ScoreOutcomes] PRIMARY KEY CLUSTERED ([ScoreOutcomeID_pk] ASC),
    CONSTRAINT [FK_dbo.ScoreOutcomes_dbo.MatchOutcomes_MatchOutcomeID_fk] FOREIGN KEY ([MatchOutcomeID_fk]) REFERENCES [dbo].[MatchOutcomes] ([MatchOutcomeID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_MatchOutcomeID_fk]
    ON [dbo].[ScoreOutcomes]([MatchOutcomeID_fk] ASC);

