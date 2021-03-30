using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectBoss.Domain.Entities;

namespace ProjectBoss.Infrastructure.Data.DatabaseContext.ModelConfiguration
{
    public class PersonInProjectConfiguration : IEntityTypeConfiguration<PersonInProject>
    {
        public void Configure(EntityTypeBuilder<PersonInProject> builder)
        {
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            builder
                .HasOne(p => p.Person)
                .WithMany(pj => pj.PersonInProject)
                .HasForeignKey(fk => fk.PersonId);

            builder
                .HasOne(pj => pj.Project)
                .WithMany(pp => pp.PersonInProject)
                .HasForeignKey(fk => fk.ProjectId);
        }
    }
}
