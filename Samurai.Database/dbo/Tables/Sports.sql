CREATE TABLE [dbo].[Sports] (
    [SportID_pk] INT            IDENTITY (1, 1) NOT NULL,
    [SportName]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_dbo.Sports] PRIMARY KEY CLUSTERED ([SportID_pk] ASC)
);

