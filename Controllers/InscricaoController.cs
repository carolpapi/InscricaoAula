using InscricaoAula.Data; // Importa o DbContext
using InscricaoAula.Models; // Importa os Models
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Importa o .Include()
using System.Linq; // Importa o .Any() e .Count()
using System.Threading.Tasks; // Importa o async/await

// ATENÇÃO: Verifique se este 'namespace' bate com o nome do seu projeto!
namespace InscricaoAula.Controllers
{
    public class InscricaoController : Controller
    {
        // Variável para guardar a conexão com o banco de dados
        private readonly ApplicationDbContext _context;

        // Construtor: Pede ao .NET para "injetar" a conexão do banco
        public InscricaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // (Amanhã vamos criar a parte GET, que mostra a página)
        // public IActionResult Index()
        // {
        //     return View();
        // }


        // =========== LÓGICA DE HOJE (DIA 1) ===========

        // Este método [HttpPost] recebe os dados do formulário
        [HttpPost]
        [ValidateAntiForgeryToken] // (Boa prática de segurança)
        public async Task<IActionResult> Inscrever(Inscricao inscricao)
        {
            // Vamos assumir que só temos 1 aula, a aula de ID=1
            int idAulaFixa = 1;
            inscricao.AulaColetivaId = idAulaFixa;

            try
            {
                // 1. Buscamos a aula e contamos quantas inscrições ela já tem
                var aula = await _context.Aulas
                    .Include(a => a.Inscricoes) // Inclui as inscrições na contagem
                    .FirstOrDefaultAsync(a => a.Id == idAulaFixa);

                if (aula == null)
                {
                    // (Mensagem de feedback para amanhã)
                    return View("Index"); // Recarrega a página com erro
                }

                // 2. VALIDAÇÃO DE VAGAS
                if (aula.Inscricoes.Count >= aula.MaxVagas)
                {
                    // (Mensagem de feedback para amanhã: "Aula Lotada")
                    return View("Index"); // Recarrega a página com erro
                }

                // 3. VALIDAÇÃO DE DUPLICIDADE (O banco já faz isso, mas podemos pré-verificar)
                // (Amanhã melhoramos isso)

                // 4. Se passou nas validações, salva a inscrição
                if (ModelState.IsValid) // Verifica se Nome e Email foram preenchidos
                {
                    _context.Add(inscricao);
                    await _context.SaveChangesAsync();

                    // (Mensagem de feedback para amanhã: "Sucesso!")
                    return RedirectToAction("Index"); // Por enquanto, só recarrega a página
                }
            }
            catch (DbUpdateException) // Pega o erro de duplicidade do banco
            {
                // (Mensagem de feedback para amanhã: "Email já cadastrado")
                return View("Index"); // Recarrega a página com erro
            }

            // Se o ModelState for inválido (ex: email em branco)
            return View("Index");
        }

    }
}