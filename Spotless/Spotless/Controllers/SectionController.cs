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
    public class SectionController : BaseController
    {
        private SectionRepository rpstry = new SectionRepository();
        private RoleRepository roleRpstry = new RoleRepository();
        private PermissionTypeRepository permissionTypeRpstry = new PermissionTypeRepository();
        private string sectionName = "Section";

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
            if (permissions.CanCreate)
            {
                ViewBag.Permissions = permissions;
                ViewBag.Roles = roleRpstry.GetAll();
                ViewBag.PermissionTypes = permissionTypeRpstry.GetAll();
                return View();
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(Section entry)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canCreate);
            if (hasPermissions)
            {
                try
                {
                    entry.dateCreated = DateTime.Now;
                    entry.dateModified = DateTime.Now;
                    entry.priority = rpstry.GetMaxPriority() + 1;
                    rpstry.Add(entry);
                    rpstry.Save();

                    #region Manage Permissions
                    var allPermissions = Request.Form.AllKeys.Where(n => n.StartsWith("permission_"));
                    rpstry.DeleteAllBySection(entry.id);
                    rpstry.Save();
                    foreach (var item in allPermissions)
                    {
                        int roleId = Convert.ToInt32(item.ToString().Replace("permission_", ""));
                        string[] permissiontypesIds = Request.Form.GetValues(item.ToString());
                        rpstry.ManagePermissions(entry.id, roleId, permissiontypesIds);
                    }
                    #endregion

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
                ViewBag.Roles = roleRpstry.GetAll();
                ViewBag.PermissionTypes = permissionTypeRpstry.GetAll();
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

                        #region Manage Permissions
                        var allPermissions = Request.Form.AllKeys.Where(n => n.StartsWith("permission_"));
                        rpstry.DeleteAllBySection(id);
                        rpstry.Save();
                        foreach (var item in allPermissions)
                        {
                            int roleId = Convert.ToInt32(item.ToString().Replace("permission_", ""));
                            string[] permissiontypesIds = Request.Form.GetValues(item.ToString());
                            rpstry.ManagePermissions(id, roleId, permissiontypesIds);
                        }
                        #endregion

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

        public string SortGrid(int newIndex, int oldIndex, int id)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canSort);
            if (hasPermissions)
            {
                return rpstry.SortGrid(newIndex, oldIndex, id);
            }
            else
            {
                return "You do not have permissions to access this section.";
            }
        }
        #endregion

        #region Delete

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, Section entry)
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