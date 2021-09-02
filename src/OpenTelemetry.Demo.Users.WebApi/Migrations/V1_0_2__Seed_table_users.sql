USE [UsersStore]
GO

INSERT INTO [dbo].[users]
           ([username]
           ,[email]
           ,[created_on]
           ,[modified_on])
VALUES
	(
        'mgk',
        'mgkmarco@gmail.com',
        SYSDATETIMEOFFSET(),
        SYSDATETIMEOFFSET()
    ),
    (
        'marco',
        'marcogalea1502@gmail.com',
        SYSDATETIMEOFFSET(),
        SYSDATETIMEOFFSET()
    )
GO