USE [AutomaticTasks]
GO

/****** Object:  Table [dbo].[CRON]    Script Date: 30/08/2021 9:30:29 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CRON](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](100) NULL,
	[Url] [nvarchar](500) NOT NULL,
	[CronExpression] [nvarchar](20) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CronResult] [nvarchar](max) NULL,
	[LastExecution] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


