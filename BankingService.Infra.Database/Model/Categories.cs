using BankingService.Infra.Database.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Model
{
    internal class Categories
    {
        public static string Path => "Database/Categories.csv";
        public static string Header => "Id;Name";

        public Dictionary<int, Categorie> Data { get; }
        private Categories(Dictionary<int, Categorie> data)
        {
            this.Data = data;
        }

        public static Categories Load(IFileSystemService fileSystemService)
        {
            return new Categories(
                fileSystemService
                .ReadAllLines(Path)
                .Skip(1)
                .Select(l => l.Split(";"))
                .ToDictionary(s => int.Parse(s[0]), s => new Categorie(int.Parse(s[0]), s[1]))
                );
        }
    }

    internal class Categorie
    {
        public Categorie(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
