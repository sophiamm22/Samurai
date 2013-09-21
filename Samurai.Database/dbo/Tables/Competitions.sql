CREATE TABLE [dbo].[Competitions] (
    [CompetitionID_pk]    INT             IDENTITY (1, 1) NOT NULL,
    [SportID_fk]          INT             NOT NULL,
    [CompetitionName]     NVARCHAR (MAX)  NOT NULL,
    [Slug]                NVARCHAR (MAX)  NOT NULL,
    [EdgeRequired]        DECIMAL (18, 2) NOT NULL,
    [GamesRequiredForBet] INT             NULL,
    CONSTRAINT [PK_dbo.Competitions] PRIMARY KEY CLUSTERED ([CompetitionID_pk] ASC),
    CONSTRAINT [FK_dbo.Competitions_dbo.Sports_SportID_fk] FOREIGN KEY ([SportID_fk]) REFERENCES [dbo].[Sports] ([SportID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_SportID_fk]
    ON [dbo].[Competitions]([SportID_fk] ASC);

