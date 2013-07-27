CREATE TABLE [dbo].[TournamentCouponURLs] (
    [TournamentCouponURLID_pk] INT            IDENTITY (1, 1) NOT NULL,
    [TournamentID_fk]          INT            NOT NULL,
    [ExternalSourceID_fk]      INT            NOT NULL,
    [CouponURL]                NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_dbo.TournamentCouponURLs] PRIMARY KEY CLUSTERED ([TournamentCouponURLID_pk] ASC),
    CONSTRAINT [FK_dbo.TournamentCouponURLs_dbo.ExternalSources_ExternalSourceID_fk] FOREIGN KEY ([ExternalSourceID_fk]) REFERENCES [dbo].[ExternalSources] ([ExternalSourceID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.TournamentCouponURLs_dbo.Tournaments_TournamentID_fk] FOREIGN KEY ([TournamentID_fk]) REFERENCES [dbo].[Tournaments] ([TournamentID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalSourceID_fk]
    ON [dbo].[TournamentCouponURLs]([ExternalSourceID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TournamentID_fk]
    ON [dbo].[TournamentCouponURLs]([TournamentID_fk] ASC);

