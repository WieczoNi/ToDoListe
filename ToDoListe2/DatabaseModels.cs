using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ViewModel.ViewModel;

namespace ToDoListe2
{
    public class DatabaseModels
    {
        

        public class SavesContext : DbContext
        {

            public DbSet<_saves.Saves> SavedDB { get; set; }
            public string DbPath { get; }
            public SavesContext()
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = Environment.GetFolderPath(folder);
                DbPath = System.IO.Path.Join(path, "saves.db");
            }
            protected override void OnConfiguring(DbContextOptionsBuilder options)

            {
                options.UseSqlite($"Data Source={DbPath}");
            }
        }
    }
}
