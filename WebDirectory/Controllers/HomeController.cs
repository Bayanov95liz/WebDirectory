using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebDirectory.Models.Directory;
using WebDirectory.Models.TreeView;
using System.IO;
using WebDirectory.Models.UnitOfWork;
using Newtonsoft.Json;

namespace WebDirectory.Controllers
{
    public class HomeController : Controller
    {
        string root;
        UnitOfWork unitOfWork;

        public HomeController()
        {
            root = System.Web.Hosting.HostingEnvironment.MapPath("/Files");
            unitOfWork = new UnitOfWork();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetTreeViewNode()
        {
            List<TreeViewNode> treeViewNode = new List<TreeViewNode>();

            foreach (Folder folder in unitOfWork.Folders.GetAll())
            {
                treeViewNode.Add(new TreeViewNode()
                {
                    id = folder.FolderCode.ToString(),
                    parent = folder.CodeOfTheParentFolder,
                    text = folder.Name,
                    type = "folder"
                });
            }

            foreach (Files files in unitOfWork.Files.GetAll())
            {
                A_attr a_attr = new A_attr() {title = files.Description};
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
            string path, newPath;

            try
            {
                path = PathName(id);
                newPath = path.Substring(0, path.LastIndexOf("\\") + 1) + name;

                Directory.Move(path, newPath);

                if (id.Contains("file") == true)
                {
                    id = id.Replace("file", "");
                    Files files = unitOfWork.Files.Get(Convert.ToInt32(id));
                    files.Name = name;
                    unitOfWork.Files.Update(files);
                    unitOfWork.Save();

                }
                else
                {
                    Folder folder = unitOfWork.Folders.Get(Convert.ToInt32(id));
                    folder.Name = name;
                    unitOfWork.Folders.Update(folder);
                    unitOfWork.Save();
                }


                return Json(true);
            }
            catch(Exception e)
            {
                return Json(e.Message);
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

                unitOfWork.Files.Delete(Convert.ToInt32(id));
                unitOfWork.Save();

                return Json(true);
            }
            catch(Exception e)
            {
                return Json(e.Message);
            }

      
        }

        [HttpPost]
        public ActionResult DeleteFolder(string id)
        {
            string path = PathName(id);

            try
            {
                Directory.Delete(path,true);

                unitOfWork.Folders.Delete(Convert.ToInt32(id));
                unitOfWork.Save();

                return Json(true);
            }
            catch(Exception e)
            {
                return Json(e.Message);
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

                unitOfWork.Folders.Create(new Folder()
                {
                    Name = name + count,
                    CodeOfTheParentFolder = id
                });

                unitOfWork.Save();

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

                            int typeCodeOfTheFile = appDbInitializer.GetTypeCodeOfTheFile(unitOfWork.FileExtensions.GetAll().ToList(), name);
                            string content = appDbInitializer.GetContent(pathFileName);
                            string description = appDbInitializer.GetDescription(content);

                            unitOfWork.Files.Create(new Files()
                            {
                                Name = name,
                                FolderCode = Convert.ToInt32(id),
                                TypeCodeOfTheFile = typeCodeOfTheFile,
                                Content = content,
                                Description = description,
                            });

                            unitOfWork.Save();
                        }
                    }
                }

                return Json(true);

            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
                
        }


        [HttpPost]
        public ActionResult GetContentFile(string id)
        {
            int _id = Convert.ToInt32(id.Replace("file", ""));

            Files files = unitOfWork.Files.Get(_id);

            string content = files.Content; 

            return Json(content, JsonRequestBehavior.AllowGet);
        }

        public FileResult GetStream(string id)
        {
            string path = PathName(id);
            FileStream fs = new FileStream(path, FileMode.Open);
            string file_type = "application/" + path.Substring(path.LastIndexOf(".") + 1);
            string file_name = path.Substring(path.LastIndexOf("\\") + 1);
            return File(fs, file_type,file_name);
        }


        private string PathName(string id)
        {
            string path = "";

            if(id.Contains("file") == true)
            {
                id = id.Replace("file", "");

                Files files = unitOfWork.Files.Get(Convert.ToInt32(id));

                path = path.Insert(0, "\\" + files.Name);

                id = files.FolderCode.ToString();

            }

            while (id != "#")
            {
                Folder folder = unitOfWork.Folders.Get(Convert.ToInt32(id));

                path = path.Insert(0, "\\" + folder.Name);

                id = folder.CodeOfTheParentFolder.ToString();
            }

            path = path.Insert(0, root);

            return path;
        }

       
        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}