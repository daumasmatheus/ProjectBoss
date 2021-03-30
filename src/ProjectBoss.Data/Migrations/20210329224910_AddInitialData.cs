using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectBoss.Data.Migrations
{
    public partial class AddInitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = @"USE [TaskBoss]
                           GO
                           INSERT [dbo].[Project] ([ProjectId], [AuthorId], [Title], [Description], [CreatedDate], [StartDate], [ConclusionDate], [ConcludedDate], [Removed], [RemovedDate]) VALUES (N'3f8361b8-edd5-4b1b-9b96-e6bf12799b94', N'a3fd4b32-927d-4538-b6f5-29ce6fd717bd', N'Lorem ipsum dolor sit amet', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis quam enim, suscipit a lectus a, rhoncus commodo lectus. Ut varius ornare mauris', CAST(N'2021-03-29T19:51:32.6327827' AS DateTime2), CAST(N'2021-03-31T03:00:00.0000000' AS DateTime2), CAST(N'2021-04-30T03:00:00.0000000' AS DateTime2), NULL, 0, NULL)
                           GO
                           SET IDENTITY_INSERT [dbo].[PersonInProject] ON 
                           GO
                           INSERT [dbo].[PersonInProject] ([Id], [PersonId], [ProjectId]) VALUES (1, N'965a468f-a41f-4cbd-925a-7ab1b2ccb700', N'3f8361b8-edd5-4b1b-9b96-e6bf12799b94')
                           GO
                           INSERT [dbo].[PersonInProject] ([Id], [PersonId], [ProjectId]) VALUES (2, N'b8b06995-a3ac-4eee-8476-738d97bdaf66', N'3f8361b8-edd5-4b1b-9b96-e6bf12799b94')
                           GO
                           INSERT [dbo].[PersonInProject] ([Id], [PersonId], [ProjectId]) VALUES (3, N'bb874f08-556c-4aca-b514-3933a2de2cce', N'3f8361b8-edd5-4b1b-9b96-e6bf12799b94')
                           GO
                           INSERT [dbo].[PersonInProject] ([Id], [PersonId], [ProjectId]) VALUES (4, N'fed79d4b-dd43-41b1-be02-b9c1a19ba15d', N'3f8361b8-edd5-4b1b-9b96-e6bf12799b94')
                           GO
                           INSERT [dbo].[PersonInProject] ([Id], [PersonId], [ProjectId]) VALUES (5, N'2070a98e-6002-476d-865c-3b7edfa02c79', N'3f8361b8-edd5-4b1b-9b96-e6bf12799b94')
                           GO
                           SET IDENTITY_INSERT [dbo].[PersonInProject] OFF
                           GO
                           INSERT [dbo].[Task] ([TaskId], [ProjectId], [AttendantId], [AuthorId], [StatusId], [PriorityId], [Title], [Description], [CreatedDate], [UpdatedDate], [ConclusionDate], [ConcludedDate], [Removed], [RemovedDate]) VALUES (N'cedf4386-bbbd-49fc-ba72-2e9ede4471f5', N'3f8361b8-edd5-4b1b-9b96-e6bf12799b94', N'fed79d4b-dd43-41b1-be02-b9c1a19ba15d', N'a3fd4b32-927d-4538-b6f5-29ce6fd717bd', 1, 1, N'Lorem ipsum dolor sit amet', N'Lorem ipsum dolor sit amet', CAST(N'2021-03-29T19:51:32.6392051' AS DateTime2), NULL, CAST(N'2021-03-30T03:00:00.0000000' AS DateTime2), NULL, 0, NULL)
                           GO
                           INSERT [dbo].[Task] ([TaskId], [ProjectId], [AttendantId], [AuthorId], [StatusId], [PriorityId], [Title], [Description], [CreatedDate], [UpdatedDate], [ConclusionDate], [ConcludedDate], [Removed], [RemovedDate]) VALUES (N'e12e924c-a2d9-4981-8ec5-3c4251911a62', N'3f8361b8-edd5-4b1b-9b96-e6bf12799b94', N'965a468f-a41f-4cbd-925a-7ab1b2ccb700', N'a3fd4b32-927d-4538-b6f5-29ce6fd717bd', 1, 2, N'Lorem ipsum dolor sit amet, consectetur adipiscing elit', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit', CAST(N'2021-03-29T20:01:59.0630875' AS DateTime2), NULL, CAST(N'2021-04-22T03:00:00.0000000' AS DateTime2), NULL, 0, NULL)
                           GO
                           INSERT [dbo].[Task] ([TaskId], [ProjectId], [AttendantId], [AuthorId], [StatusId], [PriorityId], [Title], [Description], [CreatedDate], [UpdatedDate], [ConclusionDate], [ConcludedDate], [Removed], [RemovedDate]) VALUES (N'e6d0f7e1-cd07-451d-ae9b-703b057ecc85', N'3f8361b8-edd5-4b1b-9b96-e6bf12799b94', N'bb874f08-556c-4aca-b514-3933a2de2cce', N'a3fd4b32-927d-4538-b6f5-29ce6fd717bd', 1, 3, N'Lorem ipsum dolor sit amet', N'Lorem ipsum dolor sit amet', CAST(N'2021-03-29T19:51:32.6453123' AS DateTime2), NULL, CAST(N'2021-04-21T03:00:00.0000000' AS DateTime2), NULL, 0, NULL)
                           GO
                           INSERT [dbo].[Task] ([TaskId], [ProjectId], [AttendantId], [AuthorId], [StatusId], [PriorityId], [Title], [Description], [CreatedDate], [UpdatedDate], [ConclusionDate], [ConcludedDate], [Removed], [RemovedDate]) VALUES (N'b9b98803-d721-49a2-a71c-a672e9dfd920', N'3f8361b8-edd5-4b1b-9b96-e6bf12799b94', N'b8b06995-a3ac-4eee-8476-738d97bdaf66', N'a3fd4b32-927d-4538-b6f5-29ce6fd717bd', 1, 2, N'Lorem ipsum dolor sit amet', N'Lorem ipsum dolor sit amet', CAST(N'2021-03-29T19:51:32.6452648' AS DateTime2), NULL, CAST(N'2021-04-08T03:00:00.0000000' AS DateTime2), NULL, 0, NULL)
                           GO
                           INSERT [dbo].[Comment] ([CommentId], [PersonId], [TaskId], [Content], [CreatedDate]) VALUES (N'08612824-4288-4866-85c8-43d4918a137b', N'a3fd4b32-927d-4538-b6f5-29ce6fd717bd', N'cedf4386-bbbd-49fc-ba72-2e9ede4471f5', N'Hello from admin', CAST(N'2021-03-29T20:02:59.8400253' AS DateTime2))
                           GO
                           INSERT [dbo].[Comment] ([CommentId], [PersonId], [TaskId], [Content], [CreatedDate]) VALUES (N'd357e70e-cd6e-4fd9-a56d-d818a6aeeb95', N'a3fd4b32-927d-4538-b6f5-29ce6fd717bd', N'e6d0f7e1-cd07-451d-ae9b-703b057ecc85', N'Hello from admin', CAST(N'2021-03-29T20:03:12.5299137' AS DateTime2))
                           GO";

            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
