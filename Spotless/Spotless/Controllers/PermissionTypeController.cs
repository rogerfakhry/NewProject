using Spotless.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Spotless.Controllers
{
    public class PermissionTypeController : BaseController
    {
        private PermissionTypeRepository rpstry = new PermissionTypeRepository();
        private string sectionName = "permissiontype";

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
            if (permissions.CanCreate)
            {
                ViewBag.Permissions = permissions;
                return View();
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(PermissionType entry)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canCreate);
            if (hasPermissions)
            {
                try
                {
                    entry.dateCreated = DateTime.Now;
                    entry.dateModified = DateTime.Now;
                    rpstry.Add(entry);
                    rpstry.Save();
                    return RedirectToAction("Index", new { thisid = entry.id });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "");
                    return View(entry);
                }
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }

        }
        #endregion

        #region Read
        public ActionResult Index()
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
            if (permissions.CanRead)
            {
                ViewBag.Permissions = permissions;
                return View();
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }
        }

        public ActionResult ReadData([DataSourceRequest] DataSourceRequest request)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canRead);
            if (hasPermissions)
            {
                return Json(rpstry.GetAll().Select(x => new
                    {
                        id = x.id,
                        title = x.title,
                        dateModified = x.dateModified
                    }).ToDataSourceResult(request));
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }

        }

        #endregion

        #region Update
        [HttpGet]
        public ActionResult Update(int id)
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
            if (permissions.CanUpdate)
            {
                ViewBag.Permissions = permissions;
                var entry = rpstry.GetBiId(id);
                return View(entry);
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }

        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Update(FormCollection coll, int id)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canUpdate);
            if (hasPermissions)
            {
                var entry = rpstry.GetBiId(id);
                try
                {
                    if (ModelState.IsValid)
                    {
                        UpdateModel(entry);
                        entry.dateModified = DateTime.Now;
                        rpstry.Save();
                        return RedirectToAction("Index", new { thisid = entry.id });
                    }
                    else
                    {
                        ModelState.AddModelError("", "");
                        return View(entry);
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "");
                    return View(entry);
                }
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }

        }
        #endregion

        #region Delete

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, PermissionType entry)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canDelete);
            if (hasPermissions)
            {
                try
                {
                    var realEntry = rpstry.GetBiId(entry.id);
                    rpstry.Delete(realEntry);
                    rpstry.Save();
                    return Json(new[] { entry }.ToDataSourceResult(request, ModelState));
                }
                catch (Exception e)
                {
                    return Json(new[] { entry }.ToDataSourceResult(request, ModelState));
                }
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }


        }
        #endregion
    }
}
