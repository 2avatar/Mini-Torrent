CREATE TABLE [dbo].[Files]
(
	[Username] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(50) NOT NULL, 
    [Size] INT NOT NULL, 
    CONSTRAINT [FK_Files_Users] FOREIGN KEY ([Username]) REFERENCES [Users]([Username]) 
)
