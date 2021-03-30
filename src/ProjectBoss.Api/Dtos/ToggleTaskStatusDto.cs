using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectBoss.Api.Dtos
{
    public class ToggleTaskStatusDto
    {
        [Required(ErrorMessage = "O Id da tarefa deve ser informado")]
        public Guid TaskId { get; set; }

        [Required(ErrorMessage = "O Id do novo status da tarefa deve ser informado")]
        public int StatusId { get; set; }
    }
}
