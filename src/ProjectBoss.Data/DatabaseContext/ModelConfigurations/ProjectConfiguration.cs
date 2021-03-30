using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectBoss.Core.Entities;

namespace ProjectBoss.Infrastructure.Data.DatabaseContext.ModelConfiguration
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Project");

            builder.HasMany(t => t.Tasks)
                   .WithOne(p => p.Project);

            builder.HasOne(a => a.Author)
                   .WithMany(p => p.Projects)
                   .HasForeignKey(a => a.AuthorId);
        }
    }
}
