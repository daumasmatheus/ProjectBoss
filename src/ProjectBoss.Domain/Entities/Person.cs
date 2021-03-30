using ProjectBoss.Domain.Entities;
using ProjectBoss.Domain.Extensions;
using System;
using System.Collections.Generic;

namespace ProjectBoss.Core.Entities
{
    public class Person
    {
        public Guid PersonId { get; set; }
        public int PersonCode { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Role { get; set; }
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        public ApplicationUser User { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<PersonInProject> PersonInProject { get; set; }
    }
}
