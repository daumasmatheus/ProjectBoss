using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectBoss.Api.Dtos
{
    public class TaskDto
    {
        public Guid TaskId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid AttendantId { get; set; }
        public Guid AuthorId { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ConclusionDate { get; set; }
        public DateTime? ConcludedDate { get; set; }
        public bool IsComplete { get { return StatusId == 3; } }
        public string Status 
        {
            get
            {
                return StatusId switch
                {
                    1 => "Planejada",
                    2 => "Em andamento",
                    3 => "Em pausa",
                    4 => "Concluída",
                    _ => "",
                };
            }
        }
        public string Priority
        {
            get
            {
                return PriorityId switch
                {
                    1 => "Baixa",
                    2 => "Normal",
                    3 => "Alta",
                    _ => "",
                };
            }
        }
        public bool Removed { get; set; }

        public PersonFullDto Attendant { get; set; }
        public PersonBasicDto Author { get; set; }
        public ProjectSimpleViewDto Project { get; set; }
        public ICollection<CommentDto> Comments { get; set; }
    }

    public class CreateTaskDto
    {
        public Guid TaskId { get; set; } = Guid.NewGuid();
        public Guid? ProjectId { get; set; }

        [Required(ErrorMessage = "O Id do responsavel pela tarefa é obrigatório")]
        public Guid AttendantId { get; set; }

        [Required(ErrorMessage = "O Id do autor da tarefa é obrigatório")]
        public Guid AuthorId { get; set; }

        [Required(ErrorMessage = "O Id do status da tarefa é obrigatório")]
        public int StatusId { get; set; }

        [Required(ErrorMessage = "O Id da prioridade da tarefa é obrigatório")]
        public int PriorityId { get; set; }

        [Required(ErrorMessage = "O titulo da tarefa é obrigatório")]
        public string Title { get; set; }

        [Required(ErrorMessage = "A descrição da tarefa é obrigatório")]
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ConclusionDate { get; set; }
        public DateTime? ConcludedDate { get; set; }
        public bool Removed { get; set; } = false;

        public PersonFullDto Attendant { get; set; }
        public PersonFullDto Author { get; set; }
    }

    public class CreateProjectTaskDto
    {
        public Guid TaskId { get; set; } = Guid.NewGuid();
        public Guid? ProjectId { get; set; }
        public Guid AttendantId { get; set; }
        public Guid AuthorId { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ConclusionDate { get; set; }
        public DateTime? ConcludedDate { get; set; }
        public bool Removed { get; set; } = false;

        public PersonBasicDto Attendant { get; set; }
    }

    public class EditTaskDto
    {
        [Required(ErrorMessage = "O Id da tarefa deve ser informado")]
        public Guid TaskId { get; set; }

        [Required(ErrorMessage = "O Id do responsável da tarefa deve ser informado")]
        public Guid AttendantId { get; set; }

        [Required(ErrorMessage = "O Id do autor da tarefa deve ser informado")]
        public Guid AuthorId { get; set; }

        [Required(ErrorMessage = "O Id do status da tarefa deve ser informado")]
        public int StatusId { get; set; }

        [Required(ErrorMessage = "O Id da prioridade da tarefa deve ser informado")]
        public int PriorityId { get; set; }

        [Required(ErrorMessage = "O titulo da tarefa deve ser informado")]
        public string Title { get; set; }

        [Required(ErrorMessage = "A descrição da tarefa deve ser informado")]
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public DateTime? ConclusionDate { get; set; }
        public DateTime? ConcludedDate { get; set; }

        public PersonBasicDto Attendant { get; set; }
        public PersonBasicDto Author { get; set; }
    }

    public class NewTaskMinDto
    {
        public Guid PersonId { get; set; }
        public string UserId { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ConclusionDate { get; set; }
    }

    public class TaskSimpleViewDto
    {
        public Guid TaskId { get; set; }        
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public string Title { get; set; }        
        public DateTime? ConclusionDate { get; set; }
        public bool IsComplete { get { return StatusId == 3; } }
        public string Status
        {
            get
            {
                return StatusId switch
                {
                    1 => "Planejada",
                    2 => "Em andamento",
                    3 => "Em pausa",
                    4 => "Concluída",
                    _ => "",
                };
            }
        }
        public string Priority
        {
            get
            {
                return PriorityId switch
                {
                    1 => "Baixa",
                    2 => "Normal",
                    3 => "Alta",
                    _ => "",
                };
            }
        }
        public bool Removed { get; set; }        
    }
}
