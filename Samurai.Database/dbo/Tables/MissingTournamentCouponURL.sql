CREATE TABLE [dbo].[MissingTournamentCouponURL] (
    [MissingTournamentCouponURLID_pk] INT IDENTITY (1, 1) NOT NULL,
    [TournamentID_fk]                 INT NOT NULL,
    [ExternalSourceID_fk]             INT NOT NULL,
    CONSTRAINT [PK_MissingTournamentCouponURL] PRIMARY KEY CLUSTERED ([MissingTournamentCouponURLID_pk] ASC),
    CONSTRAINT [FK_MissingTournamentCouponURL_ExternalSources] FOREIGN KEY ([ExternalSourceID_fk]) REFERENCES [dbo].[ExternalSources] ([ExternalSourceID_pk]),
    CONSTRAINT [FK_MissingTournamentCouponURL_Tournaments] FOREIGN KEY ([TournamentID_fk]) REFERENCES [dbo].[Tournaments] ([TournamentID_pk])
);

