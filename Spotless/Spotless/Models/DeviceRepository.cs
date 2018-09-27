using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Spotless.Models
{

    #region Models
    //[MetadataType(typeof(DeviceValidation))]
    public partial class Device
    {


        public string DeviceListingOfGroup
        {
            get
            {
                try
                {
                    return this.DeviceGroup.title;
                }
                catch
                {
                    return "";
                }
            }
        }
    }

    public partial class DeviceValidation
    {
        [Required]
        [Display(Name = "DeviceName")]
        public string DeviceName { get; set; }
        
        public bool? isActive { get; set; }
 
        public bool? isConnected { get; set; }
        [Required]
        public bool isMonitored { get; set; }

        [Required]
        public string deviceId { get; set; }
      
        [Display(Name = "Longitude")]
        public string Longitude { get; set; }


        [Display(Name = "Latitude")]
        public string Latitude { get; set; }

        //[Required]
        [Display(Name = "Image")]
        public string Image { get; set; }
        
        [Required]
        public int deviceGroupId { get; set; }


    }
    #endregion


    #region Repository
    public class DeviceRepository
    {

        private DataClassesDataContext dc = new DataClassesDataContext();
        private string sectionName = "Device";

        #region Create
        public void Add(Device entry)
        {
            dc.Devices.InsertOnSubmit(entry);
        }
        #endregion

        #region Read
        public Device GetBiId(int? id)
        {
            return dc.Devices.FirstOrDefault(d => d.id == id);
        }
        public Device GetBiIdIsPublished(int? id)
        {
            return dc.Devices.FirstOrDefault(d => d.id == id);
        }

        public IQueryable<Device> GetAll()
        {
                return dc.Devices;
        }

        #endregion

        #region Update
        public void Save()
        {
            dc.SubmitChanges();
        }

        #endregion

        #region Delete
        public void DeleteAllBySection(int id)
        {
            dc.Permissions.DeleteAllOnSubmit(dc.Permissions.Where(d => d.sectionId == id));
        }

        public void Delete(Device entry)
        {
            dc.Devices.DeleteOnSubmit(entry);
        }
        #endregion

    }
#endregion

}