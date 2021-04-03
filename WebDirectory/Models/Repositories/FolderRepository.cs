using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebDirectory.Models.Directory;

namespace WebDirectory.Models.Repository
{
    public class FolderRepository : IRepository<Folder>
    {
        private DirectoryContext db;

        public FolderRepository(DirectoryContext context)
        {
            this.db = context;
        }
        public void Create(Folder folder)
        {
            db.Folders.Add(folder);
        }

        public void Delete(int id)
        {
            Folder folder = db.Folders.Find(id);
            if (folder != null)
                db.Folders.Remove(folder);
        }

        public Folder Get(int id)
        {
            return db.Folders.Find(id);
        }

        public IEnumerable<Folder> GetAll()
        {
            return db.Folders;
        }

        public void Update(Folder folder)
        {
            db.Entry(folder).State = EntityState.Modified;
        }
    }
}