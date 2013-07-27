CREATE TABLE [dbo].[CompetitionCouponURLs] (
    [CompetitionCouponURLID_pk] INT            IDENTITY (1, 1) NOT NULL,
    [CompetitionCouponURLID_fk] INT            NOT NULL,
    [ExternalSourceID_fk]       INT            NOT NULL,
    [CouponURL]                 NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_dbo.CompetitionCouponURLs] PRIMARY KEY CLUSTERED ([CompetitionCouponURLID_pk] ASC),
    CONSTRAINT [FK_dbo.CompetitionCouponURLs_dbo.Competitions_CompetitionCouponURLID_fk] FOREIGN KEY ([CompetitionCouponURLID_fk]) REFERENCES [dbo].[Competitions] ([CompetitionID_pk]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.CompetitionCouponURLs_dbo.ExternalSources_ExternalSourceID_fk] FOREIGN KEY ([ExternalSourceID_fk]) REFERENCES [dbo].[ExternalSources] ([ExternalSourceID_pk]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalSourceID_fk]
    ON [dbo].[CompetitionCouponURLs]([ExternalSourceID_fk] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CompetitionCouponURLID_fk]
    ON [dbo].[CompetitionCouponURLs]([CompetitionCouponURLID_fk] ASC);

