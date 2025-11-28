using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProjetoApiPT.Domain.Ports;
using ProjetoApiPT.Infrastructure.Data;
using ProjetoApiPT.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==================== CONFIGURAÇÃO DE SERVIÇOS ====================

// Controladores
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Gestão de Clientes, Propostas e Modelos",
        Version = "v1",
        Description = "API RESTful para gerenciar Clientes, Propostas, Modelos, Envios de Formulários, Processos de Modelos e Usuários."
    });
});

// ==================== BANCO DE DADOS ====================

// String de conexão do appsettings.json, com fallback para SQLite local
var stringConexao = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=app.db";

// Registra o contexto do banco usando SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(stringConexao)
);

// ==================== REPOSITÓRIOS ====================
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IPropostaRepositorio, PropostaRepositorio>();
builder.Services.AddScoped<IModeloRepositorio, ModeloRepositorio>();
builder.Services.AddScoped<IEnvioFormularioRepositorio, EnvioFormularioRepositorio>();
builder.Services.AddScoped<IProcessoModeloRepositorio, ProcessoModeloRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// ==================== CORS ====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodas", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ==================== CONSTRUÇÃO DA APLICAÇÃO ====================
var app = builder.Build();

// ==================== PIPELINE ====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty;
    });
}

// Desabilitar redirecionamento HTTPS em desenvolvimento para evitar problemas
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("PermitirTodas");

app.UseAuthorization(); // Adicionar UseAuthorization é uma boa prática

app.MapControllers();

// ==================== MIGRAÇÕES AUTOMÁTICAS ====================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.Migrate(); // Aplica migrations automaticamente
    }
    catch (System.InvalidOperationException ex)
    {
        // Se houver pending model changes (sem dotnet-ef), continuamos com ajustes manuais
        Console.WriteLine("Aviso: migração automática falhou: " + ex.Message);
    }

    // Garantia extra para ambientes sem dotnet-ef: adiciona colunas necessárias
    try
    {
        var conn = dbContext.Database.GetDbConnection();
        conn.Open();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "PRAGMA table_info('Propostas');";
            var cols = new System.Collections.Generic.List<string>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    cols.Add(reader.GetString(1));
                }
            }

            if (!cols.Contains("Slides"))
            {
                cmd.CommandText = "ALTER TABLE Propostas ADD COLUMN Slides TEXT;";
                cmd.ExecuteNonQuery();
            }
            if (!cols.Contains("PdfUrl"))
            {
                cmd.CommandText = "ALTER TABLE Propostas ADD COLUMN PdfUrl TEXT;";
                cmd.ExecuteNonQuery();
            }
            if (!cols.Contains("ClienteId"))
            {
                cmd.CommandText = "ALTER TABLE Propostas ADD COLUMN ClienteId INTEGER DEFAULT 0;";
                cmd.ExecuteNonQuery();
            }
// ==================== INICIALIZAÇÃO ====================
        }
        conn.Close();
    }
    catch (System.Exception ex)
    {
        // Se algo falhar aqui, não interrompe a aplicação — logs para diagnóstico
        Console.WriteLine("Aviso: não foi possível aplicar alterações manuais na tabela Propostas: " + ex.Message);
    }

    // ==================== SEED DATA ====================
    try
    {
        SeedClientes(dbContext);
    }
    catch (System.Exception ex)
    {
        Console.WriteLine("Aviso: erro ao popular clientes iniciais: " + ex.Message);
    }
}

    // ==================== MÉTODO DE SEED DATA ====================
    static void SeedClientes(AppDbContext dbContext)
    {
        // Se não houver clientes, insere exemplos completos
        if (dbContext.Clientes == null)
            return;

        if (!dbContext.Clientes.Any())
        {
            var clientes = new[]
            {
                new ProjetoApiPT.Domain.Entities.Cliente
                {
                    Nome = "Tech Solutions Brasil",
                    Email = "contato@techsolutions.com.br",
                    Telefone = "+55 11 99999-0001",
                    Mensagem = "Preciso contratar um serviço de UX",
                    PdfGerado = true,
                    DataCadastro = DateTime.UtcNow
                },
                new ProjetoApiPT.Domain.Entities.Cliente
                {
                    Nome = "Consultoria Digital Ltda",
                    Email = "info@consultoriadigital.com.br",
                    Telefone = "+55 21 98888-0002",
                    Mensagem = "Preciso contratar um serviço de UX",
                    PdfGerado = false,
                    DataCadastro = DateTime.UtcNow
                },
                new ProjetoApiPT.Domain.Entities.Cliente
                {
                    Nome = "E-Commerce Pro",
                    Email = "vendas@ecommercepro.com.br",
                    Telefone = "+55 11 97777-0003",
                    Mensagem = "Preciso contratar um serviço de UX",
                    PdfGerado = true,
                    DataCadastro = DateTime.UtcNow
                },
                new ProjetoApiPT.Domain.Entities.Cliente
                {
                    Nome = "Agência Marketing XYZ",
                    Email = "contato@agenciamarketing.com.br",
                    Telefone = "+55 31 96666-0004",
                    Mensagem = "Preciso contratar um serviço de UX",
                    PdfGerado = false,
                    DataCadastro = DateTime.UtcNow
                },
                new ProjetoApiPT.Domain.Entities.Cliente
                {
                    Nome = "Serviços Financeiros Premium",
                    Email = "atendimento@sfpremium.com.br",
                    Telefone = "+55 41 95555-0005",
                    Mensagem = "Preciso contratar um serviço de UX",
                    PdfGerado = true,
                    DataCadastro = DateTime.UtcNow
                }
            };

            dbContext.Clientes.AddRange(clientes);
            dbContext.SaveChanges();
            Console.WriteLine("✅ Clientes iniciais adicionados ao banco de dados!");
            return;
        }

        // Caso já existam clientes, atualiza registros faltantes (Telefone ou Mensagem)
        var faltantes = dbContext.Clientes
            .Where(c => string.IsNullOrWhiteSpace(c.Telefone) || string.IsNullOrWhiteSpace(c.Mensagem))
            .ToList();

        if (faltantes.Count > 0)
        {
            int idx = 1;
            foreach (var c in faltantes)
            {
                if (string.IsNullOrWhiteSpace(c.Telefone))
                {
                    // Gera telefone fictício por índice para não criar duplicatas óbvias
                    c.Telefone = $"+55 11 90000-{1000 + idx}";
                }
                if (string.IsNullOrWhiteSpace(c.Mensagem))
                {
                    c.Mensagem = "Preciso contratar um serviço de UX";
                }
                idx++;
            }
            dbContext.SaveChanges();
            Console.WriteLine($"✅ Atualizados {faltantes.Count} clientes com telefone/mensagem fictícios.");
        }
    }

// ==================== INICIALIZAÇÃO ====================
app.Run();
