using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace InscricaoAula.Models
{
    // Isso garante a regra de "UNIQUE(id_aula, email_participante)"
    [Index(nameof(AulaColetivaId), nameof(EmailParticipante), IsUnique = true)]
    public class Inscricao
    {
        [Key]
        public int Id { get; set; } // Chave primária

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais que 100 caracteres.")] // <--
        [RegularExpression(@"^[a-zA-Zá-úÁ-Úâ-ûÂ-Ûã-õÃ-ÕçÇ\s]+$", ErrorMessage = "O nome deve conter apenas letras e espaços.")] // <--
        public string NomeParticipante { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        // Substitui o [EmailAddress] por uma Regex mais rigorosa
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Por favor, insira um formato de email válido (ex: nome@dominio.com).")]
        public string EmailParticipante { get; set; }

        // Chave Estrangeira
        public int AulaColetivaId { get; set; }

        // Propriedade de Navegação: Uma inscrição pertence a uma aula
        public virtual AulaColetiva Aula { get; set; }
    }
}