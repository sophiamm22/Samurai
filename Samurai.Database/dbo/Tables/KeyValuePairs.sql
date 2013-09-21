CREATE TABLE [dbo].[KeyValuePairs] (
    [KeyValuePair_pk] INT            IDENTITY (1, 1) NOT NULL,
    [Key]             NVARCHAR (MAX) NULL,
    [Value]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.KeyValuePairs] PRIMARY KEY CLUSTERED ([KeyValuePair_pk] ASC)
);

