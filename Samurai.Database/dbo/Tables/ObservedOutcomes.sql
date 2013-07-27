CREATE TABLE [dbo].[ObservedOutcomes] (
    [ObservedOutcomeID_pk] INT IDENTITY (1, 1) NOT NULL,
    [MatchID_fk]           INT NOT NULL,
    [ScoreOutcomeID_fk]    INT NOT NULL,
    [OutcomeCommentID_fk]  INT NOT NULL,
    CONSTRAINT [PK_dbo.ObservedOutcomes] PRIMARY KEY CLUSTERED ([ObservedOutcomeID_pk] ASC),
    CONSTRAINT [FK_dbo.ObservedOutcomes_dbo.Matches_MatchID_fk] FOREIGN KEY ([MatchID_fk]) REFERENCES [dbo].[Matches] ([MatchID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ObservedOutcomes_dbo.OutcomeComments_OutcomeCommentID_fk] FOREIGN KEY ([OutcomeCommentID_fk]) REFERENCES [dbo].[OutcomeComments] ([OutcomeCommentID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ObservedOutcomes_dbo.ScoreOutcomes_ScoreOutcomeID_fk] FOREIGN KEY ([ScoreOutcomeID_fk]) REFERENCES [dbo].[ScoreOutcomes] ([ScoreOutcomeID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_OutcomeCommentID_fk]
    ON [dbo].[ObservedOutcomes]([OutcomeCommentID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ScoreOutcomeID_fk]
    ON [dbo].[ObservedOutcomes]([ScoreOutcomeID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MatchID_fk]
    ON [dbo].[ObservedOutcomes]([MatchID_fk] ASC);

