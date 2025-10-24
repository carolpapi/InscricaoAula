using InscricaoAula.Data; // Importa seu DbContext
using Microsoft.EntityFrameworkCore; // Importa o EF Core

namespace InscricaoAula
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Adiciona o DbContext ao sistema, lendo a string de conexão do appsettings.json
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // ======== INÍCIO DO CÓDIGO DE SEED (ALIMENTAÇÃO) ========

            // Criamos um "escopo" de serviço para acessar o banco de dados
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

                // Garante que o banco de dados foi criado (ele já foi, mas é uma boa prática)
                context.Database.EnsureCreated();

                // Verifica se já existe alguma aula no banco
                if (!context.Aulas.Any())
                {
                    // Se não existir, cria a nossa aula de exemplo
                    context.Aulas.Add(new InscricaoAula.Models.AulaColetiva
                    {
                        Nome = "Aula de Testes de Software",
                        DataHora = new DateTime(2025, 11, 17, 19, 0, 0), // 17/11/2025 às 19:00
                        MaxVagas = 25
                    });

                    // Salva a mudança no banco
                    context.SaveChanges();
                }
            }
            // ======== FIM DO CÓDIGO DE SEED ========

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}