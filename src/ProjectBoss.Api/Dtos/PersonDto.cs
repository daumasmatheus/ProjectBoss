using ProjectBoss.Domain.Extensions;
using System;
using System.Collections.Generic;

namespace ProjectBoss.Api.Dtos
{
    public class CreatePersonDto
    {
        public Guid PersonId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public ApplicationUser User { get; set; }
    }

    public class PersonDataDto
    {
        public Guid PersonId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class EditPersonDataDto
    {
        public Guid PersonId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Company { get; set; }
        public string Country { get; set; }
    }

    public class PersonBasicDto
    {
        public Guid PersonId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }

    public class PersonDto
    {
        public Guid PersonId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Role { get; set; }
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }        
    }

    public class PersonFullDto
    {
        public Guid PersonId { get; set; }
        public int PersonCode { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public string Role { get; set; }
        public string Company { get; set; }
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public int TasksAsigned { get { return Tasks.Count; } }

        public ICollection<TaskSimpleViewDto> Tasks { get; set; }
        public ICollection<ProjectDto> Projects { get; set; }
    }    
}
