using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Spotless.Helpers;
using Spotless.Models;
using System.Threading;

namespace Spotless.Controllers
{
    public class HomeController : BaseController
    {
        private DataClassesDataContext db = new DataClassesDataContext();
        private DeviceRepository repo = new DeviceRepository();
        private List<Device> currentResult = new List<Device>();
        private ControlDeviceController devC = new ControlDeviceController();
        public ActionResult Index()
        {
            var results = repo.GetAll();
            ViewBag.isAdmin = true;
            if (loggedUser != null)
            {
                var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
                if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
                {
                    ViewBag.isAdmin = false;
                    results = loggedUser.customerId != null ? results.Where(d => d.DeviceGroup.customerId == loggedUser.customerId).AsQueryable() : new List<Device>().AsQueryable();
                }
            }
            return View(results);
        }
        [HttpPost]
        public string SetDevice()
        {
            try
            {
                var mode = Request["mode"];
                if (string.IsNullOrEmpty(mode))
                    return "invalid mode";
                var id = Request["id"];
                if (string.IsNullOrEmpty(id))
                    return "invalid id";
                var deviceid = Request["deviceid"];
                if (string.IsNullOrEmpty(deviceid))
                    return "invalid deviceid";
                var changed = devC.ChangeDeviceStatus(Convert.ToInt32(id), Convert.ToInt32(mode)).Result;
                if (changed)
                {
                    var entity = repo.GetAll().Where(d => d.id == Convert.ToInt32(id) && d.deviceId == Int32.Parse(deviceid)).FirstOrDefault();
                    entity.isActive = mode == "0" ? false : true;
                    TryUpdateModel(entity);
                    repo.Save();
                }
                else
                {
                    return "invalid";
                }
                
                
                return "success";
            }
            catch(Exception e)
            {
                return "invalid update due to : " + e.Message;
            }
        }

        public bool TurnOnOffAllDevices(bool isOn)
        {
            var Activated = Request["Activated"];
            var Monitored = Request["Monitored"];
            var customers = "";
            var count = 0;
            if (loggedUser != null)
            {
                var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
                if (currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) || currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
                {
                    customers = Request["Customers"];
                }
            }
            var deviceGroups = Request["DeviceGroups"];
            var deviceIds = Request["DeviceIds"];
            var results = fetchbyParameters(Activated, Monitored, deviceGroups, deviceIds, customers);
            List<Device> oldData = results.ToList();
            foreach (var result in results)
            {
                var changed = devC.ChangeDeviceStatus(Convert.ToInt32(result.id), Convert.ToInt32(isOn)).Result;
                if (changed)
                {
                    var entity = repo.GetBiId(result.id);
                    entity.isActive = isOn;
                    TryUpdateModel(entity);
                    count++;
                }
                Thread.Sleep(1200);
            }
            repo.Save();
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            // return Json(oldData.Select(d => new MultiSelectResult { TEXT = d.DeviceName, VALUE = Convert.ToString(d.deviceId) }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult MonitorOnOffAllDevices(bool isOn)
        {
            var Activated = Request["Activated"];
            var Monitored = Request["Monitored"];
            var customers = "";
            if (loggedUser != null)
            {
                var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
                if (currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) || currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
                {
                    customers = Request["Customers"];
                }
            }
            var deviceGroups = Request["DeviceGroups"];
            var deviceIds = Request["DeviceIds"];
            var results = fetchbyParameters(Activated, Monitored, deviceGroups, deviceIds, customers);
            List<Device> oldData = results.ToList();
            foreach (var result in results)
            {

                var entity = repo.GetBiId(result.id);
                entity.isMonitored = isOn;
                TryUpdateModel(entity);
            }
            repo.Save();
            return Json(oldData.Select(d => new MultiSelectResult { TEXT = d.DeviceName, VALUE = Convert.ToString(d.deviceId) }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string SetDeviceMonitor()
        {
            try
            {
                var mode = Request["mode"];
                if (string.IsNullOrEmpty(mode))
                    return "invalid mode";
                var id = Request["id"];
                if (string.IsNullOrEmpty(id))
                    return "invalid id";
                var deviceid = Request["deviceid"];
                if (string.IsNullOrEmpty(deviceid))
                    return "invalid deviceid";
                var entity = repo.GetAll().Where(d => d.id == Convert.ToInt32(id) && d.deviceId == Int32.Parse(deviceid)).FirstOrDefault();
                entity.isMonitored = mode == "0" ? false : true;
                TryUpdateModel(entity);
                repo.Save();
                return "success";
            }
            catch (Exception e)
            {
                return "invalid update due to : " + e.Message;
            }
        }

        public ActionResult FetchResultsByGroupAndDeviceIds()
        {
            var Activated = Request["Activated"];             
            var Monitored = Request["Monitored"];
            var customers = "";
            if (loggedUser != null)
            {
                var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
                if (currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) || currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
                {
                    customers = Request["Customers"];
                }
            }
            var deviceGroups = Request["DeviceGroups"];
            var deviceIds = Request["DeviceIds"];
            var results = fetchbyParameters(Activated, Monitored, deviceGroups, deviceIds, customers);
            return PartialView("~/Views/Home/DevicesLocation.cshtml", results.AsEnumerable());
        }
        public IEnumerable<Device> fetchbyParameters(string isActive,string isMonitored,string DeviceGroups,string DeviceIds,string customers = null){
            var results = repo.GetAll().AsEnumerable();
            if (!string.IsNullOrEmpty(isActive) && Convert.ToBoolean(isActive) != false)
            {
                var Activated = Convert.ToBoolean(isActive);
                results = results.Where(d => d.isActive == Activated).AsEnumerable();
            }
            //if (!string.IsNullOrEmpty(isMonitored))
            //{
            //        var Monitored = Convert.ToBoolean(isMonitored);
            //        results = results.Where(d => d.isMonitored == Monitored);
            //}
            
            if (loggedUser != null)
            {
                var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
                if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
                {
                    results = loggedUser.customerId != null ? results.Where(d => d.DeviceGroup.customerId == loggedUser.customerId).AsEnumerable() : new List<Device>().AsEnumerable();
                }
                else
                {
                    results = loggedUser.customerId != null ? results.AsEnumerable() : new List<Device>().AsEnumerable();
                    if (!string.IsNullOrEmpty(customers))
                    results = results.Where(d => customers.Contains(d.DeviceGroup.customerId.ToString())).AsEnumerable();
                }
            }
            var deviceGroups = DeviceGroups;
            var deviceIds = DeviceIds;
            if (!string.IsNullOrEmpty(deviceIds))
            {
                results = results.Where(d => deviceIds.Contains(d.id.ToString())).AsEnumerable();
            }
            if (!string.IsNullOrEmpty(deviceGroups))
            {
                results = results.Where(d => deviceGroups.Contains(d.deviceGroupId.ToString())).AsEnumerable();
            }
            return results;
        }
    }
}
