CREATE TABLE [dbo].[Activities]
(
	[id] INT NOT NULL PRIMARY KEY, 
    [critical] BIT NOT NULL, 
    [reporterId] INT NOT NULL, 
    [reportDate] SMALLDATETIME NOT NULL, 
    [closeDate] SMALLDATETIME NULL, 
    [service] BIT NOT NULL, 
    [price] MONEY NULL, 
    [orderDate] SMALLDATETIME NULL, 
    [contractorId] INT NULL, 
    [comments] TEXT NULL, 
    CONSTRAINT [FK_Activities_reporterId] FOREIGN KEY ([reporterId]) REFERENCES [People]([id]), 
    CONSTRAINT [FK_Activities_contractorId] FOREIGN KEY ([contractorId]) REFERENCES [Contractors]([id]) 
)
