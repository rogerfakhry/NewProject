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
    public class CustomerController : BaseController
    {

        private CustomerRepository rpstry = new CustomerRepository();
        private CustomerTypeRepository rpstryType = new CustomerTypeRepository();
        private string sectionName = "Customer";


        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);

            if (permissions.CanCreate)
            {
                ViewBag.Permissions = permissions;
                ViewBag.Types = new SelectList(rpstryType.GetAll(), "id", "name");
                return View("Details");
            }
            else
            {
                return View("Error", "You do not have Orders to access this section.");
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(Customer entry)
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
                    var customerServicesArray = !string.IsNullOrEmpty(Request["CustomerServices"]) ? Request["CustomerServices"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
                    rpstry.ManageServices(entry, customerServicesArray);
                    return RedirectToAction("Index", new { thisid = entry.id });
                }
                catch (Exception e)
                {
                    ViewBag.Types = new SelectList(rpstryType.GetAll(), "id", "name");
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
            var db = new DataClassesDataContext();
            var resultsToSearchFrom = rpstry.GetAll();
            var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
            if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
            {
                resultsToSearchFrom = loggedUser.customerId != null ? resultsToSearchFrom.Where(d => d.id == loggedUser.customerId).AsQueryable() : new List<Customer>().AsQueryable();
            }
            if (hasPermissions)
            {
                return Json(resultsToSearchFrom.Select(x => new
                {
                    id = x.id,
                    fullName = x.fullName,
                    Type = x.CustomerType.name,
                    Customer_Services = x.CustomerServices.Any() ? string.Join(",", x.CustomerServices.Select(d => d.Service.title).ToArray()) : "",
                    isActive = x.isActive,
                    dateModified = x.dateModified
                }).ToDataSourceResult(request));
            }
            else
            {
                return View("Error", "You do not have Orders to access this section.");
            }

        }

        public JsonResult GetCustomers()
        {
            var resultsToSearchFrom = rpstry.GetAll();
            return Json(resultsToSearchFrom.Select(x => new
            {
                TEXT = x.fullName,
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
                ViewBag.Permissions = permissions;
                var entry = rpstry.GetBiId(id);
                ViewBag.Types = new SelectList(rpstryType.GetAll(), "id", "name", entry.typeId);
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
                        TryUpdateModel(entry);
                        entry.dateModified = DateTime.Now;
                        rpstry.Save();
                        var customerServicesArray = !string.IsNullOrEmpty(Request["CustomerServices"]) ? Request["CustomerServices"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
                        rpstry.ManageServices(entry, customerServicesArray);
                        return RedirectToAction("Index", new { thisid = entry.id });
                    }
                    else
                    {
                        ViewBag.Types = new SelectList(rpstryType.GetAll(), "id", "name", entry.typeId);
                        ModelState.AddModelError("", "");
                        return View("details", entry);
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Types = new SelectList(rpstryType.GetAll(), "id", "name", entry.typeId);
                    ModelState.AddModelError("", "");
                    return View("details", entry);
                }
            }
            else
            {
                return View("Error", "You do not have Orders to access this section.");
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