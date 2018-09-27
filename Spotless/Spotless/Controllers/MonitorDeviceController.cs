using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spotless.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Web.Security;
using System.Configuration;

namespace Spotless.Controllers
{
    public class MonitorDeviceController : BaseController
    {
        
        private DeviceRepository rpstry = new DeviceRepository();
        private DeviceGroupRepository rpstryDeviceGroup = new DeviceGroupRepository();
        private ServiceRepository rpstryService = new ServiceRepository();
        private CustomerRepository rpstryCustomer = new CustomerRepository();
        private string sectionName = "Device";
        private DataClassesDataContext db = new DataClassesDataContext();
        // GET: MonitorDevice
        [Authorize]
        [HttpGet]
        public ActionResult Index()
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
                var resultsToGroupFrom = db.DeviceGroups.AsQueryable();
                ViewBag.DeviceGroups = new SelectList(resultsToGroupFrom, "id", "title");
                ViewBag.Services = new SelectList(resultsToSearchFrom, "id", "title");

                ViewBag.Permissions = permissions;
                return View("index");
            }
            else
            {
                return View("Error", "You do not have Orders to access this section.");
            }
        }

        // GET: MonitorDevice/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MonitorDevice/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MonitorDevice/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MonitorDevice/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MonitorDevice/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MonitorDevice/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MonitorDevice/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
