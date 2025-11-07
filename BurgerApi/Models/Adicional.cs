using System.ComponentModel.DataAnnotations;

namespace BurgerApiPT.Models
{
    /// <summary>
    /// Representa um item adicional do hambúrguer.
    /// </summary>
    public class Adicional
    {
        /// <summary>Identificador numérico do adicional.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Nome do adicional. Ex.: 'Bacon', 'Queijo Cheddar'.</summary>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 60 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>Preço em reais do adicional.</summary>
        [Range(0, 100, ErrorMessage = "O preço deve estar entre 0 e 100.")]
        public decimal Preco { get; set; }

        /// <summary>Indica se o adicional está disponível para venda.</summary>
        public bool Ativo { get; set; } = true;
    }
}
