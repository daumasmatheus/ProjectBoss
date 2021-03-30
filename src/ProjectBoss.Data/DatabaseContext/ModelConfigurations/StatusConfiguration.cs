using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectBoss.Core.Entities;
using ProjectBoss.Core.Entities.Enums;
using System;
using System.Linq;

namespace ProjectBoss.Infrastructure.Data.DatabaseContext.ModelConfiguration
{
    public class StatusConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.HasData(
                Enum.GetValues(typeof(EStatus))
                    .Cast<EStatus>()
                    .Select(e => new Status()
                    {
                        StatusId = (int)e,
                        Name = e.ToString()
                    })
            );
        }
    }
}
