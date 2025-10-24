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
        public string NomeParticipante { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string EmailParticipante { get; set; }

        // Chave Estrangeira
        public int AulaColetivaId { get; set; }

        // Propriedade de Navegação: Uma inscrição pertence a uma aula
        public virtual AulaColetiva Aula { get; set; }
    }
}