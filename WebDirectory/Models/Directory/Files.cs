using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace WebDirectory.Models.Directory
{
    [Table("Файлы")]
    public class Files
    {
        [Key]
        [Column("КодФайла")]
        public int FileCode { get; set; }
        [Column("Название", TypeName = "nvarchar")]
        [MaxLength(255)]
        public string Name { get; set; }
        [Column("Описание", TypeName = "nvarchar")]
        public string Description { get; set; }
        [Column("КодТипаФайла")]
        public int TypeCodeOfTheFile { get; set; }
        [Column("КодПапки")]
        public int FolderCode { get; set; }
        [Column("Контент", TypeName = "nvarchar")]
        [MaxLength]
        public string Content { get; set; }

        [ForeignKey("FolderCode")]
        public virtual Folder Folder {get;set;}
        [ForeignKey("TypeCodeOfTheFile")]
        public virtual FileExtension FileExtensions { get; set; }
 
    }
}