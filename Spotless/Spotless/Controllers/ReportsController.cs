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
using System.Web.Script.Serialization;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using System.ComponentModel;
namespace Spotless.Controllers
{
    public class ReportsController : BaseController
    {
        //
        // GET: /Reports/
        private string sectionName = "Reports";
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
            var resultsToSearchFrom = dc.DeviceHistories;
            ViewBag.LoggedUser = loggedUser;
            //var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
            //if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
            //{
            //    resultsToSearchFrom = loggedUser.customerId != null ? resultsToSearchFrom.Where(d => d.customerId == loggedUser.customerId).AsQueryable() : new List<Device>().AsQueryable();
            //}
            if (hasPermissions)
            {

                return Json(resultsToSearchFrom.Select(x => new
                {
                    id = x.id,
                    DeviceListingOfGroup = x.Device.DeviceGroup.title,
                    Device_id = x.Device.DeviceName,
                    PVVoltage = x.PVVoltage,
                    PVCurrent = x.PVCurrent,
                    BatteryTemp = x.BatteryTemp,
                    BatterySOC = x.BatterySOC,
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
            var data = dc.DeviceHistories.AsEnumerable();
            Response.AddHeader("content-disposition", "attachment;filename=Report1.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            return View("View1",data);
        }

    }
}
