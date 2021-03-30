using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectBoss.Data.Migrations
{
    public partial class SeedUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = @"USE [TaskBoss]
                           GO
                           INSERT [dbo].[Role] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'534027b4-98e9-41a2-9588-1d01e5d5b50f', N'CommonUser', N'CommonUser', N'4e136ba6-25d3-4e7f-b1ec-6c58783096c5')
                           GO
                           INSERT [dbo].[Role] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'daddbd44-6d79-4d5a-8b99-5cdcee017a34', N'ProjectManager', N'ProjectManager', N'91ab12a2-7694-4e1b-a6b1-101b614ba10f')
                           GO
                           INSERT [dbo].[Role] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'ED110E5C-F7B3-40FA-B3D3-736762213CB2', N'Administrator', N'Administrator', N'dc71b025-31da-4144-afd8-3fbaf48ee7a6')
                           GO
                           INSERT [dbo].[User] ([Id], [Provider], [ExternalUserId], [CreatedDate], [IsAdmin], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'129CEA46-F5C8-424F-86CB-FB3391308889', N'LOCAL', NULL, CAST(N'2021-03-29T19:40:02.9774879' AS DateTime2), 1, N'admin@pjb.com', N'admin@pjb.com', N'admin@pjb.com', N'admin@pjb.com', 1, N'AQAAAAEAACcQAAAAEPMDjNaAOUgXCoM6Pvp++mnypj0VncQAXPxf7F4gi+tirnRyFGidhylYRuMscXJ/QA==', N'43606eae-7191-4785-94e3-beb5fd4ae585', N'a46ee9c3-7b57-454b-9f67-ea8f735706b5', NULL, 0, 0, NULL, 0, 0)
                           GO
                           INSERT [dbo].[User] ([Id], [Provider], [ExternalUserId], [CreatedDate], [IsAdmin], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'41fda830-08c6-4e69-be0d-11f872f3f1f7', N'LOCAL', NULL, CAST(N'2021-03-29T19:44:01.4634361' AS DateTime2), 0, N'c.herman@email.com', N'C.HERMAN@EMAIL.COM', N'c.herman@email.com', N'C.HERMAN@EMAIL.COM', 1, N'AQAAAAEAACcQAAAAEDy4DPbjd2YcdShB1nBDl0eOcR2K6vM1WOF7SLgYyOOqgKewhtjnBTRZymjyj/8m7g==', N'RSPCJY6AZJ4ZS36NWQZBBSHLZMSSAQ6J', N'3e1d5d4c-eb97-4365-9b20-74cf23011149', NULL, 0, 0, NULL, 1, 0)
                           GO
                           INSERT [dbo].[User] ([Id], [Provider], [ExternalUserId], [CreatedDate], [IsAdmin], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'65fb4124-77a2-4a78-858a-890faa5912c6', N'LOCAL', NULL, CAST(N'2021-03-29T19:43:15.6653247' AS DateTime2), 0, N'hale.vanessa@email.com', N'HALE.VANESSA@EMAIL.COM', N'hale.vanessa@email.com', N'HALE.VANESSA@EMAIL.COM', 1, N'AQAAAAEAACcQAAAAEL9jmjQgA71NHtOw3Q6OoNZz+wwEossRwYucU7G0RwCJxVaHyQ06udJlNjYvtnvsYQ==', N'DE3QYMUCUR3RJFV2IBESEIZOCVA4GRK3', N'589ce213-4d48-44d3-b750-9132c882804f', NULL, 0, 0, NULL, 1, 0)
                           GO
                           INSERT [dbo].[User] ([Id], [Provider], [ExternalUserId], [CreatedDate], [IsAdmin], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'90364db0-b11c-47f7-a73b-6a925e43df2f', N'LOCAL', NULL, CAST(N'2021-03-29T19:43:34.2793501' AS DateTime2), 0, N'hsebastian@test.com', N'HSEBASTIAN@TEST.COM', N'hsebastian@test.com', N'HSEBASTIAN@TEST.COM', 1, N'AQAAAAEAACcQAAAAEN6fNhg0x8/CUCxrnLLaxbxe3dmSjsfYV/swYpaSpKeEJ+9GQD4xhYaBFJY/24n7GQ==', N'P7AASRRTYY7RMNXPNCY4QGZOODTN76RX', N'2c332b6f-aa77-4903-9d22-c897d7349b27', NULL, 0, 0, NULL, 1, 0)
                           GO
                           INSERT [dbo].[User] ([Id], [Provider], [ExternalUserId], [CreatedDate], [IsAdmin], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'be5361e4-d467-4809-aed9-0b2d8711b52c', N'LOCAL', NULL, CAST(N'2021-03-29T19:42:57.4493767' AS DateTime2), 0, N'amcdaniels@email.com', N'AMCDANIELS@EMAIL.COM', N'amcdaniels@email.com', N'AMCDANIELS@EMAIL.COM', 1, N'AQAAAAEAACcQAAAAENNYRQ8b+raIJWNEgNTydv2LXQciKJfhIa6aFsujKS1E/v9ZPdeLBVw1McacmzMO5Q==', N'JE2JKRQ7WBDDVWCZ6DM7MVE3KLJXXH2H', N'd6f3a427-a226-4dd5-a2f7-12277719c16b', NULL, 0, 0, NULL, 1, 0)
                           GO
                           INSERT [dbo].[User] ([Id], [Provider], [ExternalUserId], [CreatedDate], [IsAdmin], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'e27d3b25-04fd-45f1-b32e-14c113b3de0a', N'LOCAL', NULL, CAST(N'2021-03-29T19:44:24.4726204' AS DateTime2), 0, N'aschneider@email.com', N'ASCHNEIDER@EMAIL.COM', N'aschneider@email.com', N'ASCHNEIDER@EMAIL.COM', 1, N'AQAAAAEAACcQAAAAEAEfI753yrGbB7XVv+1LsTcQgvKu1zBW4Yen7fEKvAeFFPGh2OYs0NJmOHHlij3H+A==', N'XNHVSKI6I2KTPGEYFJ4WRWVAT47N3UI6', N'd2667458-ce79-49e4-9b91-29a4710d75c9', NULL, 0, 0, NULL, 1, 0)
                           GO
                           INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'41fda830-08c6-4e69-be0d-11f872f3f1f7', N'534027b4-98e9-41a2-9588-1d01e5d5b50f')
                           GO
                           INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'65fb4124-77a2-4a78-858a-890faa5912c6', N'534027b4-98e9-41a2-9588-1d01e5d5b50f')
                           GO
                           INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'90364db0-b11c-47f7-a73b-6a925e43df2f', N'534027b4-98e9-41a2-9588-1d01e5d5b50f')
                           GO
                           INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'be5361e4-d467-4809-aed9-0b2d8711b52c', N'534027b4-98e9-41a2-9588-1d01e5d5b50f')
                           GO
                           INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'e27d3b25-04fd-45f1-b32e-14c113b3de0a', N'534027b4-98e9-41a2-9588-1d01e5d5b50f')
                           GO
                           INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (N'129CEA46-F5C8-424F-86CB-FB3391308889', N'ED110E5C-F7B3-40FA-B3D3-736762213CB2')
                           GO
                           SET IDENTITY_INSERT [dbo].[Person] ON 
                           GO
                           INSERT [dbo].[Person] ([PersonId], [PersonCode], [UserId], [FirstName], [LastName], [Company], [Role], [Country], [CreatedDate], [IsActive]) VALUES (N'a3fd4b32-927d-4538-b6f5-29ce6fd717bd', 1000, N'129CEA46-F5C8-424F-86CB-FB3391308889', N'Admin', N'ProjectBoss', NULL, NULL, NULL, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 0)
                           GO
                           INSERT [dbo].[Person] ([PersonId], [PersonCode], [UserId], [FirstName], [LastName], [Company], [Role], [Country], [CreatedDate], [IsActive]) VALUES (N'bb874f08-556c-4aca-b514-3933a2de2cce', 1002, N'65fb4124-77a2-4a78-858a-890faa5912c6', N'Vanessa', N'Hale', NULL, NULL, NULL, CAST(N'2021-03-29T19:43:15.7042583' AS DateTime2), 1)
                           GO
                           INSERT [dbo].[Person] ([PersonId], [PersonCode], [UserId], [FirstName], [LastName], [Company], [Role], [Country], [CreatedDate], [IsActive]) VALUES (N'2070a98e-6002-476d-865c-3b7edfa02c79', 1005, N'e27d3b25-04fd-45f1-b32e-14c113b3de0a', N'Alex', N'Schneider', NULL, NULL, NULL, CAST(N'2021-03-29T19:44:24.4944693' AS DateTime2), 1)
                           GO
                           INSERT [dbo].[Person] ([PersonId], [PersonCode], [UserId], [FirstName], [LastName], [Company], [Role], [Country], [CreatedDate], [IsActive]) VALUES (N'b8b06995-a3ac-4eee-8476-738d97bdaf66', 1003, N'90364db0-b11c-47f7-a73b-6a925e43df2f', N'Sebastian', N'Henry', NULL, NULL, NULL, CAST(N'2021-03-29T19:43:34.3005181' AS DateTime2), 1)
                           GO
                           INSERT [dbo].[Person] ([PersonId], [PersonCode], [UserId], [FirstName], [LastName], [Company], [Role], [Country], [CreatedDate], [IsActive]) VALUES (N'965a468f-a41f-4cbd-925a-7ab1b2ccb700', 1004, N'41fda830-08c6-4e69-be0d-11f872f3f1f7', N'Herman', N'Carlson', NULL, NULL, NULL, CAST(N'2021-03-29T19:44:01.4893038' AS DateTime2), 1)
                           GO
                           INSERT [dbo].[Person] ([PersonId], [PersonCode], [UserId], [FirstName], [LastName], [Company], [Role], [Country], [CreatedDate], [IsActive]) VALUES (N'fed79d4b-dd43-41b1-be02-b9c1a19ba15d', 1001, N'be5361e4-d467-4809-aed9-0b2d8711b52c', N'Alex', N'McDaniels', NULL, NULL, NULL, CAST(N'2021-03-29T19:42:58.7110314' AS DateTime2), 1)
                           GO
                           SET IDENTITY_INSERT [dbo].[Person] OFF
                           GO";

            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
