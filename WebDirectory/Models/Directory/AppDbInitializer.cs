using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace WebDirectory.Models.Directory
{
    public class AppDbInitializer : DropCreateDatabaseAlways<DirectoryContext>
    {

        protected override void Seed(DirectoryContext directoryContext)
        {

            List<FileExtension> fileExtensions = GetFileExtension();
            List<Folder> folders = new List<Folder>();
            List<Files> files = new List<Files>();

            string root = System.Web.HttpContext.Current.Server.MapPath("~/Files/");

            if (System.IO.Directory.Exists(root) == true)
            {
                IEnumerable<string> directory = System.IO.Directory.EnumerateDirectories(root, "*", System.IO.SearchOption.AllDirectories);

                int fileID = 1;
                int folderID = 1;

                foreach (string pathName in directory)
                {
                    string pathParent = System.IO.Directory.GetParent(pathName).FullName;
                    string parent = ParentID(folders, pathParent);

                    folders.Add(new Folder() { FolderCode = folderID, Name = pathName, CodeOfTheParentFolder = parent });

                    IEnumerable<string> directoryFiles = System.IO.Directory.EnumerateFiles(pathName);

                    foreach (string pathFileName in directoryFiles)
                    {
                        string fileName = Path.GetFileName(pathFileName);
                        int typeCodeOfTheFile = GetTypeCodeOfTheFile(fileExtensions, fileName);
                        string content = GetContent(pathFileName);
                        string description = GetDescription(content);

                        files.Add(new Files()
                        {
                            FileCode = fileID,
                            Name = fileName,
                            FolderCode = folderID,
                            TypeCodeOfTheFile = typeCodeOfTheFile,
                            Content = content,
                            Description = description,
                        });
                        fileID++;
                    }

                    folderID++;
                }
              
            }

            //Заменяем полный путь папки на имя папки.
            folders = AddFolderName(folders);

            directoryContext.Folders.AddRange(folders);
            directoryContext.FileExtensions.AddRange(fileExtensions);
            directoryContext.Files.AddRange(files);

            directoryContext.SaveChanges();

        }


        public int GetTypeCodeOfTheFile(List<FileExtension> fileExtensions,string name)
        {
            string type;

            if(name.LastIndexOf(".") == -1)
            {
                type = "unknown";
            }
            {
                 type = name.Substring(name.LastIndexOf("."));
            }

            int id =  fileExtensions.FirstOrDefault(x => x.Type == type)?.FileTypeCode ?? 1;

            return id;
        }


        private List<FileExtension> GetFileExtension()
        {
            List<FileExtension> fileExtension = new List<FileExtension>()
            {   
                new FileExtension(){FileTypeCode = 1,Type = "unknown", Icon = "/Content/icons/file-earmark.svg"},
                new FileExtension(){FileTypeCode = 2,Type = ".txt", Icon = "/Content/icons/file-earmark-text.svg" },
                new FileExtension(){FileTypeCode = 3,Type = ".doc", Icon = "/Content/icons/file-earmark-word.svg" },
                new FileExtension(){FileTypeCode = 4,Type = ".zip", Icon = "/Content/icons/file-earmark-zip-fill.svg"},
                new FileExtension(){FileTypeCode = 5,Type = ".rar", Icon = "/Content/icons/file-earmark-zip-fill.svg"},
                new FileExtension(){FileTypeCode = 6,Type = ".arj", Icon = "/Content/icons/file-earmark-zip-fill.svg"},
                new FileExtension(){FileTypeCode = 7,Type = ".bmp", Icon = "/Content/icons/file-earmark-image.svg"},
                new FileExtension(){FileTypeCode = 8,Type = ".jpg", Icon = "/Content/icons/file-earmark-image.svg"},
                new FileExtension(){FileTypeCode = 9,Type = ".gif", Icon = "/Content/icons/file-earmark-image.svg"},
                new FileExtension(){FileTypeCode = 10,Type = ".xls", Icon = "/Content/icons/file-earmark-zip-fill.svg"},
                new FileExtension(){FileTypeCode = 11,Type = ".xml", Icon = "/Content/icons/file-excel.svg"},
                new FileExtension(){FileTypeCode = 12,Type = ".ppt", Icon = "/Content/icons/file-earmark-easel.svg"},
                new FileExtension(){FileTypeCode = 13,Type = ".pps", Icon = "/Content/icons/file-earmark-easel.svg"},
                new FileExtension(){FileTypeCode = 14,Type = ".exe", Icon = "/Content/icons/file-earmark"},
                new FileExtension(){FileTypeCode = 15,Type = ".bat", Icon = "/Content/icons/file-earmark"},
                new FileExtension(){FileTypeCode = 16,Type = ".com", Icon = "/Content/icons/file-earmark"},
                new FileExtension(){FileTypeCode = 17,Type = ".wav", Icon = "/Content/icons/file-earmark-music.svg"},
                new FileExtension(){FileTypeCode = 18,Type = ".mp3", Icon = "/Content/icons/file-earmark-music.svg"},
                new FileExtension(){FileTypeCode = 19,Type = ".avi", Icon = "/Content/icons/file-earmark-music.svg"},
                new FileExtension(){FileTypeCode = 20,Type = ".avi", Icon = "/Content/icons/file-earmark-music.svg"},
                new FileExtension(){FileTypeCode = 21,Type = ".mid", Icon = "/Content/icons/file-earmark-music.svg"},
                new FileExtension(){FileTypeCode = 22,Type = ".css", Icon = "/Content/WebDirectory/Content/icons/file-code.svg"},
                new FileExtension(){FileTypeCode = 23,Type = ".cs", Icon = "/Content/icons/file-code.svg"},
                new FileExtension(){FileTypeCode = 24,Type = ".config", Icon = "/Content/icons/file-code.svg"},
                new FileExtension(){FileTypeCode = 25,Type = ".cshtml", Icon = "/Content/icons/file-code.svg"},
            };

            return fileExtension;
        }

        private string ParentID(List<Folder> folders , string pathPrent)
        {
            return folders.FirstOrDefault(x => x.Name == pathPrent)?.FolderCode.ToString() ?? "#";
        }

        private List<Folder> AddFolderName(List<Folder> folders)
        {
            foreach(Folder folder in folders)
            {
                folder.Name = System.IO.Path.GetFileName(folder.Name);
            }

            return folders;
        }

        public string GetContent(string path)
        {
            string value;

            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                value = sr.ReadToEnd();
            } 

            return value;
        }

        public string GetDescription(string value)
        {
            string description;

            try
            {
                HtmlDocument htmlSnippet = new HtmlDocument();
                htmlSnippet.LoadHtml(value);

                description = htmlSnippet.DocumentNode.SelectNodes("//summary").First().InnerText;

                description = description.Replace("///", "");
            }
            catch
            {
                description = "";
            }
           
            return description;
        }
    }

}