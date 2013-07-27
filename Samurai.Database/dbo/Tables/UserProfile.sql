CREATE TABLE [dbo].[UserProfile] (
    [UserProfileID_pk] INT           IDENTITY (1, 1) NOT NULL,
    [UserName]         NVARCHAR (56) NOT NULL,
    PRIMARY KEY CLUSTERED ([UserProfileID_pk] ASC),
    UNIQUE NONCLUSTERED ([UserName] ASC)
);

