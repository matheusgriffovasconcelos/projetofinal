using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Auth.Models;

[Index(nameof(Email), IsUnique = true)]
public class UsuarioModel
{
    [Key]
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "E-mail")]
    public string Email { get; set; }

    [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
    [DataType(DataType.Password)]
    public string Senha { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
    [DataType(DataType.Password)]
    [Compare("Senha", ErrorMessage = "Os valores dos campos {0} e {1} devem ser iguais.")]
    [Display(Name = "Confirmação de Senha")]
    public string ConfSenha { get; set; }

    [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
    [Display(Name = "Administrador?")]
    public bool IsAdmin { get; set; }
}