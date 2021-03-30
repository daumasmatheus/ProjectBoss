using ProjectBoss.Core.Entities;
using System;

namespace ProjectBoss.Domain.Entities
{
    public class PersonInProject
    {
        public int Id { get; set; }
        public Guid PersonId { get; set; }
        public Person Person { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
