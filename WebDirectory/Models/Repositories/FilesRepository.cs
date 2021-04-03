using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebDirectory.Models.Directory;

namespace WebDirectory.Models.Repository
{
    public class FilesRepository : IRepository<Files>
    {
        private DirectoryContext db;

        public FilesRepository(DirectoryContext context)
        {
            db = context;
        }

        public void Create(Files files)
        {
            db.Files.Add(files);
        }

        public void Delete(int id)
        {
            Files files = db.Files.Find(id);
            if (files != null)
                db.Files.Remove(files);
        }

        public Files Get(int id)
        {
            return db.Files.Find(id);
        }

        public IEnumerable<Files> GetAll()
        {
            return db.Files.ToList();
        }

        public void Update(Files files)
        {
            db.Entry(files).State = EntityState.Modified;
        }
    }
}