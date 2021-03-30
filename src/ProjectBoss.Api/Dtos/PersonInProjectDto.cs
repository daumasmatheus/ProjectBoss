using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectBoss.Api.Dtos
{
    public class PersonInProjectDto
    {
        public int Id { get; set; }
        public Guid PersonId { get; set; }
        public PersonFullDto Person { get; set; }
        public Guid ProjectId { get; set; }
        public ProjectDto Project { get; set; }
    }

    public class PersonInProjectSimpleDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
    }

    public class PersonInProjectParameterDto
    {
        [Required(ErrorMessage = "Informe o Id do projeto")]
        public Guid ProjectId { get; set; }

        [Required(ErrorMessage = "Informe ao menos o Id de ao menos um participante")]
        public Guid[] AttendantIds { get; set; }
    }
}
