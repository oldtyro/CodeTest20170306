USE [Merchant]
GO

/****** Object:  Table [dbo].[Merchant]    Script Date: 2017/3/6 14:15:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Merchant](
	[id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_test_Merchant__id]  DEFAULT (newid()),
	[email] [nvarchar](50) NULL,
	[phone] [nvarchar](30) NULL,
	[display_name] [nvarchar](50) NULL,
	[registered_name] [nvarchar](50) NULL,
	[v] [int] NOT NULL CONSTRAINT [DF_test_Merchant___v]  DEFAULT ((0)),
	[date_modified] [datetime] NULL,
	[date_created] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [DF_test_Merchant_status]  DEFAULT ((0)),
 CONSTRAINT [PK_Merchant] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Logo](
	[id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_test_logo__id]  DEFAULT (newid()),
	[merchantid] [uniqueidentifier] NOT NULL,
	[date_created] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [DF_test_logo_status]  DEFAULT ((0)),
	[path_to_file] [nvarchar](200) NULL,
 CONSTRAINT [PK_Logo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Logo]  WITH CHECK ADD  CONSTRAINT [FK_Logo_Merchant] FOREIGN KEY([merchantid])
REFERENCES [dbo].[Merchant] ([id])
GO

ALTER TABLE [dbo].[Logo] CHECK CONSTRAINT [FK_Logo_Merchant]
GO

CREATE TABLE [dbo].[Address](
	[id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_test_Address__id]  DEFAULT (newid()),
	[merchantid] [uniqueidentifier] NOT NULL,
	[country] [nvarchar](30) NULL,
	[state] [nvarchar](30) NULL,
	[postcode] [nvarchar](10) NULL,
	[suburb] [nvarchar](30) NULL,
	[address1] [nvarchar](100) NULL,
	[address2] [nvarchar](100) NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Merchant] FOREIGN KEY([merchantid])
REFERENCES [dbo].[Merchant] ([id])
GO

ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_Merchant]
GO

