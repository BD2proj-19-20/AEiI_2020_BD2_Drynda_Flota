CREATE TABLE [dbo].[CarSupervisors]
(
	[personId] INT NOT NULL , 
    [carId] INT NOT NULL, 
    [beginDate] SMALLDATETIME NOT NULL, 
    [endDate] SMALLDATETIME NULL, 
    CONSTRAINT [FK_personId] FOREIGN KEY ([personId]) REFERENCES [People]([id]), 
    PRIMARY KEY ([personId], [carId]) 
)
