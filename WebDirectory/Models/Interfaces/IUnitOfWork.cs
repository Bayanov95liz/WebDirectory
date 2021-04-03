using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDirectory.Models.Directory;

namespace WebDirectory.Models.Interfaces
{
    interface IUnitOfWork : IDisposable
    {
        IRepository<Files> Files { get; set; }
        IRepository<FileExtension> FileExtensions { get; set; }
        IRepository<Folder> Folders { get; set; }

        void Save();
    }
}
