using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDirectory.Models.Directory;
using WebDirectory.Models.UnitOfWork;

namespace WebDirectory.Models.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Files> Files { get; }
        IRepository<FileExtension> FileExtensions{ get;}
        IRepository<Folder> Folders { get;}

        void Save();

    }
}
