using Spotless.Models;
using Spotless.Filters;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Spotless.Controllers
{
    [InitializeSimpleMembership]
    public class RoleController : BaseController
    {
        private RoleRepository rpstry = new RoleRepository();
        private PermissionTypeRepository permissionTypeRpstry = new PermissionTypeRepository();
        private string sectionName = "role";

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
            if (permissions.CanCreate)
            {
                ViewBag.Permissions = permissions;
                ViewBag.Sections = sectionRpstry.GetAllOnMenu();
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
        public ActionResult Create(webpages_Role entry)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canCreate);
            if (hasPermissions)
            {
                try
                {
                    if (Roles.RoleExists(entry.RoleName))
                    {
                        ModelState.AddModelError("", "This role already exists");
                        return View(entry);
                    }
                    else
                    {
                        Roles.CreateRole(entry.RoleName);
                        #region Manage Permissions
                        var allPermissions = Request.Form.AllKeys.Where(n => n.StartsWith("permission_"));
                        rpstry.DeleteAllByRole(entry.RoleId);
                        rpstry.Save();
                        foreach (var item in allPermissions)
                        {
                            var roleId = rpstry.GetAll().FirstOrDefault(d => d.RoleName == entry.RoleName).RoleId;
                            int sectionId = Convert.ToInt32(item.ToString().Replace("permission_", ""));
                            string[] permissiontypesIds = Request.Form.GetValues(item.ToString());
                            rpstry.ManagePermissions(roleId, sectionId, permissiontypesIds);
                        }
                        #endregion
                        return RedirectToAction("Index");
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
                return Json(rpstry.GetAll().Where(d => d.RoleName != ConfigurationManager.AppSettings["SuperAdminRoleName"] && d.RoleName != ConfigurationManager.AppSettings["AdminRoleName"]).Select(x => new
                {
                    RoleId = x.RoleId,
                    RoleName = x.RoleName
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

                if (entry.RoleName == ConfigurationManager.AppSettings["SuperAdminRoleName"] || entry.RoleName == ConfigurationManager.AppSettings["AdminRoleName"])
                {
                    return RedirectToAction("index");
                }

                ViewBag.Sections = sectionRpstry.GetAllOnMenu();
                ViewBag.PermissionTypes = permissionTypeRpstry.GetAll();
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
                        rpstry.Save();

                        #region Manage Permissions
                        var allPermissions = Request.Form.AllKeys.Where(n => n.StartsWith("permission_"));
                        rpstry.DeleteAllByRole(id);
                        rpstry.Save();
                        foreach (var item in allPermissions)
                        {
                            int sectionId = Convert.ToInt32(item.ToString().Replace("permission_", ""));
                            string[] permissiontypesIds = Request.Form.GetValues(item.ToString());
                            rpstry.ManagePermissions(id, sectionId, permissiontypesIds);
                        }
                        #endregion

                        return RedirectToAction("Index");
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
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, webpages_Role entry)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canDelete);
            if (hasPermissions)
            {
                try
                {
                    if (entry.RoleName != ConfigurationManager.AppSettings["SuperAdminRoleName"] && entry.RoleName != ConfigurationManager.AppSettings["AdminRoleName"])
                    {
                        Roles.DeleteRole(entry.RoleName);
                        rpstry.Save();
                    }
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
