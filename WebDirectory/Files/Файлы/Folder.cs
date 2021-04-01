using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema; 
using System.ComponentModel.DataAnnotations;
using System;

namespace WebDirectory.Models.Directory
{
    [Table("Папки")]
    public class Folder
    {
        [Key]
        [Column("КодПапки")]
        public int FolderCode { get; set; }
        [Column("Название", TypeName = "nvarchar")]
        [MaxLength(255)]
        public string Name { get; set; }
        [Column("КодРодительскойПапки",TypeName = "nvarchar")]
        [MaxLength(255)]
        public  string CodeOfTheParentFolder { get; set; }

        public virtual List<Files> Files { get; set; }

    }
}