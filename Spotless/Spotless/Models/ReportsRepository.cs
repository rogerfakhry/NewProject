using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Models
{
    #region Models

    public partial class DeviceHistory
    {
        public string Device_id
        {
            get
            {
                return this.Device != null ? this.Device.deviceId.ToString() : "";
            }
        }
        public string DeviceListingOfGroup
        {
            get
            {
                try
                {
                    return this.Device.DeviceGroup.title;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }
    }
    #endregion

    #region Repository
    public class ReportsRepository
    {


    }
    #endregion
}