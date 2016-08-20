CREATE TABLE [dbo].[Templates]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Class] NVARCHAR(50) NOT NULL, 
    [Css] TEXT NOT NULL
)
