using ProjectBoss.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBoss.Core.Entities
{
    public class Project
    {
        public Guid ProjectId { get; set; }
        public Guid AuthorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ConclusionDate { get; set; }
        public DateTime? ConcludedDate { get; set; }
        public bool Removed { get; set; }
        public DateTime? RemovedDate { get; set; }

        public List<Task> Tasks { get; set; }
        public Person Author { get; set; }
        public ICollection<PersonInProject> PersonInProject { get; set; }
    }
}
