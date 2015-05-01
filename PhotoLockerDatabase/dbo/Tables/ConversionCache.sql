CREATE TABLE [dbo].[ConversionCache] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_ConversionCache_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [PhotoId]     UNIQUEIDENTIFIER NOT NULL,
    [Data]        IMAGE            NOT NULL,
    [Width]       INT              NOT NULL,
    [Height]      INT              NOT NULL,
    [Format]      INT              NOT NULL,
    [MetaData]    XML              NOT NULL,
    [ConvertedOn] DATETIME         NOT NULL,
    [ConvertedBy] UNIQUEIDENTIFIER NOT NULL,
    [ExpiresOn]   DATETIME         NULL,
    CONSTRAINT [PK_ConversionCache] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ConversionCache_ConvertedBy] FOREIGN KEY ([ConvertedBy]) REFERENCES [dbo].[IdentityCache] ([Id]),
    CONSTRAINT [FK_ConversionCache_Photo] FOREIGN KEY ([PhotoId]) REFERENCES [dbo].[Photo] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ConversionCacheParameters]
    ON [dbo].[ConversionCache]([Width] ASC, [Height] ASC, [Format] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ConversionCachePhoto]
    ON [dbo].[ConversionCache]([PhotoId] ASC);

