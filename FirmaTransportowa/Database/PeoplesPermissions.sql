CREATE TABLE [dbo].[PeoplesPermissions]
(
	[personId] INT NOT NULL , 
    [permissionId] INT NOT NULL, 
    [grantDate] SMALLDATETIME NOT NULL, 
    [revokeDate] SMALLDATETIME NULL, 
    PRIMARY KEY ([personId], [permissionId]), 
    CONSTRAINT [FK_PeoplesPermissions_personId] FOREIGN KEY ([personId]) REFERENCES [People]([id]), 
    CONSTRAINT [FK_PeoplesPermissions_permissionId] FOREIGN KEY ([permissionId]) REFERENCES [Permissions]([id])
)
