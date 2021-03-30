using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectBoss.Core.Entities;
using ProjectBoss.Core.Entities.Enums;
using System;
using System.Linq;

namespace ProjectBoss.Infrastructure.Data.DatabaseContext.ModelConfiguration
{
    public class PriorityConfiguration : IEntityTypeConfiguration<Priority>
    {
        public void Configure(EntityTypeBuilder<Priority> builder)
        {
            builder.HasData(
                Enum.GetValues(typeof(EPriorities))
                    .Cast<EPriorities>()
                    .Select(e => new Priority()
                    {
                        PriorityId = (int)e,
                        Name = e.ToString()
                    })
            );
        }
    }
}
