using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var dbPath = args.Length > 0 ? args[0] : "ProjetoApi.db";
        if (!System.IO.Path.IsPathRooted(dbPath))
        {
            dbPath = System.IO.Path.GetFullPath(dbPath);
        }

        if (!System.IO.File.Exists(dbPath))
        {
            Console.Error.WriteLine($"Arquivo não encontrado: {dbPath}");
            return 2;
        }

        var cs = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();
        using var conn = new SqliteConnection(cs);
        await conn.OpenAsync();

        Console.WriteLine($"Abrindo banco: {dbPath}\n");

        var tables = new List<string>();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name;";
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                tables.Add(rdr.GetString(0));
            }
        }

        if (tables.Count == 0)
        {
            Console.WriteLine("Nenhuma tabela encontrada.");
            return 0;
        }

        Console.WriteLine("Tabelas encontradas:");
        foreach (var t in tables) Console.WriteLine($" - {t}");
        Console.WriteLine();

        foreach (var table in tables)
        {
            Console.WriteLine($"--- Tabela: {table}");
            using (var ccount = conn.CreateCommand())
            {
                ccount.CommandText = $"SELECT COUNT(*) FROM \"{table}\";";
                var count = (long) (await ccount.ExecuteScalarAsync() ?? 0L);
                Console.WriteLine($"Contagem de registros: {count}");
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT * FROM \"{table}\" LIMIT 5;";
                using var rdr = await cmd.ExecuteReaderAsync();
                var cols = new List<string>();
                for (int i = 0; i < rdr.FieldCount; i++) cols.Add(rdr.GetName(i));
                if (cols.Count == 0)
                {
                    Console.WriteLine("(sem colunas)");
                }
                else
                {
                    Console.WriteLine(string.Join(" | ", cols));
                    while (await rdr.ReadAsync())
                    {
                        var vals = new List<string>();
                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            var v = rdr.IsDBNull(i) ? "NULL" : rdr.GetValue(i)?.ToString();
                            vals.Add(v);
                        }
                        Console.WriteLine(string.Join(" | ", vals));
                    }
                }
            }

            Console.WriteLine();
        }

        return 0;
    }
}
