using Spotless.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageResizer;
using System.Web.Script.Serialization;

namespace Spotless.Controllers
{
    public class EditorController : EditorImageBrowserController
    {
        private const string contentFolderRoot = "~/MCContent/Uploads/";
        private static readonly string[] foldersToCopy = new[] { "~/MCContent/Uploads/Editor" };

        /// <summary>
        /// Gets the base paths from which content will be served.
        /// </summary>
        public override string ContentPath
        {
            get
            {
                return CreateUserFolder();
            }
        }

        private string CreateUserFolder()
        {
            var virtualPath = Path.Combine(contentFolderRoot, "Editor");

            var path = Server.MapPath(virtualPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                foreach (var sourceFolder in foldersToCopy)
                {
                    CopyFolder(Server.MapPath(sourceFolder), path);
                }
            }
            return virtualPath;
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (var file in Directory.EnumerateFiles(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(file));
                System.IO.File.Copy(file, dest);
            }

            foreach (var folder in Directory.EnumerateDirectories(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(folder));
                CopyFolder(folder, dest);
            }
        }
    }

    //[Authorize]
    public class UploadHandlerController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(string id)
        {
            var filename = id;
            var filePath = Server.MapPath(id);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return Json(new { error = String.Empty });
        }

        [HttpGet]
        public void Download(string id)
        {
            var filename = id;
            var filePath = Path.Combine(Server.MapPath("~/Files"), filename);

            var context = HttpContext;

            if (System.IO.File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        [HttpPost]
        public ActionResult UploadFiles(string directory, string inputName)
        {
            #region Default Attributes
            string mainFilesPath = ConfigurationManager.AppSettings["MainFilesPath"].ToString();
            string allowedExtensions = (ConfigurationManager.AppSettings["AllowedExtensions"]).ToString();
            int maxFileSize = Convert.ToInt32(ConfigurationManager.AppSettings["MaxFileSize"]);
            #endregion

            if (directory.StartsWith(mainFilesPath))
            {
                var r = new List<ViewDataUploadFilesResult>();
                if (!Directory.Exists(Server.MapPath(directory)))
                {
                    Directory.CreateDirectory(Server.MapPath(directory));
                }

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file.ContentLength <= maxFileSize)
                    {
                        string fileExt = Path.GetExtension(file.FileName).ToLower();

                        if (allowedExtensions.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries).Contains(fileExt.Replace(".", "")))
                        {
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName.Replace(" ", "-").Replace("&", "-").Replace("~", "-").Replace("*", "-").Replace("--", "-"));
                            string fullFileName = DateTime.Now.ToString("yMMddhhmmssfff") + "~" + fileName + fileExt;
                            var fullrelativePath = Url.Content(directory + "/" + fullFileName);
                            var fullPath = Path.Combine(Server.MapPath(directory) + "/", fullFileName);

                            var statuses = new List<ViewDataUploadFilesResult>();
                            var headers = Request.Headers;

                            if (string.IsNullOrEmpty(headers["X-File-Name"]))
                            {
                                file.SaveAs(fullPath);
                            }
                            else
                            {
                                if (Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
                                var inputStream = file.InputStream;
                                using (var fs = new FileStream(fullFileName, FileMode.Append, FileAccess.Write))
                                {
                                    var buffer = new byte[1024];

                                    var l = inputStream.Read(buffer, 0, 1024);
                                    while (l > 0)
                                    {
                                        fs.Write(buffer, 0, l);
                                        l = inputStream.Read(buffer, 0, 1024);
                                    }
                                    fs.Flush();
                                    fs.Close();
                                }
                            }

                            statuses.Add(new ViewDataUploadFilesResult()
                            {
                                name = fullFileName,
                                prettyName = fileName,
                                size = file.ContentLength,
                                type = file.ContentType,
                                url = fullrelativePath,
                                deleteUrl = "/UploadHandler/Delete?id=" + HttpUtility.UrlEncode(fullrelativePath),
                                thumbnailUrl = fullrelativePath + "?width=48&height=48&mode=max",
                                deleteType = "DELETE",
                                inputName = inputName
                            });

                            JsonResult result = Json(new { files = statuses });
                            result.ContentType = "text/plain";

                            return result;
                        }
                        else
                        {
                            return Json(new { error = "This file type is not allowed" });
                        }
                    }
                    else
                    {
                        return Json(new { error = "This file is too big" });
                    }
                }

                return Json(r);
            }
            else
            {
                return Json(new { error = "You are not allowed to access this directory" });
            }
        }
    }
    public class ViewDataUploadFilesResult
    {
        public string name { get; set; }
        public string prettyName { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string deleteUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string deleteType { get; set; }
        public string inputName { get; set; }
    }
}