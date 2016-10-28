--
-- Script
--

/****** Object:  Schema [Account]    Script Date: 08.04.2016 09:10:26 ******/

CREATE SCHEMA [Account]
GO

/****** Object:  Table [Account].[Users]    Script Date: 08.04.2016 09:12:07 ******/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Account].[User](
	[Id] [uniqueidentifier] NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [text] NOT NULL,
	[LockOut] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL CONSTRAINT [DF_User_AccessFailedCount]  DEFAULT ((0)),
	[Username] [varchar](255) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

-- error when splitting on string "go" instead of regex:
-- go '" asd 

--
-- Update Info
--


INSERT INTO  [Infrastructure].[DbScripts] ([FileName]) 
VALUES ('2016-10-01_18-00-00_Script01');
