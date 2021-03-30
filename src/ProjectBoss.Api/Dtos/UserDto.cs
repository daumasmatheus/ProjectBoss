using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectBoss.Api.Dtos
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Campo {0} obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Entre com pelo menos 2 caracteres")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Campo {0} obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Entre com pelo menos 2 caracteres")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Campo {0} obrigatório")]
        [EmailAddress(ErrorMessage = "Campo {0} com formato inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo {0} obrigatório")]
        [StringLength(100, ErrorMessage = "A senha deve ter entre {2} e {1}", MinimumLength = 8)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "As senhas não conferem")]
        public string PasswordConfirmation { get; set; }
    }

    public class ExternalUserDto
    {
        [Required(ErrorMessage = "Campo {0} obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Entre com pelo menos 2 caracteres")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Campo {0} obrigatório")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Entre com pelo menos 2 caracteres")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Campo {0} obrigatório")]
        [EmailAddress(ErrorMessage = "Campo {0} com formato inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O provider deve ser informado")]
        public string Provider { get; set; }

        [Required(ErrorMessage = "O Id do usuário externo deve ser informado")]
        public string ExternalUserId { get; set; }
    }

    public class UserLoginDto
    {
        [Required(ErrorMessage = "Campo {0} obrigatório")]
        [EmailAddress(ErrorMessage = "Campo {0} com formato inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo {0} obrigatório")]
        [StringLength(100, ErrorMessage = "A senha deve ter entre {2} e {1}", MinimumLength = 8)]
        public string Password { get; set; }
    }

    public class ChangePasswordDto
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Campo {0} obrigatório")]
        [StringLength(100, ErrorMessage = "A senha deve ter entre {2} e {1}", MinimumLength = 8)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Campo {0} obrigatório")]
        [StringLength(100, ErrorMessage = "A senha deve ter entre {2} e {1}", MinimumLength = 8)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "As senhas não conferem")]
        public string NewPasswordConfirm { get; set; }
    }

    public class UserLoginTokenResponseDto
    {
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserTokenDto UserToken { get; set; }
    }

    public class UserTokenDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid PersonId { get; set; }
        public IEnumerable<UserClaimDto> Claims { get; set; }
    }

    public class UserClaimDto
    {
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
