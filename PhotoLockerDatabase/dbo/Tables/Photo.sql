CREATE TABLE [dbo].[Photo] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_Photo_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [DisplayText]   NVARCHAR (1024)  NOT NULL,
    [Name]          NVARCHAR (1024)  NOT NULL,
    [Description]   TEXT             NOT NULL,
    [ParentId]      UNIQUEIDENTIFIER NULL,
    [ApplicationId] UNIQUEIDENTIFIER NOT NULL,
    [IsPublic]      BIT              NOT NULL,
    [Data]          IMAGE            NOT NULL,
    [Width]         INT              NOT NULL,
    [Height]        INT              NOT NULL,
    [Format]        INT              NOT NULL,
    [MetaData]      XML              NOT NULL,
    [CreatedOn]     DATETIME         NOT NULL,
    [CreatedBy]     UNIQUEIDENTIFIER NOT NULL,
    [ModfiedOn]     DATETIME         NOT NULL,
    [ModifiedBy]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Photo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Photo_Application] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Application] ([Id]),
    CONSTRAINT [FK_Photo_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[IdentityCache] ([Id]),
    CONSTRAINT [FK_Photo_Folder] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Folder] ([Id]),
    CONSTRAINT [FK_Photo_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[IdentityCache] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_PhotoParent]
    ON [dbo].[Photo]([ParentId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PhotoPublic]
    ON [dbo].[Photo]([IsPublic] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PhotoApplication]
    ON [dbo].[Photo]([ApplicationId] ASC);


GO
-- =============================================
-- Author:		Leonard T. Erwine
-- Create date: 2015-04-30
-- Description:	Enforces field value rules
-- =============================================
CREATE TRIGGER [dbo].[trig_AlterPhoto]
   ON  dbo.Photo
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM inserted WHERE inserted.CreatedOn > inserted.ModfiedOn) BEGIN
		RAISERROR('Modified date cannot be older than created date', 11, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END

	IF EXISTS (SELECT * FROM inserted LEFT JOIN deleted ON inserted.Id = deleted.Id WHERE inserted.CreatedOn <> deleted.CreatedOn) BEGIN
		RAISERROR('Created date cannot be modified', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END

	IF EXISTS (SELECT * FROM inserted LEFT JOIN deleted ON inserted.Id = deleted.Id WHERE inserted.CreatedBy <> deleted.CreatedBy) BEGIN
		RAISERROR('Created by cannot be modified', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END
	
	IF EXISTS (SELECT * FROM inserted LEFT JOIN deleted ON inserted.Id = deleted.Id WHERE inserted.ApplicationId <> deleted.ApplicationId) BEGIN
		RAISERROR('Application Id cannot be modified', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END
	
	IF EXISTS (SELECT * FROM inserted LEFT JOIN Folder AS Parent ON inserted.ParentId = Parent.Id WHERE inserted.ParentId IS NOT NULL AND inserted.ApplicationId <> Parent.ApplicationId) BEGIN
		RAISERROR('Parent must have same application id as photo', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END
	
	IF EXISTS (SELECT * FROM inserted LEFT JOIN Folder AS Sibling ON inserted.ParentId = Sibling.ParentId AND inserted.ApplicationId = Sibling.ApplicationId
			WHERE LOWER(inserted.Name) = LOWER(Sibling.Name)) BEGIN
		RAISERROR('Photo name must be unique within its parent', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END
END

GO
-- =============================================
-- Author:		Leonard T. Erwine
-- Create date: 2015-04-30
-- Description:	Enforces field value rules
-- =============================================
CREATE TRIGGER dbo.trig_AddPhoto
   ON  dbo.Photo
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM inserted WHERE inserted.CreatedBy <> inserted.ModifiedBy) BEGIN
		RAISERROR('Created by must be same as modified by', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END

	IF EXISTS (SELECT * FROM inserted LEFT JOIN Folder AS Parent ON inserted.ParentId = Parent.Id WHERE inserted.ParentId IS NOT NULL AND inserted.ApplicationId <> Parent.ApplicationId) BEGIN
		RAISERROR('Photo must have same application id as its parent', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END
END
