using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectBoss.Core.Entities;

namespace ProjectBoss.Infrastructure.Data.DatabaseContext.ModelConfiguration
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Person");

            builder.Property(p => p.PersonCode)
                   .UseIdentityColumn(1000, 1)
                   .Metadata
                   .SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

            builder.HasOne(u => u.User)
                   .WithOne()
                   .HasForeignKey<Person>(p => p.UserId);

            builder.HasMany(t => t.Tasks)
                   .WithOne(p => p.Attendant);            

            builder.HasMany(p => p.Projects)
                   .WithOne(p => p.Author);

            builder.HasMany(c => c.Comments)
                   .WithOne(p => p.Person);
        }
    }
}
