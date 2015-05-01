-- =============================================
-- Author:		Leonard T. Erwine
-- Create date: 2015-04-30
-- Description:	Determines if a folder belongs to an ancestor
-- =============================================
CREATE FUNCTION [dbo].[IsDescendentOf]
(
	@FolderId UniqueIdentifier,
	@AncestorId UniqueIdentifier
)
RETURNS bit
AS
BEGIN
	DECLARE @ResultVar bit;
	DECLARE @ApplicationId uniqueidentifier;
	DECLARE @AncestorApplicationId uniqueidentifier;
	DECLARE @ParentId uniqueidentifier;
	
	SELECT @ApplicationId = ApplicationId FROM Folder WHERE Id = @FolderId;
	IF @ApplicationId IS NULL BEGIN
		SET @ResultVar = 0;
		RETURN @ResultVar;
	END
	
	SELECT @AncestorApplicationId = ApplicationId FROM Folder WHERE Id = @AncestorId;
	IF @AncestorApplicationId IS NULL OR @ApplicationId <> @AncestorApplicationId BEGIN
		SET @ResultVar = 0;
		RETURN @ResultVar;
	END
	
	SELECT @ParentId = ParentId FROM Folder WHERE Id = @FolderId;

	IF @ParentId IS NULL BEGIN
		SET @ResultVar = 0;
		RETURN @ResultVar;
	END
	
	IF @ParentId = @AncestorId BEGIN
		SET @ResultVar = 1;
	END
	ELSE BEGIN
		SET @ResultVar = dbo.IsDescendentOf(@ParentId, @AncestorId);
	END
	
	RETURN @ResultVar;
END
