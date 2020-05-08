CREATE TABLE [dbo].[CarExtrasInCars]
(
	[carId] INT NOT NULL , 
    [extraId] INT NOT NULL, 
    CONSTRAINT [FK_CarId] FOREIGN KEY ([carId]) REFERENCES [Cars]([id]), 
    CONSTRAINT [FK_ExtraId] FOREIGN KEY ([extraId]) REFERENCES [CarExtras]([id]), 
    PRIMARY KEY ([carId], [extraId]) 
)
