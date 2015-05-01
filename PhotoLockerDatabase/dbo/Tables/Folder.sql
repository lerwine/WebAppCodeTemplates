CREATE TABLE [dbo].[Folder] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_Folder_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [DisplayText]   NVARCHAR (1024)  NOT NULL,
    [Name]          NVARCHAR (1024)  NOT NULL,
    [Description]   TEXT             NOT NULL,
    [ParentId]      UNIQUEIDENTIFIER NULL,
    [ApplicationId] UNIQUEIDENTIFIER NOT NULL,
    [IsPublic]      BIT              NOT NULL,
    [MetaData]      XML              NOT NULL,
    [CreatedOn]     DATETIME         NOT NULL,
    [CreatedBy]     UNIQUEIDENTIFIER NOT NULL,
    [ModfiedOn]     DATETIME         NOT NULL,
    [ModifiedBy]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Folder] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Folder_Application] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Application] ([Id]),
    CONSTRAINT [FK_Folder_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[IdentityCache] ([Id]),
    CONSTRAINT [FK_Folder_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[IdentityCache] ([Id]),
    CONSTRAINT [FK_Folder_Parent] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Folder] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_FolderPublic]
    ON [dbo].[Folder]([IsPublic] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FolderParent]
    ON [dbo].[Folder]([ParentId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FolderApplication]
    ON [dbo].[Folder]([ApplicationId] ASC);


GO
-- =============================================
-- Author:		Leonard T. Erwine
-- Create date: 2015-04-30
-- Description:	Enforces field value rules
-- =============================================
CREATE TRIGGER [dbo].[trig_AddFolder]
   ON  dbo.Folder
   AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM inserted WHERE inserted.CreatedBy <> inserted.ModifiedBy) BEGIN
		RAISERROR('Created by must be same as modified by', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END

	IF EXISTS (SELECT * FROM inserted WHERE  inserted.ParentId IS NOT NULL AND inserted.ParentId = inserted.Id) BEGIN
		RAISERROR('Folder cannot be parent of itself', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END

	IF EXISTS (SELECT * FROM inserted LEFT JOIN Folder AS Parent ON inserted.ParentId = Parent.Id WHERE inserted.ParentId IS NOT NULL AND inserted.ApplicationId <> Parent.ApplicationId) BEGIN
		RAISERROR('Folder must have same application id as its parent', 12, 1);
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
CREATE TRIGGER [dbo].[trig_AlterFolder]
   ON  dbo.Folder
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
	
	IF EXISTS (SELECT * FROM inserted LEFT JOIN Folder AS Parent ON inserted.ParentId = Parent.Id WHERE inserted.ParentId <> NULL AND inserted.ApplicationId <> Parent.ApplicationId) BEGIN
		RAISERROR('Folder must have same application id as its parent', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END
	
	IF EXISTS (SELECT * FROM inserted WHERE  inserted.ParentId IS NOT NULL AND inserted.ParentId = inserted.Id) BEGIN
		RAISERROR('Folder cannot be parent of itself', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END

	IF EXISTS (SELECT * FROM inserted WHERE inserted.ParentId IS NOT NULL AND dbo.IsDescendentOf(inserted.ParentId, inserted.Id) = 1) BEGIN
		RAISERROR('Circular hierarchical references not allowed.', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END

	IF EXISTS (SELECT * FROM inserted LEFT JOIN Folder AS Sibling ON inserted.ParentId = Sibling.ParentId AND inserted.ApplicationId = Sibling.ApplicationId
			WHERE LOWER(inserted.Name) = LOWER(Sibling.Name)) BEGIN
		RAISERROR('Folder name must be unique within its parent', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END
END
