CREATE TABLE [dbo].[OutcomeComments] (
    [OutcomeCommentID_pk] INT            IDENTITY (1, 1) NOT NULL,
    [Comment]             NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_dbo.OutcomeComments] PRIMARY KEY CLUSTERED ([OutcomeCommentID_pk] ASC)
);

