namespace WebDirectory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWebDirectoryMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Расширение файлов",
                c => new
                    {
                        КодТипаФайла = c.Int(nullable: false, identity: true),
                        Тип = c.String(maxLength: 50),
                        Иконка = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.КодТипаФайла);
            
            CreateTable(
                "dbo.Файлы",
                c => new
                    {
                        КодФайла = c.Int(nullable: false, identity: true),
                        Название = c.String(maxLength: 255),
                        Описание = c.String(maxLength: 4000),
                        КодТипаФайла = c.Int(nullable: false),
                        КодПапки = c.Int(nullable: false),
                        Контент = c.String(),
                    })
                .PrimaryKey(t => t.КодФайла)
                .ForeignKey("dbo.Расширение файлов", t => t.КодТипаФайла, cascadeDelete: true)
                .ForeignKey("dbo.Папки", t => t.КодПапки, cascadeDelete: true)
                .Index(t => t.КодТипаФайла)
                .Index(t => t.КодПапки);
            
            CreateTable(
                "dbo.Папки",
                c => new
                    {
                        КодПапки = c.Int(nullable: false, identity: true),
                        Название = c.String(maxLength: 255),
                        КодРодительскойПапки = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.КодПапки);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Файлы", "КодПапки", "dbo.Папки");
            DropForeignKey("dbo.Файлы", "КодТипаФайла", "dbo.Расширение файлов");
            DropIndex("dbo.Файлы", new[] { "КодПапки" });
            DropIndex("dbo.Файлы", new[] { "КодТипаФайла" });
            DropTable("dbo.Папки");
            DropTable("dbo.Файлы");
            DropTable("dbo.Расширение файлов");
        }
    }
}
