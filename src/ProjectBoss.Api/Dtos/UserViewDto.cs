using System;

namespace ProjectBoss.Api.Dtos
{
    public class UserViewDto
    {
        public string Id { get; set; }
        public Guid PersonId { get { return Person != null ? Person.PersonId : Guid.Empty; } }
        public string Provider { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public UserRoleDto Role { get; set; }
        public PersonFullDto Person { get; set; }
    }

    public class UserRoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class UserDataDto
    {
        public string Id { get; set; }
        public string Provider { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public UserRoleDto Role { get; set; }
    }
}
