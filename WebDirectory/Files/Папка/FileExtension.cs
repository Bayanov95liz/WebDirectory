using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebDirectory.Models.Directory
{
    [Table("Расширение файлов")]
    public class FileExtension
    {
        [Key]
        [Column("КодТипаФайла")]
        public int FileTypeCode { get; set; }
        [Column("Тип", TypeName = "nvarchar")]
        [MaxLength(50)]
        public string Type { get; set; }
        [Column("Иконка", TypeName = "nvarchar")]
        [MaxLength(255)]
        public string Icon { get; set; }
        public virtual List<Files> Files { get; set; }
    }
}