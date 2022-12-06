
using System.ComponentModel.DataAnnotations;

namespace Auth.Models;
public class CategoriaModel
{

    [Key]
    [Display(Name = "Código")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
    [StringLength(64, ErrorMessage = "O Campo {0} comporta até {1} caracteres apenas.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; }

}
