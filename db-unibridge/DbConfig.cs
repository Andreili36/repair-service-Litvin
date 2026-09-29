using System;
using System.IO;
using System.Text.Json;

namespace RepairService.Db
{
    public class DbConfig
    {
        public string Host { get; set; } = "";
        public int Port { get; set; }
        public string Name { get; set; } = "";
        public string User { get; set; } = "";
        public string Password { get; set; } = "";

        public static DbConfig Load()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "app-config.json");
            if (!File.Exists(path))
                throw new FileNotFoundException("Не найден app-config.json рядом с .exe", path);

            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            var d = doc.RootElement.GetProperty("database");

            return new DbConfig
            {
                Host = d.GetProperty("host").GetString() ?? "",
                Port = d.GetProperty("port").GetInt32(),
                Name = d.GetProperty("name").GetString() ?? "",
                User = d.GetProperty("user").GetString() ?? "",
                Password = d.GetProperty("password").GetString() ?? ""
            };
        }
    }
}