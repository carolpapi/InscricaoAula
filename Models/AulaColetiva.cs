using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace InscricaoAula.Models
{
    public class AulaColetiva
    {
        [Key]
        public int Id { get; set; } // Chave primária
        public string Nome { get; set; }
        public DateTime DataHora { get; set; }
        public int MaxVagas { get; set; }

        // Propriedade de Navegação: Uma aula tem muitas inscrições
        public virtual ICollection<Inscricao> Inscricoes { get; set; }
    }
}