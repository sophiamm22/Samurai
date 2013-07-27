CREATE TABLE [dbo].[MatchCouponURLs] (
    [MatchCouponURLID_pk] INT            IDENTITY (1, 1) NOT NULL,
    [MatchID_fk]          INT            NOT NULL,
    [ExternalSourceID_fk] INT            NOT NULL,
    [MatchCouponURL]      NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_dbo.MatchCouponURLs] PRIMARY KEY CLUSTERED ([MatchCouponURLID_pk] ASC),
    CONSTRAINT [FK_dbo.MatchCouponURLs_dbo.ExternalSources_ExternalSourceID_fk] FOREIGN KEY ([ExternalSourceID_fk]) REFERENCES [dbo].[ExternalSources] ([ExternalSourceID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MatchCouponURLs_dbo.Matches_MatchID_fk] FOREIGN KEY ([MatchID_fk]) REFERENCES [dbo].[Matches] ([MatchID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalSourceID_fk]
    ON [dbo].[MatchCouponURLs]([ExternalSourceID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MatchID_fk]
    ON [dbo].[MatchCouponURLs]([MatchID_fk] ASC);

