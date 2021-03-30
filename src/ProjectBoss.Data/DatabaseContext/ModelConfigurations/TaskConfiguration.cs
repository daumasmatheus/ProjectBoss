using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectBoss.Core.Entities;

namespace ProjectBoss.Infrastructure.Data.DatabaseContext.ModelConfiguration
{
    public class TaskConfiguration : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.ToTable("Task");

            builder.HasOne(p => p.Status)
                   .WithMany()
                   .IsRequired();

            builder.HasOne(p => p.Priority)
                   .WithMany()
                   .IsRequired();

            builder.HasOne(p => p.Attendant)
                   .WithMany(t => t.Tasks)
                   .IsRequired();            

            builder.HasMany(c => c.Comments)
                   .WithOne(t => t.Task);
        }
    }
}
