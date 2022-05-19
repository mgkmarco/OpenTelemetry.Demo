USE [UsersStore]
GO

CREATE PROCEDURE [dbo].[sp_GetUser]
	@UserId INT
AS
BEGIN
	SELECT user_Id AS UserID, username AS Username, email AS [Email] FROM dbo.USERS WITH(NOLOCK) WHERE user_id = @UserId
END

GO