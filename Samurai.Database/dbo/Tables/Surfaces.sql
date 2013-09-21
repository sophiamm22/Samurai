CREATE TABLE [dbo].[Surfaces] (
    [SurfaceID_pk] INT            IDENTITY (1, 1) NOT NULL,
    [SurfaceName]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_dbo.Surfaces] PRIMARY KEY CLUSTERED ([SurfaceID_pk] ASC)
);

