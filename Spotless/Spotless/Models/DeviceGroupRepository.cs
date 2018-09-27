using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Models
{

    #region Models
    [MetadataType(typeof(DeviceGroupValidation))]
    public partial class DeviceGroup
    {
        public string ServiceOfDeviceGroup
        {
            get
            {
                try
                {
                    return this.Service.title;
                }
                catch
                {
                    return "";
                }
            }
        }
        public string CustomerOfDeviceGroup
        {
            get
            {
                try
                {
                    return this.Customer.fullName;
                }
                catch
                {
                    return "";
                }
            }
        }
    }

    public partial class DeviceGroupValidation
    {
        [Required]
        [Display(Name = "Title")]
        public string title { get; set; }
        [Required]
        [Display(Name = "ComPort")]
        public string comPort { get; set; }
        [Required]
        public string serviceId { get; set; }
        [Required]
        public string customerId { get; set; }
        [Required]
        [Display(Name = "IP")]
        public string StaticIP { get; set; }
        [Display(Name = "MAC Address")]
        public string Macaddress { get; set; }
        [Display(Name = "Longitude")]
        public string Longitude { get; set; }
        [Display(Name = "Latitude")]
        public string Latitude { get; set; }


    }


    #endregion


    #region Repository
    public class DeviceGroupRepository
    {
        private DataClassesDataContext dc = new DataClassesDataContext();
        private string sectionName = "DeviceGroup";

        #region Create
        public void Add(DeviceGroup entry)
        {
            dc.DeviceGroups.InsertOnSubmit(entry);
        }
        #endregion

        #region Read
        public DeviceGroup GetBiId(int? id)
        {
            return dc.DeviceGroups.FirstOrDefault(d => d.id == id);
        }

        public IQueryable<DeviceGroup> GetAll()
        {
            return dc.DeviceGroups.OrderByDescending(d => d.id);

        }
        #endregion

        #region Update

        public void Save()
        {
            dc.SubmitChanges();
        }
        #endregion

        #region Delete
        public void Delete(DeviceGroup entry)
        {
            dc.DeviceGroups.DeleteOnSubmit(entry);
        }
        #endregion

    }
    #endregion

}//end namespace