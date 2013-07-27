CREATE TABLE [dbo].[TennisPredictionStats] (
    [MatchID_fk]   INT             NOT NULL,
    [PlayerAGames] INT             NOT NULL,
    [PlayerBGames] INT             NOT NULL,
    [EPoints]      DECIMAL (10, 4) NULL,
    [EGames]       DECIMAL (10, 4) NULL,
    [ESets]        DECIMAL (10, 4) NULL,
    CONSTRAINT [PK_dbo.TennisPredictionStats] PRIMARY KEY CLUSTERED ([MatchID_fk] ASC),
    CONSTRAINT [FK_dbo.TennisPredictionStats_dbo.Matches_MatchID_fk] FOREIGN KEY ([MatchID_fk]) REFERENCES [dbo].[Matches] ([MatchID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_MatchID_fk]
    ON [dbo].[TennisPredictionStats]([MatchID_fk] ASC);

