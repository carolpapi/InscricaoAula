using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// ATENÇÃO: Verifique se este 'namespace' bate com o nome do seu projeto!
namespace InscricaoAula.Models
{
    public class AulaColetiva
    {
        // ======== CORREÇÃO (ADICIONE ESTE CONSTRUTOR) ========
        public AulaColetiva()
        {
            // Inicializa a coleção para que ela nunca seja 'null'
            Inscricoes = new HashSet<Inscricao>();
        }
        // =================== FIM DA CORREÇÃO ===================


        [Key]
        public int Id { get; set; } // Chave primária
        public string Nome { get; set; }
        public DateTime DataHora { get; set; }
        public int MaxVagas { get; set; }

        // Propriedade de Navegação: Uma aula tem muitas inscrições
        public virtual ICollection<Inscricao> Inscricoes { get; set; }
    }
}