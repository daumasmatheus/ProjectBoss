using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectBoss.Api.Dtos
{
    public class CommentDto
    {
        public Guid CommentId { get; set; }
        public Guid PersonId { get; set; }
        public Guid TaskId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        public PersonFullDto Person { get; set; }
        public TaskDto Task { get; set; }
    }

    public class CommentSimpleDto
    {
        public Guid CommentId { get; set; }
        public Guid PersonId { get; set; }
        public Guid TaskId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }        
    }

    public class NewCommentDto
    {
        public Guid CommentId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "O conteúdo do comentário é obrigatório")]
        public string Content { get; set; }

        [Required(ErrorMessage = "O Id da Pessoa é obrigatório")]
        public Guid PersonId { get; set; }

        [Required(ErrorMessage = "O Id da Tarefa é obrigatório")]
        public Guid TaskId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
