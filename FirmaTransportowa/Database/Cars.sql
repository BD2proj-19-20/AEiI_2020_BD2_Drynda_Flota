CREATE TABLE [dbo].[Cars]
(
	[id] INT NOT NULL PRIMARY KEY, 
    [Registration] CHAR(8) NOT NULL, 
    [engineCapacity] SMALLINT NOT NULL, 
    [purchaseDate] SMALLDATETIME NOT NULL, 
    [saleDate] SMALLDATETIME NULL, 
    [inspectionValidUntil] SMALLDATETIME NOT NULL, 
    [onService] BIT NOT NULL, 
    [modelId] INT NOT NULL, 
    [destinationId] INT NOT NULL, 
    CONSTRAINT [FK_modelId] FOREIGN KEY ([modelId]) REFERENCES [CarModels]([id]), 
    CONSTRAINT [FK_destinationId] FOREIGN KEY ([destinationId]) REFERENCES [CarDestinations]([id]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'[cm^3]',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Cars',
    @level2type = N'COLUMN',
    @level2name = N'engineCapacity'