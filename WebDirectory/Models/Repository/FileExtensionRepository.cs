using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebDirectory.Models.Directory;

namespace WebDirectory.Models.Repository
{
    public class FileExtensionRepository : IRepository<FileExtension>
    {
        private DirectoryContext db;

        public FileExtensionRepository(DirectoryContext context)
        {
            db = context;
        }

        public void Create(FileExtension fileExtension)
        {
            db.FileExtensions.Add(fileExtension);
        }

        public void Delete(int id)
        {
            FileExtension fileExtension = db.FileExtensions.Find(id);
            if (fileExtension != null)
                db.FileExtensions.Remove(fileExtension);
        }

        public FileExtension Get(int id)
        {
            return db.FileExtensions.Find(id);
        }

        public IEnumerable<FileExtension> GetAll()
        {
            return db.FileExtensions;
        }

        public void Update(FileExtension fileExtension)
        {
            db.Entry(fileExtension).State = EntityState.Modified;
        }
    }
}