CREATE TABLE [dbo].[ExternalSources] (
    [ExternalSourceID_pk]   INT            IDENTITY (1, 1) NOT NULL,
    [Source]                NVARCHAR (MAX) NOT NULL,
    [SourceNotes]           NVARCHAR (MAX) NULL,
    [OddsSource]            BIT            NOT NULL,
    [TheoreticalOddsSource] BIT            NOT NULL,
    [PredictionURL]         NVARCHAR (MAX) NULL,
    [UseByDefault]          BIT            NOT NULL,
    [PrescreenDecider]      BIT            NOT NULL,
    CONSTRAINT [PK_dbo.ExternalSources] PRIMARY KEY CLUSTERED ([ExternalSourceID_pk] ASC)
);

