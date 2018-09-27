using Spotless.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;


namespace Spotless.Controllers
{
    public class DeviceGroupController : BaseController
    {

        private DeviceGroupRepository rpstry = new DeviceGroupRepository();
        private string sectionName = "DeviceGroup";
        private ServiceRepository rpstryService = new ServiceRepository();

        private CustomerRepository rpstryCustomer = new CustomerRepository();
        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
            if (permissions.CanCreate)
            {
                var resultsToSearchFrom = rpstryService.GetAll();
                ViewBag.LoggedUser = loggedUser;
                ViewBag.Customers = new SelectList(rpstryCustomer.GetAll(), "id", "fullName");
                ViewBag.Services = new SelectList(resultsToSearchFrom, "id", "title");
                ViewBag.Permissions = permissions;
                return View("Details");
            }
            else
            {
                return View("Error", "You do not have Orders to access this section.");
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(DeviceGroup entry)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canCreate);
            if (hasPermissions)
            {
                try
                {
                    entry.Latitude = Convert.ToDouble(Request["googleMapLat"]);
                    entry.Longitude = Convert.ToDouble(Request["googleMapLong"]);
                    rpstry.Add(entry);
                    rpstry.Save();
                    return RedirectToAction("Index", new { thisid = entry.id });
                }
                catch (Exception e)
                {
                    var resultsToSearchFrom = rpstryService.GetAll();
                    ViewBag.LoggedUser = loggedUser;
                    ViewBag.Customers = new SelectList(rpstryCustomer.GetAll(), "id", "fullName");
                    ViewBag.Services = new SelectList(resultsToSearchFrom, "id", "title");
                    //ViewBag.Permissions = permissions;
                    ModelState.AddModelError("", "");
                    return View("details", entry);
                }
            }
            else
            {
                return View("Error", "You do not have Orders to access this section.");
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
                return View("Error", "You do not have Orders to access this section.");
            }
        }

        public ActionResult ReadData([DataSourceRequest] DataSourceRequest request)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canRead);
            if (hasPermissions)
            {
                var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
                if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
                {
                    return Json(rpstry.GetAll().Where(x => x.customerId == loggedUser.customerId).Select(x => new
                    {
                        id = x.id,
                        title = x.title,
                        comPort = x.comPort,
                        ServiceOfDeviceGroup = x.ServiceOfDeviceGroup,
                        CustomerOfDeviceGroup = x.CustomerOfDeviceGroup
                    }).ToDataSourceResult(request));
                }
                else
                {


                    return Json(rpstry.GetAll().Select(x => new
                    {
                        id = x.id,
                        title = x.title,
                        comPort = x.comPort,
                        ServiceOfDeviceGroup = x.ServiceOfDeviceGroup,
                        CustomerOfDeviceGroup = x.CustomerOfDeviceGroup
                    }).ToDataSourceResult(request));
                }
            }
            else
            {
                return View("Error", "You do not have Orders to access this section.");
            }

        }

        public JsonResult GetDeviceGroups()
        {
            var resultsToSearchFrom = rpstry.GetAll();
            return Json(rpstry.GetAll().Select(x => new
            {
                TEXT = x.title,
                VALUE = x.id.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Update
        [HttpGet]
        public ActionResult Update(int id)
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
            if (permissions.CanUpdate)
            {
                var resultsToSearchFrom = rpstryService.GetAll();
                ViewBag.LoggedUser = loggedUser;
                ViewBag.Customers = new SelectList(rpstryCustomer.GetAll(), "id", "fullName");
                ViewBag.Services = new SelectList(resultsToSearchFrom, "id", "title");
                ViewBag.Permissions = permissions;
                var entry = rpstry.GetBiId(id);
                return View("Details", entry);
            }
            else
            {
                return View("Error", "You do not have Orders to access this section.");
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
                        entry.Latitude = Convert.ToDouble(Request["googleMapLat"]);
                        entry.Longitude = Convert.ToDouble(Request["googleMapLong"]);
                        UpdateModel(entry);
                        rpstry.Save();
                        return RedirectToAction("Index", new { thisid = entry.id });
                    }
                    else
                    {
                        ModelState.AddModelError("", "");
                        return View("details", entry);
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "");
                    return View("details", entry);
                }
            }
            else
            {
                return View("Error", "You do not have Orders to access this section.");
            }

        }

        #endregion

        #region Delete

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, Service entry)
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
                return View("Error", "You do not have Orders to access this section.");
            }


        }
        #endregion
    }
}
