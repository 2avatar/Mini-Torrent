CREATE TABLE [dbo].[Users]
(
	[Username] VARCHAR(50) NOT NULL PRIMARY KEY, 
    [Password] VARCHAR(50) NOT NULL, 
    [Active] BIT NOT NULL
)
