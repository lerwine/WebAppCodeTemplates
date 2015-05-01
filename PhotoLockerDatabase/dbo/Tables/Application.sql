CREATE TABLE [dbo].[Application] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Application_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Title]       NVARCHAR (255)   NOT NULL,
    [Description] TEXT             NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [ModfiedOn]   DATETIME         NOT NULL,
    [ModifiedBy]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Application_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[IdentityCache] ([Id]),
    CONSTRAINT [FK_Application_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[IdentityCache] ([Id])
);


GO
-- =============================================
-- Author:		Leonard T. Erwine
-- Create date: 2015-04-30
-- Description:	Enforces field value rules
-- =============================================
CREATE TRIGGER [dbo].[trig_AddApplication] 
   ON  [dbo].[Application]
   AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM inserted WHERE inserted.CreatedBy <> inserted.ModifiedBy) BEGIN
		RAISERROR('Created by must be same as modified by', 12, 1);
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
CREATE TRIGGER [dbo].[trig_AlterApplication] 
   ON  [dbo].[Application]
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
END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Contains photo application definitions', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Application';

