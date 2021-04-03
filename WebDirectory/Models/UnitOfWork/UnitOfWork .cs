using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDirectory.Models.Directory;
using WebDirectory.Models.Interfaces;
using WebDirectory.Models.Repository;


namespace WebDirectory.Models.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {

        private DirectoryContext db;
        private FilesRepository filesRepository;
        private FolderRepository folderRepository;
        private FileExtensionRepository fileExtensionRepository;

        public UnitOfWork(string connectionString)
        {
            db = new DirectoryContext(connectionString);
        }

        private bool disposed = false;

        public IRepository<Files> Files
        {
            get
            {
                if (filesRepository == null)
                    filesRepository = new FilesRepository(db);
                return filesRepository;
            }
        }
        public IRepository<FileExtension> FileExtensions
        {
            get 
            {

                if (fileExtensionRepository == null)
                    fileExtensionRepository = new FileExtensionRepository(db);
                return fileExtensionRepository;
            }
        }


        public IRepository<Folder> Folders
        {
            get
            {
                if (folderRepository == null)
                    folderRepository = new FolderRepository(db);
                return folderRepository;
        }
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}