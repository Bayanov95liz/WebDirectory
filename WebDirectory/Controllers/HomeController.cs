using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDirectory.Models.Directory;
using WebDirectory.Models.TreeView;
using Newtonsoft.Json;
using System.IO;
using System.Data.Entity;

namespace WebDirectory.Controllers
{
    public class HomeController : Controller
    {
        DirectoryContext directoryContext = new DirectoryContext();
        string root = System.Web.HttpContext.Current.Server.MapPath("~/Files");

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetTreeViewNode()
        {
            List<TreeViewNode> treeViewNode = new List<TreeViewNode>();
            A_attr a_attr = new A_attr();

            foreach(FileExtension fileExtension in directoryContext.FileExtensions)
            {

            }

            foreach (Folder folder in directoryContext.Folders)
            {
                treeViewNode.Add(new TreeViewNode() 
                { 
                    id = folder.FolderCode.ToString(),
                    parent = folder.CodeOfTheParentFolder, 
                    text = folder.Name,
                    type = "folder" 
                });
            }

            List<Folder> folder1 = directoryContext.Folders.ToList();


            foreach(Files files in directoryContext.Files)
            {
                a_attr = new A_attr() {title = files.Description};
                treeViewNode.Add(new TreeViewNode() 
                {
                    id = "file" + files.FileCode.ToString(), 
                    text = files.Name, 
                    parent = files.FolderCode.ToString(),
                    type = "file",
                    icon = files.FileExtensions.Icon,
                    a_attr = a_attr 
                });
            }

            return Json(treeViewNode, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Rename(string id,string name)
        {
            try
            {
                string path = PathName(id);

                string newPath = path.Substring(0, path.LastIndexOf("\\") + 1) + name;

                Directory.Move(path, newPath);

                if (id.Contains("file") == true)
                {
                    id = id.Replace("file", "");
                    directoryContext.Files.First(x => x.FileCode.ToString() == id).Name = name;
                }
                else
                {
                    directoryContext.Folders.First(x => x.FolderCode.ToString() == id).Name = name;
                }

                directoryContext.SaveChanges();
                return Json(true);
            }
            catch(Exception e)
            {
                return Json(e.ToString());
            }
        }

        [HttpPost]
        public ActionResult DeleteFile(string id)
        {
            string path = PathName(id);

            id = id.Replace("file","");
            try
            {

                System.IO.File.Delete(path);

                Files file = directoryContext.Files
                    .Where(x => x.FileCode.ToString() == id)
                    .FirstOrDefault();

                directoryContext.Files.Remove(file);
                directoryContext.SaveChanges();

                return Json(true);
            }
            catch(Exception e)
            {
                return Json(e);
            }

      
        }

        [HttpPost]
        public ActionResult DeleteFolder(string id)
        {
            string path = PathName(id);

            try
            {
                Directory.Delete(path,true);

                AppDbInitializer appDbInitializer = new AppDbInitializer();
                appDbInitializer.InitializeDatabase(new DirectoryContext());

                return Json(true);
            }
            catch(Exception e)
            {
                return Json(e);
            }
        }


        [HttpPost]
        public ActionResult CreateFolder(string id, string name)
        {
            string path = PathName(id);

            path = path + "\\" + name;

            string count = "";

            while (Directory.Exists(path + count))
            {
                if (count == "") count = "0";
                count = (Convert.ToInt32(count) + 1).ToString();
            }

            try
            {

                var value =  Directory.CreateDirectory(path + count).Exists;

                directoryContext.Folders.Add(new Folder() { Name = name + count, CodeOfTheParentFolder = id});
                directoryContext.SaveChanges();

                return Json(true);
            }
            catch(Exception e)
            {
                return Json(e.Message);
            }
            
      
        }
        [HttpPost]
        public ActionResult Upload()
        {
            try
            {
                string id = Request.Form["id"];
                string path = PathName(id);


                foreach (string file in Request.Files)
                {
                    var upload = Request.Files[file];

                    if (upload != null)
                    {

                        AppDbInitializer appDbInitializer = new AppDbInitializer();
                        List<FileExtension> fileExtensions = directoryContext.FileExtensions.ToList();
                        string name = upload.FileName;
                        string pathFileName = path + "\\" + name;

                        if (System.IO.File.Exists(pathFileName) == true)
                        {
                            upload.SaveAs(pathFileName);

                            return Json("Файл успешно заменен!");
                        }
                        else
                        {
                            upload.SaveAs(pathFileName);

                            int typeCodeOfTheFile = appDbInitializer.GetTypeCodeOfTheFile(fileExtensions, name);
                            string content = appDbInitializer.GetContent(pathFileName);
                            string description = appDbInitializer.GetDescription(content);

                            directoryContext.Files.Add(new Files()
                            {
                                Name = name,
                                FolderCode = Convert.ToInt32(id),
                                TypeCodeOfTheFile = typeCodeOfTheFile,
                                Content = content,
                                Description = description,
                            });

                            directoryContext.SaveChanges();
                        }
                    }
                }

                return Json(true);

            }
            catch (Exception e)
            {
                return Json(e.ToString());
            }
                
        }


        [HttpPost]
        public ActionResult GetContentFile(string id)
        {
            id = id.Replace("file", "");

            string content = directoryContext.Files.First(x => x.FileCode.ToString() == id).Content.ToString();

            return Json(content, JsonRequestBehavior.AllowGet);
        }

        public FileResult GetStream(string id)
        {
            string path = PathName(id);
            // Объект Stream
            FileStream fs = new FileStream(path, FileMode.Open);
            string file_type = "application/" + path.Substring(path.LastIndexOf(".") + 1);
            string file_name = path.Substring(path.LastIndexOf("\\") + 1);
            return File(fs, file_type,file_name);
        }

        private string PathName(string id)
        {
            string path = "";
            string name;

            if(id.Contains("file") == true)
            {
                id = id.Replace("file", "");
                name = directoryContext.Files.First(x => x.FileCode.ToString() == id).Name.ToString();

                path = path.Insert(0, "\\" + name);

                id = directoryContext.Files.First(x => x.FileCode.ToString() == id).FolderCode.ToString();

            }

            while (id != "#")
            {
                name = directoryContext.Folders.First(x => x.FolderCode.ToString() == id).Name.ToString();

                path = path.Insert(0, "\\" + name);

                id = directoryContext.Folders.First(x => x.FolderCode.ToString() == id).CodeOfTheParentFolder.ToString();
            }

            path = path.Insert(0, root);

            return path;
        }

    }
}