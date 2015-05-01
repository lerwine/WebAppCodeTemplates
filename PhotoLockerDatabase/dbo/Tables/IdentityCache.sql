﻿CREATE TABLE [dbo].[IdentityCache] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_IdentityCache_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [ObjectGuid]   UNIQUEIDENTIFIER NOT NULL,
    [SID]          NVARCHAR (1024)  NOT NULL,
    [Domain]       NVARCHAR (256)   NOT NULL,
    [Account]      NVARCHAR (256)   NOT NULL,
    [DisplayName]  NVARCHAR (256)   NOT NULL,
    [EmailAddress] NVARCHAR (1024)  NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [VerifiedOn]   DATETIME         NOT NULL,
    CONSTRAINT [PK_IdentityCache] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_IdentityCache]
    ON [dbo].[IdentityCache]([ObjectGuid] ASC);


GO
-- =============================================
-- Author:		Leonard T. Erwine
-- Create date: 2015-04-30
-- Description:	Enforces field value rules
-- =============================================
CREATE TRIGGER [dbo].[trig_AddIdentityCache] 
   ON  [dbo].[IdentityCache]
   AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM inserted WHERE inserted.CreatedOn <> inserted.VerifiedOn) BEGIN
		RAISERROR('Verified date must be the same as the created date', 11, 1);
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
CREATE TRIGGER [dbo].[trig_AlterIdentityCache] 
   ON  [dbo].[IdentityCache]
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM inserted WHERE inserted.CreatedOn > inserted.VerifiedOn) BEGIN
		RAISERROR('Verified cannot be older than the created date', 11, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END

	IF EXISTS (SELECT * FROM inserted LEFT JOIN deleted ON inserted.Id = deleted.Id WHERE inserted.CreatedOn <> deleted.CreatedOn) BEGIN
		RAISERROR('Created date cannot be modified', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END

	IF EXISTS (SELECT * FROM inserted LEFT JOIN deleted ON inserted.Id = deleted.Id WHERE inserted.ObjectGuid <> deleted.ObjectGuid) BEGIN
		RAISERROR('Object Guid cannot be modified', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END

	IF EXISTS (SELECT * FROM inserted LEFT JOIN deleted ON inserted.Id = deleted.Id WHERE inserted.SID <> deleted.SID) BEGIN
		RAISERROR('SID cannot be modified', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END

	IF EXISTS (SELECT * FROM inserted LEFT JOIN deleted ON inserted.Id = deleted.Id WHERE inserted.Domain <> deleted.Domain) BEGIN
		RAISERROR('Domain cannot be modified', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END

	IF EXISTS (SELECT * FROM inserted LEFT JOIN deleted ON inserted.Id = deleted.Id WHERE inserted.Account <> deleted.Account) BEGIN
		RAISERROR('Account cannot be modified', 12, 1);
		ROLLBACK TRANSACTION;
		RETURN
	END
END
