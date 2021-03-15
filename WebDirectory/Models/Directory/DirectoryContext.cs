   using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebDirectory.Models.Directory
{   
    /// <summary>
    /// Контекст данных 
    /// </summary>
    public class DirectoryContext : DbContext
    {
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<FileExtension> FileExtensions { get; set; }


        static DirectoryContext()
        {
            Database.SetInitializer<DirectoryContext>(new AppDbInitializer());
        }

    }
}