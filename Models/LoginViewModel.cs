using System.ComponentModel.DataAnnotations;

namespace Auth.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "E-mail")]
    public string Email { get; set; }

    [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
    [DataType(DataType.Password)]
    public string Senha { get; set; }

    public bool Lembrar { get; set; }
}