USE [Test]
GO

/****** Object:  Table [dbo].[OvenDataTable]    Script Date: 2018/8/14 14:31:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OvenDataTable](
	[录入时间] [datetime] NOT NULL,
	[系列号] [nchar](10) NOT NULL,
	[门状态] [nchar](10) NULL,
	[温度] [nchar](10) NULL,
	[倒数计时] [nchar](10) NULL,
	[温度状态] [nchar](10) NULL,
	[计时差] [nchar](10) NULL,
	[操作员工号] [nchar](10) NULL,
	[操作机器号] [nchar](10) NULL,
	[异常停机代码] [nchar](10) NULL,
	[上下数分开记录信息] [nchar](10) NULL,
	[扫描] [nchar](10) NULL,
 CONSTRAINT [PK_OvenDataTable] PRIMARY KEY CLUSTERED 
(
	[录入时间] ASC,
	[系列号] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

