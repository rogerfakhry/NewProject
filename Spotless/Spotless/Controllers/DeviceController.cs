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
    public class DeviceController : BaseController
    {

        private DeviceRepository rpstry = new DeviceRepository();
        private DeviceGroupRepository rpstryDeviceGroup = new DeviceGroupRepository();
        private ServiceRepository rpstryService = new ServiceRepository();
        private CustomerRepository rpstryCustomer = new CustomerRepository();
        private string sectionName = "Device";
        private DataClassesDataContext db = new DataClassesDataContext();


        #region Create
        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
         
            if (permissions.CanCreate)
            {
                var resultsToSearchFrom = rpstryService.GetAll();
                ViewBag.LoggedUser = loggedUser;
                var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
                if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
                {
                    resultsToSearchFrom = loggedUser.customerId != null ? db.CustomerServices.Where(d => d.customerId == loggedUser.customerId).Select(d => d.Service).AsQueryable() : new List<Service>().AsQueryable();
                }
                else
                {
                    ViewBag.Customers = new SelectList(rpstryCustomer.GetAll(), "id", "fullName");
                }
                var resultsToGroupFrom= db.DeviceGroups.AsQueryable();
                ViewBag.DeviceGroups = new SelectList(resultsToGroupFrom, "id", "title");
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
        public ActionResult Create(Device entry)
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
                    var customerServicesArray = !string.IsNullOrEmpty(Request["Device_Groups"]) ? Request["Device_Groups"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
                    //rpstry.ManageGroups(entry, customerServicesArray);
                    return RedirectToAction("Index", new { thisid = entry.id });
                }
                catch (Exception e)
                {
                    var resultsToSearchFrom = rpstryService.GetAll();
                    ViewBag.LoggedUser = loggedUser;
                    var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
                    if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
                    {
                        resultsToSearchFrom = loggedUser.customerId != null ? db.CustomerServices.Where(d => d.customerId == loggedUser.customerId).Select(d => d.Service).AsQueryable() : new List<Service>().AsQueryable();
                    }
                    else
                    {
                        ViewBag.Customers = new SelectList(rpstryCustomer.GetAll(), "id", "fullName");
                    }
                    ViewBag.Services = new SelectList(resultsToSearchFrom, "id", "title");
                    var resultsToGroupFrom = db.DeviceGroups.AsQueryable();
                    ViewBag.DeviceGroups = new SelectList(resultsToGroupFrom, "id", "title");
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
                ViewBag.LoggedUser = loggedUser;
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
            var dc = new DataClassesDataContext();
            var resultsToSearchFrom = (from a in dc.Devices
                                       select a).AsEnumerable();
            ViewBag.LoggedUser = loggedUser;
            var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
            if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
            {
                 if (loggedUser.customerId != null){
                  resultsToSearchFrom = (from a in dc.Devices
                                       join em in dc.DeviceGroups on a.deviceGroupId equals em.id
                                       join es in dc.Customers on em.customerId equals es.id
                                         where es.id == loggedUser.customerId 
                                         select a).AsEnumerable();
                }else{
                    resultsToSearchFrom = new List<Device>().AsEnumerable();
                }
            }
            if (hasPermissions)
            {
                return Json(resultsToSearchFrom.Select(x => new
                {
                    id = x.id,
                    deviceId = x.deviceId,
                    DeviceListingOfGroup = x.DeviceListingOfGroup,
                    Longitude = x.Longitude,
                    Latitude = x.Latitude,
                    Image = x.Image,
                    isActive = x.isActive,
                    isMonitored = x.isMonitored
                }).ToDataSourceResult(request));
            }
            else
            {
                return View("Error", "You do not have Orders to access this section.");
            }

        }


        public ActionResult Excel_Export_Save(string base64, string fileName)
        {
            var dc = new DataClassesDataContext();
            var resultsToSearchFrom = rpstry.GetAll();
            ViewBag.LoggedUser = loggedUser;
            var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
            if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
            {
                resultsToSearchFrom = loggedUser.customerId != null ? resultsToSearchFrom.AsQueryable() : new List<Device>().AsQueryable();
                //resultsToSearchFrom = loggedUser.customerId != null ? resultsToSearchFrom.Where(d => d.customerId == loggedUser.customerId).AsQueryable() : new List<Device>().AsQueryable();
            }
            Response.AddHeader("content-disposition", "attachment;filename=Report1.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            return View("View1", resultsToSearchFrom);
        }

        public JsonResult GetDeviceIds()
        {
            var resultsToSearchFrom = rpstry.GetAll();
            var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
            if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
            {
                resultsToSearchFrom = loggedUser.customerId != null ? resultsToSearchFrom.AsQueryable() : new List<Device>().AsQueryable();
                //resultsToSearchFrom = loggedUser.customerId != null ? resultsToSearchFrom.Where(d => d.customerId == loggedUser.customerId).AsQueryable() : new List<Device>().AsQueryable();
            }
            return Json(resultsToSearchFrom.Select(x => new
            {
                TEXT = x.deviceId,
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
                var entry = rpstry.GetBiId(id);
                var resultsToSearchFrom = rpstryService.GetAll();
                ViewBag.LoggedUser = loggedUser;
                var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
                if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
                {
                    resultsToSearchFrom = loggedUser.customerId != null ? db.CustomerServices.Where(d => d.customerId == loggedUser.customerId).Select(d => d.Service).AsQueryable() : new List<Service>().AsQueryable();
                }
                else
                {
                    ViewBag.Customers = new SelectList(rpstryCustomer.GetAll(), "id", "fullName", loggedUser.customerId);
                }
                //ViewBag.Services = new SelectList(resultsToSearchFrom, "id", "title", entry.serviceId);
                var resultsToGroupFrom = db.DeviceGroups.AsQueryable();
                ViewBag.DeviceGroups = new SelectList(resultsToGroupFrom, "id", "title");
                ViewBag.Services = new SelectList(resultsToSearchFrom, "id", "title");
                ViewBag.Permissions = permissions;
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
                        TryUpdateModel(entry);
                        rpstry.Save();
                        var customerServicesArray = !string.IsNullOrEmpty(Request["Device_Groups"]) ? Request["Device_Groups"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
                        //rpstry.ManageGroups(entry, customerServicesArray);
                        return RedirectToAction("Index", new { thisid = entry.id });
                    }
                    else
                    {

                        var resultsToSearchFrom = rpstryService.GetAll();
                        ViewBag.LoggedUser = loggedUser;
                        var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
                        if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
                        {
                            resultsToSearchFrom = loggedUser.customerId != null ? db.CustomerServices.Where(d => d.customerId == loggedUser.customerId).Select(d => d.Service).AsQueryable() : new List<Service>().AsQueryable();
                        }
                        else
                        {
                            ViewBag.Customers = new SelectList(rpstryCustomer.GetAll(), "id", "fullName");
                        }
                        //ViewBag.Services = new SelectList(resultsToSearchFrom, "id", "title", entry.serviceId);
                        ViewBag.Services = new SelectList(resultsToSearchFrom, "id", "title");
                        var resultsToGroupFrom = db.DeviceGroups.AsQueryable();
                        ViewBag.DeviceGroups = new SelectList(resultsToGroupFrom, "id", "title");
                        ModelState.AddModelError("", "");
                        return View("details", entry);
                    }
                }
                catch (Exception e)
                {

                    var resultsToSearchFrom = rpstryService.GetAll();
                    ViewBag.LoggedUser = loggedUser;
                    var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
                    if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
                    {
                        resultsToSearchFrom = loggedUser.customerId != null ? db.CustomerServices.Where(d => d.customerId == loggedUser.customerId).Select(d => d.Service).AsQueryable() : new List<Service>().AsQueryable();
                    }
                    else
                    {
                        ViewBag.Customers = new SelectList(rpstryCustomer.GetAll(), "id", "fullName");
                    }
                    //ViewBag.Services = new SelectList(resultsToSearchFrom, "id", "title", entry.serviceId);
                    ViewBag.Services = new SelectList(resultsToSearchFrom, "id", "title");
                    var resultsToGroupFrom = db.DeviceGroups.AsQueryable();
                    ViewBag.DeviceGroups = new SelectList(resultsToGroupFrom, "id", "title");
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
        public ActionResult Delete(int id)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canDelete);
            var db = new DataClassesDataContext();
            if (hasPermissions)
            {
                try
                {
                    var entry = rpstry.GetBiId(id);
                    rpstry.Delete(entry);
                    rpstry.Save();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    return View("Error", "Problem On Delete Entry.");
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