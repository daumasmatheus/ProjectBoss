using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectBoss.Api.Dtos
{
    public class ProjectDto
    {
        public Guid ProjectId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "O Id do autor do projeto deve ser informado")]
        public Guid AuthorId { get; set; }

        [Required(ErrorMessage = "O titulo do projeto deve ser informado")]
        public string Title { get; set; }

        [Required(ErrorMessage = "A descrição do projeto deve ser informado")]
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "A data de inicio do projeto deve ser informado")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "A data de conclusão do projeto deve ser informado")]
        public DateTime ConclusionDate { get; set; }
        public DateTime? ConcludedDate { get; set; }
        public bool Removed { get; set; } = false;

        public List<CreateProjectTaskDto> Tasks { get; set; }
        public PersonFullDto Author { get; set; }
        public List<Guid> AttendantIds { get; set; }
    }

    public class ProjectDataDto
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

        public PersonBasicDto Author { get; set; }
        public ICollection<PersonInProjectDto> PersonInProject { get; set; }
    }

    public class ProjectFullDto
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

        public List<TaskDto> Tasks { get; set; }
        public PersonFullDto Author { get; set; }
        public ICollection<PersonInProjectDto> PersonInProject { get; set; }
    }

    public class ProjectSimpleViewDto
    {
        public Guid ProjectId { get; set; }        
        public string Title { get; set; }
        public string Description { get; set; }        
        public DateTime? ConclusionDate { get; set; }        
    }

    public class ProjectDataForDropdownDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}
