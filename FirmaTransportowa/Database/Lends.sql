CREATE TABLE [dbo].[Lends]
(
	[id] INT NOT NULL PRIMARY KEY, 
    [carId] INT NOT NULL, 
    [personId] INT NOT NULL, 
    [lendDate] SMALLDATETIME NOT NULL, 
    [plannedReturnDate] SMALLDATETIME NULL, 
    [returnDate] SMALLDATETIME NULL, 
    [startOdometer] INT NOT NULL, 
    [endOdometer] INT NULL, 
    [startFuel] FLOAT NOT NULL, 
    [endFuel] FLOAT NULL, 
    [private] BIT NOT NULL, 
    [reservationId] INT NULL, 
    [comments] TEXT NULL, 
    CONSTRAINT [FK_Lends_carId] FOREIGN KEY ([carId]) REFERENCES [Cars]([id]), 
    CONSTRAINT [FK_Lends_personId] FOREIGN KEY ([personId]) REFERENCES [People]([id]), 
    CONSTRAINT [FK_Lends_reservationId] FOREIGN KEY ([reservationId]) REFERENCES [Reservations]([id])
)
