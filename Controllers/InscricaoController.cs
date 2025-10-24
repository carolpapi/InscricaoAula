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

        // ESTE É O MÉTODO GET (CARREGA A PÁGINA)
        public async Task<IActionResult> Index()
        {
            int idAulaFixa = 1;

            // 1. Busca a aula (SEM .Include)
            var aula = await _context.Aulas.FirstOrDefaultAsync(a => a.Id == idAulaFixa);

            if (aula == null)
            {
                ViewBag.NomeAula = "Aula não encontrada";
                ViewBag.VagasRestantes = 0;
                ViewBag.MaxVagas = 0;
                ViewBag.DataAula = "Indisponível";
            }
            else
            {
                // 2. Busca a contagem de inscrições SEPARADAMENTE (MAIS EFICIENTE)
                var contagemInscricoes = await _context.Inscricoes
                    .CountAsync(i => i.AulaColetivaId == idAulaFixa);

                // 3. Calcula as vagas
                int vagasRestantes = aula.MaxVagas - contagemInscricoes;

                // Envia os dados para a View (página)
                ViewBag.NomeAula = aula.Nome;
                ViewBag.VagasRestantes = vagasRestantes;
                ViewBag.MaxVagas = aula.MaxVagas;
                ViewBag.DataAula = aula.DataHora.ToString("dd/MM/yyyy 'às' HH:mm");
            }

            return View(new Inscricao());
        }


        // =========== LÓGICA DE HOJE (DIA 1) ===========

        // Este método [HttpPost] recebe os dados do formulário
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inscrever([Bind("NomeParticipante,EmailParticipante")] Inscricao inscricao)
        {
            int idAulaFixa = 1;
            inscricao.AulaColetivaId = idAulaFixa;

            ModelState.Remove(nameof(inscricao.Aula));
            ModelState.Remove(nameof(inscricao.AulaColetivaId));

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Buscamos a aula
                    var aula = await _context.Aulas
                        .FirstOrDefaultAsync(a => a.Id == idAulaFixa);

                    if (aula == null)
                    {
                        TempData["MensagemErro"] = "Erro: Aula não encontrada.";
                        return RedirectToAction("Index");
                    }

                    // 2. VALIDAÇÃO DE VAGAS
                    var contagemInscricoes = await _context.Inscricoes
                        .CountAsync(i => i.AulaColetivaId == idAulaFixa);

                    if (contagemInscricoes >= aula.MaxVagas)
                    {
                        TempData["MensagemErro"] = "Que pena! As vagas para esta aula estão esgotadas.";
                        return RedirectToAction("Index");
                    }

                    // =========== INÍCIO DA CORREÇÃO (TESTE 3) ===========
                    // 3. VALIDAÇÃO DE DUPLICIDADE (Manual)
                    // Verifica se já existe ALGUÉM nesta aula (idAulaFixa) com este email
                    var emailJaInscrito = await _context.Inscricoes
                        .AnyAsync(i => i.AulaColetivaId == idAulaFixa &&
                                        i.EmailParticipante == inscricao.EmailParticipante);

                    if (emailJaInscrito)
                    {
                        // ERRO: Email duplicado
                        TempData["MensagemErro"] = $"O email '{inscricao.EmailParticipante}' já está inscrito nesta aula.";
                        return RedirectToAction("Index");
                    }
                    // ============ FIM DA CORREÇÃO (TESTE 3) ============


                    // 4. Se passou em TUDO, salva a inscrição
                    _context.Add(inscricao);
                    await _context.SaveChangesAsync();

                    TempData["MensagemSucesso"] = $"Inscrição realizada com sucesso, {inscricao.NomeParticipante}! Vemos você lá.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException) // O 'catch' agora é só um backup (para o SQLite)
                {
                    TempData["MensagemErro"] = $"O email '{inscricao.EmailParticipante}' já está inscrito nesta aula.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["MensagemErro"] = $"Ocorreu um erro inesperado: {ex.Message}";
                    return RedirectToAction("Index");
                }
            }

            // Se o ModelState for inválido
            await RecarregarDadosView();
            return View("Index", inscricao);
        }

        // Método auxiliar para recarregar o ViewBag quando o ModelState é inválido
        private async Task RecarregarDadosView()
        {
            int idAulaFixa = 1;

            // 1. Busca a aula (SEM .Include)
            var aula = await _context.Aulas.FirstOrDefaultAsync(a => a.Id == idAulaFixa);

            if (aula != null)
            {
                // 2. Busca a contagem SEPARADAMENTE
                var contagemInscricoes = await _context.Inscricoes
                    .CountAsync(i => i.AulaColetivaId == idAulaFixa);

                int vagasRestantes = aula.MaxVagas - contagemInscricoes;

                // 3. Envia os dados
                ViewBag.NomeAula = aula.Nome;
                ViewBag.VagasRestantes = vagasRestantes;
                ViewBag.MaxVagas = aula.MaxVagas;
                ViewBag.DataAula = aula.DataHora.ToString("dd/MM/yyyy 'às' HH:mm");
            }
        }
    }
}