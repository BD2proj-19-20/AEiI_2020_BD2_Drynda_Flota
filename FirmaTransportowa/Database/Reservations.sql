CREATE TABLE [dbo].[Reservations]
(
	[id] INT NOT NULL PRIMARY KEY, 
    [carId] INT NOT NULL, 
    [reservationDate] SMALLDATETIME NOT NULL, 
    [lendDate] SMALLDATETIME NOT NULL, 
    [returnDate] SMALLDATETIME NULL, 
    [lendId] INT NULL, 
    [ended] BIT NOT NULL, 
    [private] BIT NULL, 
    [personId] INT NOT NULL, 
    CONSTRAINT [FK_Reservations_carId] FOREIGN KEY ([carId]) REFERENCES [Cars]([id]), 
    CONSTRAINT [FK_Reservations_lendId] FOREIGN KEY ([lendId]) REFERENCES [Lends]([id]), 
    CONSTRAINT [FK_Reservations_personId] FOREIGN KEY ([personId]) REFERENCES [People]([id])
)
