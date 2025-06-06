CREATE database IOTBaseFarm
go

USE [IOTBaseFarm]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[AccountId] [bigint] IDENTITY(1,1) NOT NULL,
	[email] [varchar](255) NULL,
	[passwordHash] [varchar](255) NULL,
	[role] [int] NULL,
	[status] [int] NULL,
	[createdAt] [date] NULL,
	[updatedAt] [date] NULL,
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AccountProfile]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountProfile](
	[AccountProfileId] [bigint] NOT NULL,
	[gender] [int] NULL,
	[phone] [varchar](255) NULL,
	[fullname] [varchar](75) NULL,
	[address] [varchar](255) NULL,
	[images] [varchar](255) NULL,
	[createdAt] [date] NULL,
	[updatedAt] [date] NULL,
 CONSTRAINT [PK_AccountProfile] PRIMARY KEY CLUSTERED 
(
	[AccountProfileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[CategoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[categoryName] [varchar](255) NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Crop]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Crop](
	[CropId] [bigint] IDENTITY(1,1) NOT NULL,
	[cropName] [varchar](255) NULL,
	[description] [varchar](255) NULL,
	[imageUrl] [varchar](255) NULL,
	[origin] [varchar](255) NULL,
	[status] [int] NULL,
	[CategoryId] [bigint] NOT NULL,
 CONSTRAINT [PK_Crop] PRIMARY KEY CLUSTERED 
(
	[CropId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CropRequirement]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CropRequirement](
	[RequirementId] [bigint] NOT NULL,
	[estimatedDate] [int] NULL,
	[moisture] [decimal](5, 2) NULL,
	[temperature] [decimal](5, 2) NULL,
	[fertilizer] [varchar](255) NULL,
	[deviceID] [bigint] NOT NULL,
 CONSTRAINT [PK_CropRequirement] PRIMARY KEY CLUSTERED 
(
	[RequirementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Farm]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Farm](
	[FarmId] [bigint] IDENTITY(1,1) NOT NULL,
	[farmName] [varchar](255) NULL,
	[location] [varchar](255) NULL,
	[createdAt] [date] NULL,
	[updatedAt] [date] NULL,
	[accountID] [bigint] NOT NULL,
 CONSTRAINT [PK_Farm] PRIMARY KEY CLUSTERED 
(
	[FarmId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FarmActivity]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FarmActivity](
	[FarmActivitiesId] [bigint] IDENTITY(1,1) NOT NULL,
	[activityType] [int] NULL,
	[startDate] [date] NULL,
	[endDate] [date] NULL,
	[status] [int] NULL,
	[scheduleId] [bigint] NOT NULL,
 CONSTRAINT [PK_FarmActivity] PRIMARY KEY CLUSTERED 
(
	[FarmActivitiesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[FeedbackId] [bigint] NOT NULL,
	[comment] [varchar](255) NULL,
	[rating] [int] NULL,
	[status] [int] NULL,
	[createdAt] [date] NULL,
	[customerID] [bigint] NOT NULL,
	[OrderDetailId] [bigint] NOT NULL,
 CONSTRAINT [PK_Feedback] PRIMARY KEY CLUSTERED 
(
	[FeedbackId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Inventory]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Inventory](
	[InventoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[location] [varchar](255) NULL,
	[stockQuantity] [int] NULL,
	[status] [int] NULL,
	[createdAt] [date] NULL,
	[expiryDate] [date] NULL,
	[productId] [bigint] NOT NULL,
	[scheduleId] [bigint] NOT NULL,
 CONSTRAINT [PK_Inventory] PRIMARY KEY CLUSTERED 
(
	[InventoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IoTDevice]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IoTDevice](
	[IoTdevicesId] [bigint] IDENTITY(1,1) NOT NULL,
	[deviceName] [varchar](255) NULL,
	[deviceType] [varchar](255) NULL,
	[status] [int] NULL,
	[unit] [varchar](255) NULL,
	[lastUpdate] [date] NULL,
	[expiryDate] [date] NULL,
	[farmDetailsID] [bigint] NOT NULL,
 CONSTRAINT [PK_IoTDevice] PRIMARY KEY CLUSTERED 
(
	[IoTdevicesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[OrderId] [bigint] IDENTITY(1,1) NOT NULL,
	[totalPrice] [decimal](10, 2) NULL,
	[shippingAddress] [varchar](255) NULL,
	[status] [int] NULL,
	[createdAt] [date] NULL,
	[updatedAt] [date] NULL,
	[customerID] [bigint] NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetail]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetail](
	[OrderDetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[quantity] [int] NULL,
	[unitPrice] [decimal](10, 2) NULL,
	[orderID] [bigint] NOT NULL,
	[productID] [bigint] NOT NULL,
 CONSTRAINT [PK_OrderDetail] PRIMARY KEY CLUSTERED 
(
	[OrderDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[PaymentId] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderId] [bigint] NOT NULL,
	[TransactionId] [nvarchar](max) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[PaymentMethod] [nvarchar](max) NOT NULL,
	[VnPayResponseCode] [nvarchar](max) NOT NULL,
	[Success] [bit] NOT NULL,
	[PaymentTime] [datetime2](7) NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[UpdateDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ProductId] [bigint] NOT NULL,
	[productName] [varchar](255) NULL,
	[images] [varchar](255) NULL,
	[price] [decimal](10, 2) NULL,
	[stockQuantity] [int] NULL,
	[description] [varchar](255) NULL,
	[status] [int] NULL,
	[CreatedAt] [date] NULL,
	[UpdatedAt] [date] NULL,
	[categoryID] [bigint] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Schedule]    Script Date: 5/27/2025 12:02:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schedule](
	[ScheduleId] [bigint] IDENTITY(1,1) NOT NULL,
	[startDate] [date] NULL,
	[endDate] [date] NULL,
	[quantity] [int] NULL,
	[status] [int] NULL,
	[plantingDate] [date] NULL,
	[harvestDate] [date] NULL,
	[createdAt] [date] NULL,
	[updatedAt] [date] NULL,
	[assignedTo] [bigint] NOT NULL,
	[farmDetailsID] [bigint] NOT NULL,
	[cropID] [bigint] NOT NULL,
 CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Account] ON 

INSERT [dbo].[Account] ([AccountId], [email], [passwordHash], [role], [status], [createdAt], [updatedAt]) VALUES (1, N'admin@email.com', N'$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq', 1, 1, CAST(N'2025-03-01' AS Date), CAST(N'2025-03-01' AS Date))
INSERT [dbo].[Account] ([AccountId], [email], [passwordHash], [role], [status], [createdAt], [updatedAt]) VALUES (2, N'manager@email.com', N'$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq', 2, 1, CAST(N'2025-01-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[Account] ([AccountId], [email], [passwordHash], [role], [status], [createdAt], [updatedAt]) VALUES (3, N'staff01@email.com', N'$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq', 3, 1, CAST(N'2025-05-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[Account] ([AccountId], [email], [passwordHash], [role], [status], [createdAt], [updatedAt]) VALUES (4, N'cus01@email.com', N'$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq', 0, 1, CAST(N'2025-04-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[Account] ([AccountId], [email], [passwordHash], [role], [status], [createdAt], [updatedAt]) VALUES (5, N'cus02@email.com', N'$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq', 0, 1, CAST(N'2025-02-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[Account] ([AccountId], [email], [passwordHash], [role], [status], [createdAt], [updatedAt]) VALUES (6, N'staff02@email.com', N'$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq', 3, 1, CAST(N'2025-05-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[Account] ([AccountId], [email], [passwordHash], [role], [status], [createdAt], [updatedAt]) VALUES (7, N'cus04@email.com', N'$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq', 0, 1, CAST(N'2025-04-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[Account] ([AccountId], [email], [passwordHash], [role], [status], [createdAt], [updatedAt]) VALUES (8, N'cus03@email.com', N'$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq', 0, 1, CAST(N'2025-04-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[Account] ([AccountId], [email], [passwordHash], [role], [status], [createdAt], [updatedAt]) VALUES (9, N'staff03@email.com', N'$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq', 3, 1, CAST(N'2025-05-01' AS Date), CAST(N'2025-05-01' AS Date))
SET IDENTITY_INSERT [dbo].[Account] OFF
GO
INSERT [dbo].[AccountProfile] ([AccountProfileId], [gender], [phone], [fullname], [address], [images], [createdAt], [updatedAt]) VALUES (1, 0, N'0123456789', N'Admin', N'TP. Ho Chi Minh', NULL, CAST(N'2025-03-01' AS Date), CAST(N'2025-03-01' AS Date))
INSERT [dbo].[AccountProfile] ([AccountProfileId], [gender], [phone], [fullname], [address], [images], [createdAt], [updatedAt]) VALUES (2, 0, N'0123456789', N'Manager', N'TP. Ho Chi Minh', NULL, CAST(N'2025-01-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[AccountProfile] ([AccountProfileId], [gender], [phone], [fullname], [address], [images], [createdAt], [updatedAt]) VALUES (3, 1, N'0123456789', N'Staff 1', N'TP. Ho Chi Minh', NULL, CAST(N'2025-05-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[AccountProfile] ([AccountProfileId], [gender], [phone], [fullname], [address], [images], [createdAt], [updatedAt]) VALUES (4, 1, N'0123456789', N'Customer 1', N'TP. Ho Chi Minh', NULL, CAST(N'2025-04-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[AccountProfile] ([AccountProfileId], [gender], [phone], [fullname], [address], [images], [createdAt], [updatedAt]) VALUES (5, 1, N'0123456789', N'Customer 2', N'TP. Ho Chi Minh', NULL, CAST(N'2025-02-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[AccountProfile] ([AccountProfileId], [gender], [phone], [fullname], [address], [images], [createdAt], [updatedAt]) VALUES (6, 0, N'0123456789', N'Staff 2', N'TP. Ho Chi Minh', NULL, CAST(N'2025-05-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[AccountProfile] ([AccountProfileId], [gender], [phone], [fullname], [address], [images], [createdAt], [updatedAt]) VALUES (7, 0, N'0123456789', N'Customer 4', N'TP. Ho Chi Minh', NULL, CAST(N'2025-04-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[AccountProfile] ([AccountProfileId], [gender], [phone], [fullname], [address], [images], [createdAt], [updatedAt]) VALUES (8, 0, N'0123456789', N'Customer 3', N'TP. Ho Chi Minh', NULL, CAST(N'2025-04-01' AS Date), CAST(N'2025-05-01' AS Date))
INSERT [dbo].[AccountProfile] ([AccountProfileId], [gender], [phone], [fullname], [address], [images], [createdAt], [updatedAt]) VALUES (9, 0, N'0123456789', N'Staff 3', N'TP. Ho Chi Minh', NULL, CAST(N'2025-02-01' AS Date), CAST(N'2025-05-01' AS Date))
GO
SET IDENTITY_INSERT [dbo].[Category] ON 

INSERT [dbo].[Category] ([CategoryId], [categoryName]) VALUES (1, N'Leafy Vegetables - Rau An La')
INSERT [dbo].[Category] ([CategoryId], [categoryName]) VALUES (2, N'Root Vegetables - Rau Cu')
INSERT [dbo].[Category] ([CategoryId], [categoryName]) VALUES (3, N'Fruiting Vegetables - Rau Qua')
INSERT [dbo].[Category] ([CategoryId], [categoryName]) VALUES (4, N'Herbs and Spices - Rau Gia Vi')
SET IDENTITY_INSERT [dbo].[Category] OFF
GO
SET IDENTITY_INSERT [dbo].[Crop] ON 

INSERT [dbo].[Crop] ([CropId], [cropName], [description], [imageUrl], [origin], [status], [CategoryId]) VALUES (1, N'Cop 01', N'3x3 m2', NULL, NULL, 1, 1)
INSERT [dbo].[Crop] ([CropId], [cropName], [description], [imageUrl], [origin], [status], [CategoryId]) VALUES (2, N'Cop 02', N'2x3 m2', NULL, NULL, 1, 1)
INSERT [dbo].[Crop] ([CropId], [cropName], [description], [imageUrl], [origin], [status], [CategoryId]) VALUES (3, N'Cop 03', N'3x2 m2', NULL, NULL, 1, 1)
INSERT [dbo].[Crop] ([CropId], [cropName], [description], [imageUrl], [origin], [status], [CategoryId]) VALUES (4, N'Cop 04', N'3x5 m2', NULL, NULL, 1, 3)
INSERT [dbo].[Crop] ([CropId], [cropName], [description], [imageUrl], [origin], [status], [CategoryId]) VALUES (5, N'Cop 05', N'4x3 m2', NULL, NULL, 1, 3)
SET IDENTITY_INSERT [dbo].[Crop] OFF
GO
INSERT [dbo].[CropRequirement] ([RequirementId], [estimatedDate], [moisture], [temperature], [fertilizer], [deviceID]) VALUES (1, 25, CAST(1.00 AS Decimal(5, 2)), CAST(29.00 AS Decimal(5, 2)), N'NPK', 1)
INSERT [dbo].[CropRequirement] ([RequirementId], [estimatedDate], [moisture], [temperature], [fertilizer], [deviceID]) VALUES (2, 20, CAST(1.00 AS Decimal(5, 2)), CAST(22.00 AS Decimal(5, 2)), N'NPK', 3)
INSERT [dbo].[CropRequirement] ([RequirementId], [estimatedDate], [moisture], [temperature], [fertilizer], [deviceID]) VALUES (3, 30, CAST(1.00 AS Decimal(5, 2)), CAST(26.00 AS Decimal(5, 2)), N'NPK', 2)
GO
SET IDENTITY_INSERT [dbo].[Farm] ON 

INSERT [dbo].[Farm] ([FarmId], [farmName], [location], [createdAt], [updatedAt], [accountID]) VALUES (1, N'Rau Muong ', N'TP. Ho Chi Minh', CAST(N'2025-02-01' AS Date), CAST(N'2025-03-01' AS Date), 4)
INSERT [dbo].[Farm] ([FarmId], [farmName], [location], [createdAt], [updatedAt], [accountID]) VALUES (2, N'Rau Den', N'TP. Ho Chi Minh', CAST(N'2025-02-01' AS Date), CAST(N'2025-03-02' AS Date), 5)
INSERT [dbo].[Farm] ([FarmId], [farmName], [location], [createdAt], [updatedAt], [accountID]) VALUES (3, N'Cai', N'TP. Ho Chi Minh', CAST(N'2025-02-01' AS Date), CAST(N'2025-03-05' AS Date), 7)
SET IDENTITY_INSERT [dbo].[Farm] OFF
GO
SET IDENTITY_INSERT [dbo].[FarmActivity] ON 

INSERT [dbo].[FarmActivity] ([FarmActivitiesId], [activityType], [startDate], [endDate], [status], [scheduleId]) VALUES (1, 0, CAST(N'2025-03-01' AS Date), CAST(N'2025-04-01' AS Date), 1, 1)
INSERT [dbo].[FarmActivity] ([FarmActivitiesId], [activityType], [startDate], [endDate], [status], [scheduleId]) VALUES (3, 4, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-11' AS Date), 1, 2)
INSERT [dbo].[FarmActivity] ([FarmActivitiesId], [activityType], [startDate], [endDate], [status], [scheduleId]) VALUES (4, 3, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-03' AS Date), 1, 3)
SET IDENTITY_INSERT [dbo].[FarmActivity] OFF
GO
INSERT [dbo].[Feedback] ([FeedbackId], [comment], [rating], [status], [createdAt], [customerID], [OrderDetailId]) VALUES (1, N'App very good, but load data slow', 4, NULL, CAST(N'2025-04-03' AS Date), 4, 1)
INSERT [dbo].[Feedback] ([FeedbackId], [comment], [rating], [status], [createdAt], [customerID], [OrderDetailId]) VALUES (2, N'My avt so cute <3', 5, NULL, CAST(N'2025-04-10' AS Date), 7, 2)
INSERT [dbo].[Feedback] ([FeedbackId], [comment], [rating], [status], [createdAt], [customerID], [OrderDetailId]) VALUES (3, N'Test FeedBack <3', 5, NULL, CAST(N'2025-03-03' AS Date), 8, 3)
GO
SET IDENTITY_INSERT [dbo].[Inventory] ON 

INSERT [dbo].[Inventory] ([InventoryId], [location], [stockQuantity], [status], [createdAt], [expiryDate], [productId], [scheduleId]) VALUES (1, N'TP. Ho Chi Minh', 1000, 1, CAST(N'2025-03-01' AS Date), CAST(N'2025-06-01' AS Date), 1, 1)
INSERT [dbo].[Inventory] ([InventoryId], [location], [stockQuantity], [status], [createdAt], [expiryDate], [productId], [scheduleId]) VALUES (2, N'TP. Ho Chi Minh', 500, 1, CAST(N'2025-03-11' AS Date), CAST(N'2025-06-01' AS Date), 2, 2)
INSERT [dbo].[Inventory] ([InventoryId], [location], [stockQuantity], [status], [createdAt], [expiryDate], [productId], [scheduleId]) VALUES (3, N'TP. Ho Chi Minh', 666, 1, CAST(N'2025-03-08' AS Date), CAST(N'2025-06-01' AS Date), 3, 3)
SET IDENTITY_INSERT [dbo].[Inventory] OFF
GO
SET IDENTITY_INSERT [dbo].[IoTDevice] ON 

INSERT [dbo].[IoTDevice] ([IoTdevicesId], [deviceName], [deviceType], [status], [unit], [lastUpdate], [expiryDate], [farmDetailsID]) VALUES (1, N'Thermocouple - 1', N'Temperature IC', 1, NULL, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-03' AS Date), 2)
INSERT [dbo].[IoTDevice] ([IoTdevicesId], [deviceName], [deviceType], [status], [unit], [lastUpdate], [expiryDate], [farmDetailsID]) VALUES (2, N'LM393 - 1', N'Humidity measurement IC', 1, NULL, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-03' AS Date), 1)
INSERT [dbo].[IoTDevice] ([IoTdevicesId], [deviceName], [deviceType], [status], [unit], [lastUpdate], [expiryDate], [farmDetailsID]) VALUES (3, N'LM393 - 2', N'Humidity measurement IC', 1, NULL, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-03' AS Date), 2)
INSERT [dbo].[IoTDevice] ([IoTdevicesId], [deviceName], [deviceType], [status], [unit], [lastUpdate], [expiryDate], [farmDetailsID]) VALUES (4, N'Thermocouple  - 2', N'Temperature IC', 1, NULL, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-03' AS Date), 3)
INSERT [dbo].[IoTDevice] ([IoTdevicesId], [deviceName], [deviceType], [status], [unit], [lastUpdate], [expiryDate], [farmDetailsID]) VALUES (5, N'Thermocouple  - 3', N'Temperature IC', 1, NULL, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-03' AS Date), 1)
INSERT [dbo].[IoTDevice] ([IoTdevicesId], [deviceName], [deviceType], [status], [unit], [lastUpdate], [expiryDate], [farmDetailsID]) VALUES (6, N'LM393 - 3', N'Humidity measurement IC', 1, NULL, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-03' AS Date), 3)
INSERT [dbo].[IoTDevice] ([IoTdevicesId], [deviceName], [deviceType], [status], [unit], [lastUpdate], [expiryDate], [farmDetailsID]) VALUES (7, N'Soil Moisture Sensor 1', N'Soil Moisture Sensor IC', 1, NULL, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-03' AS Date), 1)
INSERT [dbo].[IoTDevice] ([IoTdevicesId], [deviceName], [deviceType], [status], [unit], [lastUpdate], [expiryDate], [farmDetailsID]) VALUES (8, N'Soil Moisture Sensor 2', N'Soil Moisture Sensor IC', 1, NULL, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-03' AS Date), 2)
INSERT [dbo].[IoTDevice] ([IoTdevicesId], [deviceName], [deviceType], [status], [unit], [lastUpdate], [expiryDate], [farmDetailsID]) VALUES (9, N'Soil Moisture Sensor 3', N'Soil Moisture Sensor IC', 1, NULL, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-03' AS Date), 3)
SET IDENTITY_INSERT [dbo].[IoTDevice] OFF
GO
SET IDENTITY_INSERT [dbo].[Order] ON 

INSERT [dbo].[Order] ([OrderId], [totalPrice], [shippingAddress], [status], [createdAt], [updatedAt], [customerID]) VALUES (1, CAST(200.00 AS Decimal(10, 2)), N'TP. Ho Chi Minh', 0, CAST(N'2025-03-01' AS Date), CAST(N'2025-03-01' AS Date), 8)
INSERT [dbo].[Order] ([OrderId], [totalPrice], [shippingAddress], [status], [createdAt], [updatedAt], [customerID]) VALUES (2, CAST(105.00 AS Decimal(10, 2)), N'TP. Ho Chi Minh', 0, CAST(N'2025-03-01' AS Date), CAST(N'2025-03-01' AS Date), 8)
INSERT [dbo].[Order] ([OrderId], [totalPrice], [shippingAddress], [status], [createdAt], [updatedAt], [customerID]) VALUES (3, CAST(315.00 AS Decimal(10, 2)), N'TP. Ho Chi Minh', 0, CAST(N'2025-03-01' AS Date), CAST(N'2025-03-01' AS Date), 8)
SET IDENTITY_INSERT [dbo].[Order] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderDetail] ON 

INSERT [dbo].[OrderDetail] ([OrderDetailId], [quantity], [unitPrice], [orderID], [productID]) VALUES (1, 20, CAST(10.00 AS Decimal(10, 2)), 1, 1)
INSERT [dbo].[OrderDetail] ([OrderDetailId], [quantity], [unitPrice], [orderID], [productID]) VALUES (2, 10, CAST(15.00 AS Decimal(10, 2)), 2, 2)
INSERT [dbo].[OrderDetail] ([OrderDetailId], [quantity], [unitPrice], [orderID], [productID]) VALUES (3, 21, CAST(10.00 AS Decimal(10, 2)), 3, 2)
SET IDENTITY_INSERT [dbo].[OrderDetail] OFF
GO
SET IDENTITY_INSERT [dbo].[Payment] ON 

INSERT [dbo].[Payment] ([PaymentId], [OrderId], [TransactionId], [Amount], [PaymentMethod], [VnPayResponseCode], [Success], [PaymentTime], [CreateDate], [UpdateDate]) VALUES (1, 1, N'VNPay01', CAST(10.00 AS Decimal(18, 2)), N'VNPay', N'VNPayPayment01', 1, CAST(N'2025-03-01T00:00:00.0000000' AS DateTime2), CAST(N'2025-03-01T00:00:00.0000000' AS DateTime2), NULL)
INSERT [dbo].[Payment] ([PaymentId], [OrderId], [TransactionId], [Amount], [PaymentMethod], [VnPayResponseCode], [Success], [PaymentTime], [CreateDate], [UpdateDate]) VALUES (2, 2, N'VNPay02', CAST(15.00 AS Decimal(18, 2)), N'VNPay', N'VNPayPayment02', 0, CAST(N'2025-03-01T00:00:00.0000000' AS DateTime2), CAST(N'2025-03-01T00:00:00.0000000' AS DateTime2), NULL)
INSERT [dbo].[Payment] ([PaymentId], [OrderId], [TransactionId], [Amount], [PaymentMethod], [VnPayResponseCode], [Success], [PaymentTime], [CreateDate], [UpdateDate]) VALUES (3, 3, N'VNPay03', CAST(21.00 AS Decimal(18, 2)), N'VNPay', N'VNPayPayment03', 0, CAST(N'2025-03-01T00:00:00.0000000' AS DateTime2), CAST(N'2025-03-01T00:00:00.0000000' AS DateTime2), NULL)
SET IDENTITY_INSERT [dbo].[Payment] OFF
GO
INSERT [dbo].[Product] ([ProductId], [productName], [images], [price], [stockQuantity], [description], [status], [CreatedAt], [UpdatedAt], [categoryID]) VALUES (1, N'Morning Glory - Rau Muong', N'https://i1-vnexpress.vnecdn.net/2022/12/02/61lN5mpZAGL-SL1200-1-3093-1669931323.jpg?w=680&h=0&q=100&dpr=1&fit=crop&s=Cj5kP5ZFslHpx0ogALNRdA', CAST(10000.00 AS Decimal(10, 2)), 10000, N'Rau Muong', 1, CAST(N'2025-02-01' AS Date), CAST(N'2025-02-01' AS Date), 1)
INSERT [dbo].[Product] ([ProductId], [productName], [images], [price], [stockQuantity], [description], [status], [CreatedAt], [UpdatedAt], [categoryID]) VALUES (2, N'Amarant - Rau Den', N'https://www.vinmec.com/static/uploads/small_20201226_005345_144787_rau_den_max_1800x1800_jpg_aaca13f0a2.jpg', CAST(15000.00 AS Decimal(10, 2)), 10000, N'Rau Den', 1, CAST(N'2025-02-01' AS Date), CAST(N'2025-02-01' AS Date), 1)
INSERT [dbo].[Product] ([ProductId], [productName], [images], [price], [stockQuantity], [description], [status], [CreatedAt], [UpdatedAt], [categoryID]) VALUES (3, N'Turnip - Cai', N'https://www.hasfarmgreens.com/wp-content/uploads/2023/03/DSC_0255-Edit.jpg', CAST(20000.00 AS Decimal(10, 2)), 10000, N'Cai', 1, CAST(N'2025-02-01' AS Date), CAST(N'2025-02-01' AS Date), 1)
GO
SET IDENTITY_INSERT [dbo].[Schedule] ON 

INSERT [dbo].[Schedule] ([ScheduleId], [startDate], [endDate], [quantity], [status], [plantingDate], [harvestDate], [createdAt], [updatedAt], [assignedTo], [farmDetailsID], [cropID]) VALUES (1, CAST(N'2025-03-01' AS Date), CAST(N'2025-04-03' AS Date), 100, 1, CAST(N'2025-03-01' AS Date), CAST(N'2025-03-02' AS Date), CAST(N'2025-03-01' AS Date), CAST(N'2025-03-01' AS Date), 3, 1, 1)
INSERT [dbo].[Schedule] ([ScheduleId], [startDate], [endDate], [quantity], [status], [plantingDate], [harvestDate], [createdAt], [updatedAt], [assignedTo], [farmDetailsID], [cropID]) VALUES (2, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-11' AS Date), 100, 1, CAST(N'2025-03-01' AS Date), CAST(N'2025-03-02' AS Date), CAST(N'2025-03-01' AS Date), CAST(N'2025-03-01' AS Date), 9, 2, 2)
INSERT [dbo].[Schedule] ([ScheduleId], [startDate], [endDate], [quantity], [status], [plantingDate], [harvestDate], [createdAt], [updatedAt], [assignedTo], [farmDetailsID], [cropID]) VALUES (3, CAST(N'2025-05-04' AS Date), CAST(N'2025-04-03' AS Date), 100, 1, CAST(N'2025-03-01' AS Date), CAST(N'2025-03-02' AS Date), CAST(N'2025-03-01' AS Date), CAST(N'2025-03-01' AS Date), 6, 3, 3)
SET IDENTITY_INSERT [dbo].[Schedule] OFF
GO
ALTER TABLE [dbo].[AccountProfile]  WITH CHECK ADD  CONSTRAINT [FKAccountPro371971] FOREIGN KEY([AccountProfileId])
REFERENCES [dbo].[Account] ([AccountId])
GO
ALTER TABLE [dbo].[AccountProfile] CHECK CONSTRAINT [FKAccountPro371971]
GO
ALTER TABLE [dbo].[Crop]  WITH CHECK ADD  CONSTRAINT [FKCrop824568] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([CategoryId])
GO
ALTER TABLE [dbo].[Crop] CHECK CONSTRAINT [FKCrop824568]
GO
ALTER TABLE [dbo].[CropRequirement]  WITH CHECK ADD  CONSTRAINT [FKCropRequir719183] FOREIGN KEY([RequirementId])
REFERENCES [dbo].[Crop] ([CropId])
GO
ALTER TABLE [dbo].[CropRequirement] CHECK CONSTRAINT [FKCropRequir719183]
GO
ALTER TABLE [dbo].[CropRequirement]  WITH CHECK ADD  CONSTRAINT [FKCropRequir740127] FOREIGN KEY([deviceID])
REFERENCES [dbo].[IoTDevice] ([IoTdevicesId])
GO
ALTER TABLE [dbo].[CropRequirement] CHECK CONSTRAINT [FKCropRequir740127]
GO
ALTER TABLE [dbo].[Farm]  WITH CHECK ADD  CONSTRAINT [FKFarm576533] FOREIGN KEY([accountID])
REFERENCES [dbo].[Account] ([AccountId])
GO
ALTER TABLE [dbo].[Farm] CHECK CONSTRAINT [FKFarm576533]
GO
ALTER TABLE [dbo].[FarmActivity]  WITH CHECK ADD  CONSTRAINT [FKFarmActivi709275] FOREIGN KEY([scheduleId])
REFERENCES [dbo].[Schedule] ([ScheduleId])
GO
ALTER TABLE [dbo].[FarmActivity] CHECK CONSTRAINT [FKFarmActivi709275]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Order_FeedbackId] FOREIGN KEY([FeedbackId])
REFERENCES [dbo].[Order] ([OrderId])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK_Feedback_Order_FeedbackId]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FKFeedback388276] FOREIGN KEY([OrderDetailId])
REFERENCES [dbo].[OrderDetail] ([OrderDetailId])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FKFeedback388276]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FKFeedback770592] FOREIGN KEY([customerID])
REFERENCES [dbo].[Account] ([AccountId])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FKFeedback770592]
GO
ALTER TABLE [dbo].[Inventory]  WITH CHECK ADD  CONSTRAINT [FKInventory560231] FOREIGN KEY([scheduleId])
REFERENCES [dbo].[Schedule] ([ScheduleId])
GO
ALTER TABLE [dbo].[Inventory] CHECK CONSTRAINT [FKInventory560231]
GO
ALTER TABLE [dbo].[Inventory]  WITH CHECK ADD  CONSTRAINT [FKInventory855573] FOREIGN KEY([productId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[Inventory] CHECK CONSTRAINT [FKInventory855573]
GO
ALTER TABLE [dbo].[IoTDevice]  WITH CHECK ADD  CONSTRAINT [FKIoTDevice324669] FOREIGN KEY([farmDetailsID])
REFERENCES [dbo].[Farm] ([FarmId])
GO
ALTER TABLE [dbo].[IoTDevice] CHECK CONSTRAINT [FKIoTDevice324669]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FKOrder459404] FOREIGN KEY([customerID])
REFERENCES [dbo].[Account] ([AccountId])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FKOrder459404]
GO
ALTER TABLE [dbo].[OrderDetail]  WITH CHECK ADD  CONSTRAINT [FKOrderDetai274486] FOREIGN KEY([productID])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[OrderDetail] CHECK CONSTRAINT [FKOrderDetai274486]
GO
ALTER TABLE [dbo].[OrderDetail]  WITH CHECK ADD  CONSTRAINT [FKOrderDetai876065] FOREIGN KEY([orderID])
REFERENCES [dbo].[Order] ([OrderId])
GO
ALTER TABLE [dbo].[OrderDetail] CHECK CONSTRAINT [FKOrderDetai876065]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FKPayment513267] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([OrderId])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FKPayment513267]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FKProduct309661] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Crop] ([CropId])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FKProduct309661]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FKProduct896000] FOREIGN KEY([categoryID])
REFERENCES [dbo].[Category] ([CategoryId])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FKProduct896000]
GO
ALTER TABLE [dbo].[Schedule]  WITH CHECK ADD  CONSTRAINT [FKSchedule19350] FOREIGN KEY([assignedTo])
REFERENCES [dbo].[Account] ([AccountId])
GO
ALTER TABLE [dbo].[Schedule] CHECK CONSTRAINT [FKSchedule19350]
GO
ALTER TABLE [dbo].[Schedule]  WITH CHECK ADD  CONSTRAINT [FKSchedule407130] FOREIGN KEY([farmDetailsID])
REFERENCES [dbo].[Farm] ([FarmId])
GO
ALTER TABLE [dbo].[Schedule] CHECK CONSTRAINT [FKSchedule407130]
GO
ALTER TABLE [dbo].[Schedule]  WITH CHECK ADD  CONSTRAINT [FKSchedule700520] FOREIGN KEY([cropID])
REFERENCES [dbo].[Crop] ([CropId])
GO
ALTER TABLE [dbo].[Schedule] CHECK CONSTRAINT [FKSchedule700520]
GO
