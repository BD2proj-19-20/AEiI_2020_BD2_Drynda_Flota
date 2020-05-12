CREATE TABLE [dbo].[People]
(
	[id] INT NOT NULL PRIMARY KEY, 
    [firstName] NVARCHAR(63) NOT NULL, 
    [lastName] NVARCHAR(63) NOT NULL, 
    [employmentData] SMALLDATETIME NOT NULL, 
    [layoffDate] SMALLDATETIME NULL, 
    [systemLogin] VARCHAR(63) NULL, 
    [passwordHash] BINARY(256) NULL
)
