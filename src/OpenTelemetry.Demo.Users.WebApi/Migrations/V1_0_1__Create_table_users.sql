IF NOT EXISTS ( SELECT	TOP(1) 1
				FROM	sys.objects
				WHERE	object_id = OBJECT_ID(N'[users]') AND
						type IN ( N'U' ) )
BEGIN
CREATE TABLE users
(
    user_id INT IDENTITY(1,1),
    username NVARCHAR(320) NOT NULL,
    email NVARCHAR(320) NOT NULL,
    created_on DATETIMEOFFSET NOT NULL,
    modified_on DATETIMEOFFSET NULL
)
END