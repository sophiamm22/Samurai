CREATE TABLE [dbo].[Tournaments] (
    [TournamentID_pk] INT            IDENTITY (1, 1) NOT NULL,
    [CompetitionID]   INT            NOT NULL,
    [TournamentName]  NVARCHAR (MAX) NOT NULL,
    [Slug]            NVARCHAR (MAX) NOT NULL,
    [Location]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Tournaments] PRIMARY KEY CLUSTERED ([TournamentID_pk] ASC),
    CONSTRAINT [FK_dbo.Tournaments_dbo.Competitions_CompetitionID] FOREIGN KEY ([CompetitionID]) REFERENCES [dbo].[Competitions] ([CompetitionID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CompetitionID]
    ON [dbo].[Tournaments]([CompetitionID] ASC);

