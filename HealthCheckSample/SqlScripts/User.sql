--請先先增一個 User 資料庫

USE [User]
GO
/****** Object:  Table [dbo].[User]    Script Date: 2023/5/30 上午 10:02:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[account] [varchar](30) NOT NULL,
	[password] [varchar](50) NOT NULL,
	[enable] [int] NOT NULL,
	[create_id] [int] NOT NULL,
	[create_time] [datetime] NOT NULL,
	[update_id] [int] NULL,
	[update_time] [datetime] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([id], [account], [password], [enable], [create_id], [create_time], [update_id], [update_time]) VALUES (2, N'user1', N'password1', 1, 1, CAST(N'2023-05-30T09:48:02.527' AS DateTime), NULL, NULL)
INSERT [dbo].[User] ([id], [account], [password], [enable], [create_id], [create_time], [update_id], [update_time]) VALUES (3, N'user2', N'password2', 1, 1, CAST(N'2023-05-30T09:48:02.527' AS DateTime), NULL, NULL)
INSERT [dbo].[User] ([id], [account], [password], [enable], [create_id], [create_time], [update_id], [update_time]) VALUES (4, N'user3', N'password3', 0, 2, CAST(N'2023-05-30T09:48:02.527' AS DateTime), NULL, NULL)
INSERT [dbo].[User] ([id], [account], [password], [enable], [create_id], [create_time], [update_id], [update_time]) VALUES (5, N'user4', N'password4', 1, 2, CAST(N'2023-05-30T09:48:02.527' AS DateTime), NULL, NULL)
INSERT [dbo].[User] ([id], [account], [password], [enable], [create_id], [create_time], [update_id], [update_time]) VALUES (6, N'user5', N'password5', 1, 3, CAST(N'2023-05-30T09:48:02.527' AS DateTime), NULL, NULL)
INSERT [dbo].[User] ([id], [account], [password], [enable], [create_id], [create_time], [update_id], [update_time]) VALUES (7, N'user1', N'password1', 1, 1, CAST(N'2023-05-30T09:48:29.447' AS DateTime), NULL, NULL)
INSERT [dbo].[User] ([id], [account], [password], [enable], [create_id], [create_time], [update_id], [update_time]) VALUES (8, N'user2', N'password2', 1, 1, CAST(N'2023-05-30T09:48:29.467' AS DateTime), NULL, NULL)
INSERT [dbo].[User] ([id], [account], [password], [enable], [create_id], [create_time], [update_id], [update_time]) VALUES (9, N'user3', N'password3', 0, 2, CAST(N'2023-05-30T09:48:29.467' AS DateTime), NULL, NULL)
INSERT [dbo].[User] ([id], [account], [password], [enable], [create_id], [create_time], [update_id], [update_time]) VALUES (10, N'user4', N'password4', 1, 2, CAST(N'2023-05-30T09:48:29.467' AS DateTime), NULL, NULL)
INSERT [dbo].[User] ([id], [account], [password], [enable], [create_id], [create_time], [update_id], [update_time]) VALUES (11, N'user5', N'password5', 1, 3, CAST(N'2023-05-30T09:48:29.467' AS DateTime), NULL, NULL)
INSERT [dbo].[User] ([id], [account], [password], [enable], [create_id], [create_time], [update_id], [update_time]) VALUES (12, N'test001', N'test001', 0, 0, CAST(N'2023-05-30T09:50:04.163' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[User] OFF
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_enable]  DEFAULT ((0)) FOR [enable]
GO
EXEC sys.sp_addextendedproperty @name=N'test001', @value=N'流水號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'帳號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'account'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'password'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用, 0:否, 1:是' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'enable'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立者id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'create_id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新增時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'create_time'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'編輯者id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'update_id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'編輯時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'update_time'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用者' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User'
GO
