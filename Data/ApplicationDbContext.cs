using InscricaoAula.Models; // Importa seus models
using Microsoft.EntityFrameworkCore;

namespace InscricaoAula.Data
{
    public class ApplicationDbContext : DbContext
    {
        // O construtor que o .NET usa para injetar a conexão
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Mapeia seus Models para se tornarem tabelas no banco
        public DbSet<AulaColetiva> Aulas { get; set; }
        public DbSet<Inscricao> Inscricoes { get; set; }
    }
}