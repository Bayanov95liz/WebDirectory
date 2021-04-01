using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDirectory.Models.Directory;
using WebDirectory.Models.Repository;

namespace WebDirectory.Models.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {

        private DirectoryContext db = new DirectoryContext();
        private FilesRepository filesRepository;
        private FolderRepository folderRepository;
        private FileExtensionRepository fileExtensionRepository;

        private bool disposed = false;


        public FilesRepository Files
        {
            get
            {
                if (filesRepository == null)
                    filesRepository = new FilesRepository(db);
                return filesRepository;
            }
        }
        public FolderRepository Folders
        {
            get
            {
                if (folderRepository == null)
                    folderRepository = new FolderRepository(db);
                return folderRepository;
            }
        }
        public FileExtensionRepository FileExtensions
        {
            get
            {
                if (fileExtensionRepository == null)
                    fileExtensionRepository = new FileExtensionRepository(db);
                return fileExtensionRepository;
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