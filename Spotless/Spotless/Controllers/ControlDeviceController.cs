using Spotless.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.Devices;
using System.DirectoryServices;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;
using System.Dynamic;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System.IO;
using System.Text;


namespace Spotless.Controllers
{

    internal sealed class Module1
    {
        public static float HVD;

        public static int CmdLenth;

        public static int ID;

        public static int SetID;

        public static byte HiByte;

        public static byte LoByte;

        public static byte buscmd;

        public static byte year;

        public static byte day;

        public static byte month;

        public static byte hour;

        public static byte minute;

        public static byte second;

        public static byte flag_checking;

        public static float[] mytag = new float[124];

        public static byte[] flashback = new byte[28673];
    }

    // The class derived from DynamicObject.
    public class DynamicDictionary : DynamicObject
    {
        // The inner dictionary.
        public Dictionary<object, object> dictionary
            = new Dictionary<object, object>();

        // This property returns the number of elements
        // in the inner dictionary.
        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        // If you try to get a value of a property 
        // not defined in the class, this method is called.
        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name.ToLower();

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return dictionary.TryGetValue(name, out result);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            dictionary[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
    }


  

    public class ControlDeviceController : BaseController
    {
        private DeviceRepository rpstry = new DeviceRepository();
        private DeviceGroupRepository rpstryDeviceGroup = new DeviceGroupRepository();
        private ServiceRepository rpstryService = new ServiceRepository();
        private CustomerRepository rpstryCustomer = new CustomerRepository();
        private string sectionName = "Device";
        private DataClassesDataContext db = new DataClassesDataContext();

        private int currentLightStatus;
        private string Textfault;

        private SerialPort SerialPort1 = new SerialPort("COM1");


        dynamic MyProject;

        dynamic parameters = new DynamicDictionary();
        dynamic deviceParameters = new DynamicDictionary();
        string Alert ;
        private DirectoryEntry _DirectoryEntry1;

        private string buffer;

        private byte[] recBuf;

        private int i;

        private float BV_Real;

        private float PV_Real;

        private float PP_Real;

        private float PP_RealH;

        private float PP_RealL;

        private float PI_Real;

        private float BI_Real;

        private float LI_Real;

        private float LV_Real;

        private float LP_Real;

        private float LP_RealH;

        private float LP_RealL;

        private float LV1_Real;

        private float LI1_Real;

        private float LP1_Real;

        private float LP1_RealH;

        private float LP1_RealL;

        private float LV2_Real;

        private float LI2_Real;

        private float LP2_Real;

        private float LP2_RealH;

        private float LP2_RealL;

        private float LV3_Real;

        private float LI3_Real;

        private float LP3_Real;

        private float LP3_RealH;

        private float LP3_RealL;

        private float DCV_Real;

        private float DCI_Real;

        private float DCP_Real;

        private float DCP_RealH;

        private float DCP_RealL;

        private float Day_ChargeKWH;

        private float Month_ChargeKWH;

        private float Year_ChargeKWH;

        private float Total_ChargeKWH;

        private float Day_DisChargeKWH;

        private float Month_DisChargeKWH;

        private float Year_DisChargeKWH;

        private float Total_DisChargeKWH;

        private int mycount;

        private int fault;

        private int TI_Real;

        private int Tout_Real;

        private int SOC_Real;

        private int tickcount;

        private int Flag_recok;

        private float TempH;

        private float TempL;

        private float PV_DayMax;

        private float PV_DayMin;

        private float BA_DayMax;

        private float BA_DayMin;

        private byte State_Work;

        private byte State_Fault;

        private byte State_Load;

        private byte cycle;

        private byte BatteryStateH;

        private byte BatteryStateL;

        private byte PVStateH;

        private byte PVStateL;

        private byte LoadStateH;

        private byte LoadStateL;

        private byte[] recbuf2;

        private byte Flag_recok2;

        private byte tickcount2;

        private byte flag_Pararead1;

        private byte flag_Parawrite1;

        

        private int totaltickcount;
        // GET: ControlDevice
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);

            if (permissions.CanCreate)
            {
                var resultsToSearchFrom = rpstryService.GetAll();
                DeviceRepository repo = new DeviceRepository();
                List<Device> currentDevices = new List<Device>();
                var DeviceResults = repo.GetAll();
                ViewBag.LoggedUser = loggedUser;
                var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
                if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
                {
                    DeviceResults = loggedUser.customerId != null ? DeviceResults.Where(d => d.DeviceGroup.customerId == loggedUser.customerId).AsQueryable() : new List<Device>().AsQueryable();
                    resultsToSearchFrom = loggedUser.customerId != null ? db.CustomerServices.Where(d => d.customerId == loggedUser.customerId).Select(d => d.Service).AsQueryable() : new List<Service>().AsQueryable();
                }
                else
                {
                    ViewBag.Customers = new SelectList(rpstryCustomer.GetAll(), "id", "fullName");
                }
                var resultsToGroupFrom = db.DeviceGroups.AsQueryable();
                ViewBag.DeviceGroups = new SelectList(resultsToGroupFrom, "id", "title");
                ViewBag.Services = new SelectList(resultsToSearchFrom, "id", "title");
                ViewBag.Devices = DeviceResults;
                ViewBag.Permissions = permissions;

                return View("index");
            }
            else
            {
                return View("Error", "You do not have Orders to access this section.");
            }
        }

        public async Task<bool> GetDeviceStatus(int deviceId, string ComPort)
        {
            int x = 0;
            if (this.SerialPort1.IsOpen == false)
            {
                var m = await Check_Device(deviceId, ComPort);
                return m;
            }
            else
            {
                x++;
                if (x > 1) this.PortClose();
                Thread.Sleep(1000);
                return await this.GetDeviceStatus(deviceId, ComPort);
            }


        }

        public async Task<bool> checkDevicesAvialability()
        {
            var dc = new DataClassesDataContext();
            var resultsToSearchFrom = (from a in dc.Devices
                                       select a).AsEnumerable();
            var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
            if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
            {
                if (loggedUser.customerId != null)
                {
                    resultsToSearchFrom = (from a in dc.Devices
                                           join em in dc.DeviceGroups on a.deviceGroupId equals em.id
                                           where em.customerId == loggedUser.customerId
                                           select a).AsEnumerable();
                    //resultsToSearchFrom = new List<Device>().Where(x=>x.DeviceGroup.customerId == loggedUser.customerId).AsEnumerable();

                }
                else
                {
                    resultsToSearchFrom = new List<Device>().AsEnumerable();
                }
            }
            foreach (var item in resultsToSearchFrom)
            {
                try
                {
                    var m = await this.GetDeviceStatus(item.deviceId, "COM" + item.DeviceGroup.comPort);
                    this.PortClose();
                    var entry = rpstry.GetBiId(item.id);
                    entry.isConnected = m;
                    entry.isActive = this.LI_Real > 0 ? true : false;
                    TryUpdateModel(entry);
                    rpstry.Save();
                }
                catch (Exception e)
                {
                    this.PortClose();
                    if (e.Message == "")
                    {
                        //this.PortClose();
                    }
                    else if (e.Message.Contains("does not exist."))
                    {
                        var msg = "Port does not exist.";
                    }
                    var entry = rpstry.GetBiId(item.id);
                    entry.isConnected = false;
                    TryUpdateModel(entry);
                    rpstry.Save();
                }
                var s = 4;
                Thread.Sleep(1100);
            }
            return true;


        }

        public async Task<bool> ChangeDeviceStatus(int deviceId, string ComPort, int status)
        {
            int x = 0;
            if (this.SerialPort1.IsOpen == false)
            {
                this.SerialPort1.PortName = ComPort;
                this.SerialPort1.ReadBufferSize = 1000;
                this.SerialPort1.ReceivedBytesThreshold = 64;
                this.SerialPort1.WriteBufferSize = 100;

                this.SerialPort1.Open();
                Module1.CmdLenth = 8;
                Module1.ID = deviceId;
                Module1.flag_checking = 1;
                this.currentLightStatus = status == 1 ? 5: 4 ;
                await this.Sendcmd(36984, 16, 1);

                return true;
            }
            else
            {
                x++;
                if (x > 1) this.PortClose();
                Thread.Sleep(1000);
                
                return await this.ChangeDeviceStatus(deviceId, ComPort, status);
            }


        }

        public async Task<bool> ChangeDeviceStatus(int id, int status)
        {
            var item = rpstry.GetAll().Where(d => d.id == Convert.ToInt32(id)).FirstOrDefault();


            try
            {
                var m = await this.ChangeDeviceStatus(item.deviceId, "COM" + item.DeviceGroup.comPort, status);
                this.PortClose();
            }
            catch (Exception e)
            {
                return false;
            }

            

            return true;


        }

        public async Task<bool> GetDeviceParameters(int id)
        {
            var item = rpstry.GetAll().Where(d => d.id == Convert.ToInt32(id)).FirstOrDefault();


            try
            {
                this.SerialPort1.PortName = "COM" + item.DeviceGroup.comPort;
                this.SerialPort1.ReadBufferSize = 1000;
                this.SerialPort1.ReceivedBytesThreshold = 64;
                this.SerialPort1.WriteBufferSize = 100;

                this.SerialPort1.Open();
                Module1.CmdLenth = 8;
                Module1.ID = item.deviceId;
                Module1.flag_checking = 1;
                Module1.buscmd = 3;
                await this.Sendcmd(36864, 3, 61);
                this.flag_Pararead1 = 1;
                this.recbuf2 = new byte[261];
                this.Read_parameters();
                var mn = this.deviceParameters;
                this.PortClose();
            }
            catch (Exception e)
            {
                this.PortClose();
                return false;
            }

            //Thread.Sleep(1100);

            return true;

        }

        public async Task<JsonResult> GetDeviceParametersList(int id)
        {
            await this.GetDeviceParameters(id);
            Thread.Sleep(100);
            if (this.Alert != "" && this.deviceParameters.Count != 0) { this.Alert = "Read OK , OK!"; } else
            { this.Alert = "Unreachable , Error!"; }
            return Json(new { data = this.deviceParameters.dictionary,message = this.Alert }, JsonRequestBehavior.AllowGet);
            
        }

        
        [HttpPost]
        public async Task<JsonResult> SaveDeviceParametersList(int id, Dictionary<object, object> status)
        {
            var item = rpstry.GetAll().Where(d => d.id == Convert.ToInt32(id)).FirstOrDefault();


            try
            {
                this.SerialPort1.PortName = "COM" + item.DeviceGroup.comPort;
                this.SerialPort1.ReadBufferSize = 1000;
                this.SerialPort1.ReceivedBytesThreshold = 64;
                this.SerialPort1.WriteBufferSize = 100;

                this.SerialPort1.Open();
                Module1.CmdLenth = 8;
                Module1.ID = item.deviceId;
                Module1.flag_checking = 1;
                Module1.buscmd = 3;
                var s = new DynamicDictionary();
                s.dictionary = status;
                this.deviceParameters = s;
                await this.SaveParameters();
                Thread.Sleep(100);
                this.PortClose();
                //return true;
            }
            catch (Exception e)
            {
                if(this.Alert != "") { this.Alert = "Unreachable , Error!"; }
                this.PortClose();
                return Json(new { data = this.Alert , message = this.Alert }, JsonRequestBehavior.AllowGet);
                //throw;
            }
            if (this.Alert != "") { this.Alert = "Write Ok , Ok!"; }
            return Json(new { data = this.Alert , message = this.Alert }, JsonRequestBehavior.AllowGet);


        }

        public async Task<bool> SaveDeviceParametersToGroup(int id, Dictionary<object, object> status)
        {
            var result = rpstry.GetAll().Where(d => d.DeviceGroup.id == Convert.ToInt32(id) && d.isConnected == true);

            foreach (var item in result)
            {
                try
                {
                    this.SerialPort1.PortName = "COM" + item.DeviceGroup.comPort;
                    this.SerialPort1.ReadBufferSize = 1000;
                    this.SerialPort1.ReceivedBytesThreshold = 64;
                    this.SerialPort1.WriteBufferSize = 100;

                    this.SerialPort1.Open();
                    Module1.CmdLenth = 8;
                    Module1.ID = item.deviceId;
                    Module1.flag_checking = 1;
                    Module1.buscmd = 3;
                    var s = new DynamicDictionary();
                    s.dictionary = status;
                    this.deviceParameters = s;
                    await this.SaveParameters();
                    Thread.Sleep(100);
                    this.PortClose();
                    
                }
                catch (Exception e)
                {
                    if (this.Alert != "") { this.Alert = "Unreachable , Error!"; }
                    this.PortClose();
 
                }
                Thread.Sleep(1000);
            }
            return true;


        }
        [HttpPost]
        public async Task<JsonResult> SaveDeviceParametersListToGroup(int id, Dictionary<object, object> status)
        {
            await SaveDeviceParametersToGroup(id, status);
            if (this.Alert != "") { this.Alert = "Write Ok , Ok!"; }
            return Json(new { data = this.Alert, message = this.Alert }, JsonRequestBehavior.AllowGet);


        }
        //public async Task<ActionResult> GetDevice()
        //{
        //    //Module1.buscmd = 16;
        //    //this.Sendcmd(36925, Module1.buscmd, 62);
        //    //this.PortClose();
        //    try
        //    {
        //        //var m = await this.GetDeviceStatus(2, "COM7");
        //        this.SerialPort1.PortName = "COM7";
        //        this.SerialPort1.ReadBufferSize = 1000;
        //        this.SerialPort1.ReceivedBytesThreshold = 64;
        //        this.SerialPort1.WriteBufferSize = 100;

        //        this.SerialPort1.Open();
        //        Module1.CmdLenth = 8;
        //        Module1.ID = 2;
        //        Module1.flag_checking = 1;
        //        this.currentLightStatus = 4;
        //        Module1.buscmd = 3;
        //        await this.Sendcmd(36864, 3, 61);
        //        this.flag_Pararead1 = 1;
        //        this.recbuf2 = new byte[261];

        //        this.Read_parameters();

        //        var mn =this.deviceParameters;
        //        //this.buffer = "";
        //        ////this.recBuf = new byte[101];
        //        ////this.Read_Device();
        //        //this.recbuf2 = new byte[261];

        //        //Module1.buscmd = 3;
        //        //var m = this.Sendcmd(36864, 3, 61);
        //        //Module1.buscmd = 3;
        //        //this.Read_Battary();
        //        ////Module1.buscmd = 3;
        //        ////this.Read_Battary();
        //        //var t = this.ManualControl;
        //        //this.ManualControl = 2;
        //        //    Module1.buscmd = 16;
        //        //this.Sendcmd(36864, 16, 62);
        //        this.PortClose();
        //        var t = true;
        //    }
        //    catch (Exception e)
        //    {
        //        this.PortClose();
        //        if (e.Message == "")
        //        {
        //            this.PortClose();
        //        }
        //        else if (e.Message.Contains("does not exist."))
        //        {
        //            var msg = "Port does not exist.";
        //        }
        //        var s = e;
        //    }

        //    return this.MyProject;
        //}

        public void PortClose()
        {
            try
            {
                this.SerialPort1.DiscardInBuffer();
                this.SerialPort1.Close();
            }
            catch (Exception expr_18)
            {
                ProjectData.SetProjectError(expr_18);
                this.Alert = "The serial port is not opened or the serial port is abnormal！ , Error！";
                ProjectData.ClearProjectError();
            }
        }

        public object crc16(ref byte[] cmdstring, int j)
        {
            int Addressreg_crc = 65535;
            int arg_0A_0 = 0;
            int num = j;
            checked
            {
                for (int i = arg_0A_0; i <= num; i++)
                {
                    Addressreg_crc ^= (int)cmdstring[i];
                    j = 0;
                    do
                    {
                        int data = Addressreg_crc & 1;
                        if (data != 0)
                        {
                            Addressreg_crc = (int)Math.Round(Conversion.Int((double)Addressreg_crc / 2.0));
                            Addressreg_crc &= 32767;
                            Addressreg_crc ^= 40961;
                        }
                        else
                        {
                            Addressreg_crc = (int)Math.Round((double)Addressreg_crc / 2.0);
                            Addressreg_crc &= 32767;
                        }
                        j++;
                    }
                    while (j <= 7);
                }
                if (Addressreg_crc < 0)
                {
                    Addressreg_crc -= -65536;
                }
                Module1.HiByte = (byte)(Addressreg_crc & 255);
                Module1.LoByte = (byte)Math.Round((double)(Addressreg_crc & 65280) / 256.0);
                object crc16 = new object();
                return crc16;
            }
        }

        public async Task<object> Sendcmd(int myaddress, byte mycmd, byte mylength)
        {
            byte[] SendStr = new byte[261];
            checked
            {
                byte addressH = (byte)(myaddress / 256);
                byte addressL = (byte)(myaddress & 255);
                Module1.buscmd = mycmd;
                if (Module1.buscmd == 16)
                {

                    if (myaddress == 36984)
                    {
                        SendStr[0] = (byte)Module1.ID;
                        SendStr[1] = Module1.buscmd;
                        SendStr[2] = addressH;
                        SendStr[3] = addressL;
                        SendStr[4] = 0;
                        SendStr[5] = mylength;
                        SendStr[6] = Conversions.ToByte(mylength * 2);
                        SendStr[7] = 0;
                        SendStr[8] = Conversions.ToByte(this.currentLightStatus);
                        this.crc16(ref SendStr, 8);
                        SendStr[9] = Module1.HiByte;
                        SendStr[10] = Module1.LoByte;
                        Module1.CmdLenth = 8;
                        this.SerialPort1.ReceivedBytesThreshold = 8;
                        this.SerialPort1.Write(SendStr, 0, 11);
                    }
                    else if (myaddress == 8192)
                    {
                        SendStr[0] = (byte)Module1.ID;
                        SendStr[1] = Module1.buscmd;
                        SendStr[2] = addressH;
                        SendStr[3] = addressL;
                        SendStr[4] = 0;
                        SendStr[5] = mylength;
                        SendStr[6] = Conversions.ToByte(mylength * 2);
                        SendStr[7] = 0;
                        SendStr[8] = (byte)Module1.SetID;
                        this.crc16(ref SendStr, 8);
                        SendStr[9] = Module1.HiByte;
                        SendStr[10] = Module1.LoByte;
                        Module1.CmdLenth = 8;
                        this.SerialPort1.ReceivedBytesThreshold = 8;
                        this.SerialPort1.Write(SendStr, 0, 11);
                    }
                    else if (myaddress == 36882)
                    {
                        SendStr[0] = (byte)Module1.ID;
                        SendStr[1] = Module1.buscmd;
                        SendStr[2] = addressH;
                        SendStr[3] = addressL;
                        SendStr[4] = 0;
                        SendStr[5] = mylength;

                        SendStr[6] = Conversions.ToByte(mylength * 2);
                        SendStr[7] = Module1.year;
                        SendStr[8] = Module1.month;
                        SendStr[9] = Module1.day;
                        SendStr[10] = Module1.hour;
                        SendStr[11] = Module1.minute;
                        SendStr[12] = Module1.second;
                        this.crc16(ref SendStr, 12);
                        SendStr[13] = Module1.HiByte;
                        SendStr[14] = Module1.LoByte;
                        Module1.CmdLenth = 8;
                        this.SerialPort1.ReceivedBytesThreshold = 8;
                        this.SerialPort1.Write(SendStr, 0, 15);
                    }
                    else if (myaddress == 36864)
                    {
                        SendStr[0] = (byte)Module1.ID;
                        SendStr[1] = Module1.buscmd;
                        SendStr[2] = addressH;
                        SendStr[3] = addressL;
                        SendStr[4] = 0;
                        SendStr[5] = mylength;
                        SendStr[6] = Conversions.ToByte(mylength * 2);
                        float mytemp = Conversions.ToSingle(this.deviceParameters.textbox1);
                        int mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[11] = (byte)(mytemp2 / 256);
                        SendStr[12] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox2);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[13] = (byte)(mytemp2 / 256);
                        SendStr[14] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox3);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[15] = (byte)(mytemp2 / 256);
                        SendStr[16] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox4);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[17] = (byte)(mytemp2 / 256);
                        SendStr[18] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox5);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[19] = (byte)(mytemp2 / 256);
                        SendStr[20] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox6);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[21] = (byte)(mytemp2 / 256);
                        SendStr[22] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox7);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[23] = (byte)(mytemp2 / 256);
                        SendStr[24] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox8);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[25] = (byte)(mytemp2 / 256);
                        SendStr[26] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox9);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[27] = (byte)(mytemp2 / 256);
                        SendStr[28] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox10);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[29] = (byte)(mytemp2 / 256);
                        SendStr[30] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox11);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[31] = (byte)(mytemp2 / 256);
                        SendStr[32] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox12);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[33] = (byte)(mytemp2 / 256);
                        SendStr[34] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox13);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[35] = (byte)(mytemp2 / 256);
                        SendStr[36] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox14);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[37] = (byte)(mytemp2 / 256);
                        SendStr[38] = (byte)(mytemp2 & 255);
                        SendStr[39] = 255;
                        SendStr[40] = 216;
                        SendStr[41] = 255;
                        SendStr[42] = 216;
                        SendStr[43] = (byte)DateAndTime.Now.Minute;
                        SendStr[44] = (byte)DateAndTime.Now.Second;
                        SendStr[45] = (byte)DateAndTime.Now.Day;
                        SendStr[46] = (byte)DateAndTime.Now.Hour;
                        SendStr[47] = (byte)DateAndTime.Now.Month;
                        SendStr[48] = 17;
                        SendStr[49] = 0;
                        SendStr[50] = 30;
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox18);
                        SendStr[51] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[52] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        SendStr[53] = 255;
                        SendStr[54] = 216;
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox20);
                        SendStr[55] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[56] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox21);
                        SendStr[57] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[58] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox22);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[59] = (byte)(mytemp2 / 256);
                        SendStr[60] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox23);
                        SendStr[61] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[62] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox25);
                        mytemp2 = (int)Math.Round((double)(unchecked(mytemp * 100f)));
                        SendStr[63] = (byte)(mytemp2 / 256);
                        SendStr[64] = (byte)(mytemp2 & 255);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox24);
                        SendStr[65] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[66] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        SendStr[67] = 0;
                        if (this.deviceParameters.radiobutton1)
                        {
                            SendStr[68] = 0;
                        }
                        else if (this.deviceParameters.radiobutton2)
                        {
                            SendStr[68] = 1;
                        }
                        else if (this.deviceParameters.radiobutton3)
                        {
                            SendStr[68] = 2;
                        }
                        else if (this.deviceParameters.radiobutton4)
                        {
                            SendStr[68] = 3;
                        }
                        SendStr[69] = 0;
                        if (this.deviceParameters.radiobutton8)
                        {
                            SendStr[70] = 0;
                        }
                        else if (this.deviceParameters.radiobutton7)
                        {
                            SendStr[70] = 1;
                        }
                        else if (this.deviceParameters.radiobutton6)
                        {
                            SendStr[70] = 2;
                        }
                        else if (this.deviceParameters.radiobutton5)
                        {
                            SendStr[70] = 3;
                        }
                        SendStr[71] = 0;
                        if (this.deviceParameters.radiobutton20)
                        {
                            SendStr[72] = 0;
                        }
                        else if (this.deviceParameters.radiobutton19)
                        {
                            SendStr[72] = 1;
                        }
                        else if (this.deviceParameters.radiobutton18)
                        {
                            SendStr[72] = 2;
                        }
                        else if (this.deviceParameters.radiobutton17)
                        {
                            SendStr[72] = 3;
                        }
                        SendStr[73] = 0;
                        if (this.deviceParameters.radiobutton28)
                        {
                            SendStr[74] = 0;
                        }
                        else if (this.deviceParameters.radiobutton27)
                        {
                            SendStr[74] = 1;
                        }
                        else if (this.deviceParameters.radiobutton26)
                        {
                            SendStr[74] = 2;
                        }
                        else if (this.deviceParameters.radiobutton25)
                        {
                            SendStr[74] = 3;
                        }
                        SendStr[75] = Conversions.ToByte(this.deviceParameters.textbox26);
                        SendStr[76] = Conversions.ToByte(this.deviceParameters.textbox27);
                        SendStr[77] = Conversions.ToByte(this.deviceParameters.textbox31);
                        SendStr[78] = Conversions.ToByte(this.deviceParameters.textbox30);
                        SendStr[79] = Conversions.ToByte(this.deviceParameters.textbox34);
                        SendStr[80] = Conversions.ToByte(this.deviceParameters.textbox33);
                        SendStr[81] = Conversions.ToByte(this.deviceParameters.textbox37);
                        SendStr[82] = Conversions.ToByte(this.deviceParameters.textbox36);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox28);
                        SendStr[83] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[84] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox29);
                        SendStr[85] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[86] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox32);
                        SendStr[87] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[88] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox35);
                        SendStr[89] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[90] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        SendStr[91] = 0;
                        SendStr[92] = Conversions.ToByte(this.deviceParameters.textbox40);
                        SendStr[93] = 0;
                        SendStr[94] = Conversions.ToByte(this.deviceParameters.textbox39);
                        SendStr[95] = 0;
                        SendStr[96] = Conversions.ToByte(this.deviceParameters.textbox38);
                        SendStr[97] = 0;
                        SendStr[98] = Conversions.ToByte(this.deviceParameters.textbox41);
                        SendStr[99] = 0;
                        SendStr[100] = Conversions.ToByte(this.deviceParameters.textbox42);
                        SendStr[101] = 0;
                        SendStr[102] = Conversions.ToByte(this.deviceParameters.textbox43);
                        SendStr[103] = 0;
                        SendStr[104] = Conversions.ToByte(this.deviceParameters.textbox47);
                        SendStr[105] = 0;
                        SendStr[106] = Conversions.ToByte(this.deviceParameters.textbox48);
                        SendStr[107] = 0;
                        SendStr[108] = Conversions.ToByte(this.deviceParameters.textbox49);
                        SendStr[109] = 0;
                        SendStr[110] = Conversions.ToByte(this.deviceParameters.textbox44);
                        SendStr[111] = 0;
                        SendStr[112] = Conversions.ToByte(this.deviceParameters.textbox45);
                        SendStr[113] = 0;
                        SendStr[114] = Conversions.ToByte(this.deviceParameters.textbox46);
                        SendStr[115] = Conversions.ToByte(this.deviceParameters.textbox75);
                        SendStr[116] = Conversions.ToByte(this.deviceParameters.textbox72);
                        SendStr[117] = Conversions.ToByte(this.deviceParameters.textbox68);
                        SendStr[118] = Conversions.ToByte(this.deviceParameters.textbox65);
                        SendStr[119] = Conversions.ToByte(this.deviceParameters.textbox82);
                        SendStr[120] = Conversions.ToByte(this.deviceParameters.textbox84);
                        SendStr[121] = Conversions.ToByte(this.deviceParameters.textbox87);
                        SendStr[122] = Conversions.ToByte(this.deviceParameters.textbox89);
                        SendStr[123] = 0;
                        if (!this.deviceParameters.button11)
                        {
                            SendStr[124] = 1;
                        }
                        else
                        {
                            SendStr[124] = 0;
                        }
                        SendStr[125] = 0;
                        if (!this.deviceParameters.button14)
                        {
                            SendStr[126] = 1;
                        }
                        else
                        {
                            SendStr[126] = 0;
                        }
                        SendStr[127] = 0;
                        if (!this.deviceParameters.button16)
                        {
                            SendStr[128] = 1;
                        }
                        else
                        {
                            SendStr[128] = 0;
                        }
                        SendStr[129] = 0;
                        if (!this.deviceParameters.button18)
                        {
                            SendStr[130] = 1;
                        }
                        else
                        {
                            SendStr[130] = 0;
                        }
                        this.crc16(ref SendStr, 130);
                        SendStr[131] = Module1.HiByte;
                        SendStr[132] = Module1.LoByte;
                        Module1.CmdLenth = 8;
                        this.SerialPort1.ReceivedBytesThreshold = 8;
                        this.SerialPort1.Write(SendStr, 0, 133);
                    }
                    else
                    {
                        SendStr[0] = (byte)Module1.ID;
                        SendStr[1] = Module1.buscmd;
                        SendStr[2] = addressH;
                        SendStr[3] = addressL;
                        SendStr[4] = 0;
                        SendStr[5] = mylength;
                        SendStr[6] = Conversions.ToByte(mylength * 2);
                        SendStr[7] = 0;
                        SendStr[8] = Conversions.ToByte(this.deviceParameters.textbox96);
                        SendStr[9] = 0;
                        SendStr[10] = Conversions.ToByte(this.deviceParameters.textbox95);
                        SendStr[11] = 0;
                        SendStr[12] = Conversions.ToByte(this.deviceParameters.textbox93);
                        SendStr[13] = 0;
                        SendStr[14] = Conversions.ToByte(this.deviceParameters.textbox91);
                        SendStr[15] = 0;
                        SendStr[16] = Conversions.ToByte(this.deviceParameters.textbox92);
                        SendStr[17] = 0;
                        SendStr[18] = Conversions.ToByte(this.deviceParameters.textbox94);
                        SendStr[19] = 0;
                        SendStr[20] = Conversions.ToByte(this.deviceParameters.textbox85);
                        SendStr[21] = 0;
                        SendStr[22] = Conversions.ToByte(this.deviceParameters.textbox86);
                        SendStr[23] = 0;
                        SendStr[24] = Conversions.ToByte(this.deviceParameters.textbox88);
                        SendStr[25] = 0;
                        SendStr[26] = Conversions.ToByte(this.deviceParameters.textbox80);
                        SendStr[27] = 0;
                        SendStr[28] = Conversions.ToByte(this.deviceParameters.textbox81);
                        SendStr[29] = 0;
                        SendStr[30] = Conversions.ToByte(this.deviceParameters.textbox83);
                        SendStr[31] = Conversions.ToByte(this.deviceParameters.textbox105);
                        SendStr[32] = Conversions.ToByte(this.deviceParameters.textbox102);
                        SendStr[33] = Conversions.ToByte(this.deviceParameters.textbox99);
                        SendStr[34] = Conversions.ToByte(this.deviceParameters.textbox90);
                        SendStr[35] = Conversions.ToByte(this.deviceParameters.textbox111);
                        SendStr[36] = Conversions.ToByte(this.deviceParameters.textbox113);
                        SendStr[37] = Conversions.ToByte(this.deviceParameters.textbox116);
                        SendStr[38] = Conversions.ToByte(this.deviceParameters.textbox118);
                        SendStr[39] = 0;
                        if (!this.deviceParameters.button34)
                        {
                            SendStr[40] = 1;
                        }
                        else
                        {
                            SendStr[40] = 0;
                        }
                        SendStr[41] = 0;
                        if (!this.deviceParameters.button32)
                        {
                            SendStr[42] = 1;
                        }
                        else
                        {
                            SendStr[42] = 0;
                        }
                        SendStr[43] = 0;
                        if (!this.deviceParameters.button30)
                        {
                            SendStr[44] = 1;
                        }
                        else
                        {
                            SendStr[44] = 0;
                        }
                        SendStr[45] = 0;
                        if (!this.deviceParameters.button28)
                        {
                            SendStr[46] = 1;
                        }
                        else
                        {
                            SendStr[46] = 0;
                        }
                        SendStr[47] = 0;
                        SendStr[48] = Conversions.ToByte(this.deviceParameters.textbox124);
                        SendStr[49] = 0;
                        SendStr[50] = Conversions.ToByte(this.deviceParameters.textbox123);
                        SendStr[51] = 0;
                        SendStr[52] = Conversions.ToByte(this.deviceParameters.textbox121);
                        SendStr[53] = 0;
                        SendStr[54] = Conversions.ToByte(this.deviceParameters.textbox119);
                        SendStr[55] = 0;
                        SendStr[56] = Conversions.ToByte(this.deviceParameters.textbox120);
                        SendStr[57] = 0;
                        SendStr[58] = Conversions.ToByte(this.deviceParameters.textbox122);
                        SendStr[59] = 0;
                        SendStr[60] = Conversions.ToByte(this.deviceParameters.textbox114);
                        SendStr[61] = 0;
                        SendStr[62] = Conversions.ToByte(this.deviceParameters.textbox115);
                        SendStr[63] = 0;
                        SendStr[64] = Conversions.ToByte(this.deviceParameters.textbox117);
                        SendStr[65] = 0;
                        SendStr[66] = Conversions.ToByte(this.deviceParameters.textbox109);
                        SendStr[67] = 0;
                        SendStr[68] = Conversions.ToByte(this.deviceParameters.textbox110);
                        SendStr[69] = 0;
                        SendStr[70] = Conversions.ToByte(this.deviceParameters.textbox112);
                        SendStr[71] = Conversions.ToByte(this.deviceParameters.textbox137);
                        SendStr[72] = Conversions.ToByte(this.deviceParameters.textbox134);
                        SendStr[73] = Conversions.ToByte(this.deviceParameters.textbox131);
                        SendStr[74] = Conversions.ToByte(this.deviceParameters.textbox128);
                        SendStr[75] = Conversions.ToByte(this.deviceParameters.textbox143);
                        SendStr[76] = Conversions.ToByte(this.deviceParameters.textbox145);
                        SendStr[77] = Conversions.ToByte(this.deviceParameters.textbox148);
                        SendStr[78] = Conversions.ToByte(this.deviceParameters.textbox150);
                        SendStr[79] = 0;
                        if (!this.deviceParameters.button26)
                        {
                            SendStr[80] = 1;
                        }
                        else
                        {
                            SendStr[80] = 0;
                        }
                        SendStr[81] = 0;
                        if (!this.deviceParameters.button24)
                        {
                            SendStr[82] = 1;
                        }
                        else
                        {
                            SendStr[82] = 0;
                        }
                        SendStr[83] = 0;
                        if (!this.deviceParameters.button22)
                        {
                            SendStr[84] = 1;
                        }
                        else
                        {
                            SendStr[84] = 0;
                        }
                        SendStr[85] = 0;
                        if (!this.deviceParameters.button20)
                        {
                            SendStr[86] = 1;
                        }
                        else
                        {
                            SendStr[86] = 0;
                        }
                        SendStr[87] = 0;
                        SendStr[88] = Conversions.ToByte(this.deviceParameters.textbox156);
                        SendStr[89] = 0;
                        SendStr[90] = Conversions.ToByte(this.deviceParameters.textbox155);
                        SendStr[91] = 0;
                        SendStr[92] = Conversions.ToByte(this.deviceParameters.textbox153);
                        SendStr[93] = 0;
                        SendStr[94] = Conversions.ToByte(this.deviceParameters.textbox151);
                        SendStr[95] = 0;
                        SendStr[96] = Conversions.ToByte(this.deviceParameters.textbox152);
                        SendStr[97] = 0;
                        SendStr[98] = Conversions.ToByte(this.deviceParameters.textbox154);
                        SendStr[99] = 0;
                        SendStr[100] = Conversions.ToByte(this.deviceParameters.textbox146);
                        SendStr[101] = 0;
                        SendStr[102] = Conversions.ToByte(this.deviceParameters.textbox147);
                        SendStr[103] = 0;
                        SendStr[104] = Conversions.ToByte(this.deviceParameters.textbox149);
                        SendStr[105] = 0;
                        SendStr[106] = Conversions.ToByte(this.deviceParameters.textbox141);
                        SendStr[107] = 0;
                        SendStr[108] = Conversions.ToByte(this.deviceParameters.textbox142);
                        SendStr[109] = 0;
                        SendStr[110] = Conversions.ToByte(this.deviceParameters.textbox144);
                        SendStr[111] = 0;
                        if (!this.deviceParameters.button36)
                        {
                            SendStr[112] = (byte)(SendStr[112] | 1);
                        }
                        else
                        {
                            SendStr[112] = (byte)(SendStr[112] & 254);
                        }
                        if (!this.deviceParameters.button40)
                        {
                            SendStr[112] = (byte)(SendStr[112] | 2);
                        }
                        else
                        {
                            SendStr[112] = (byte)(SendStr[112] & 253);
                        }
                        if (!this.deviceParameters.button38)
                        {
                            SendStr[112] = (byte)(SendStr[112] | 4);
                        }
                        else
                        {
                            SendStr[112] = (byte)(SendStr[112] & 251);
                        }
                        if (!this.deviceParameters.button42)
                        {
                            SendStr[112] = (byte)(SendStr[112] | 8);
                        }
                        else
                        {
                            SendStr[112] = (byte)(SendStr[112] & 247);
                        }
                        float mytemp = Conversions.ToSingle(this.deviceParameters.textbox53);
                        SendStr[113] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[114] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox15);
                        SendStr[115] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[116] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox50);
                        SendStr[117] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[118] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox51);
                        SendStr[119] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[120] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        mytemp = Conversions.ToSingle(this.deviceParameters.textbox52);
                        SendStr[121] = (byte)((long)Math.Round((double)mytemp) / 256L);
                        SendStr[122] = (byte)((long)Math.Round((double)mytemp) & 255L);
                        SendStr[123] = 0;
                        if (!this.deviceParameters.button1)
                        {
                            SendStr[124] = (byte)(SendStr[124] | 1);
                        }
                        else
                        {
                            SendStr[124] = (byte)(SendStr[124] & 254);
                        }
                        if (!this.deviceParameters.button4)
                        {
                            SendStr[124] = (byte)(SendStr[124] | 2);
                        }
                        else
                        {
                            SendStr[124] = (byte)(SendStr[124] & 253);
                        }
                        if (!this.deviceParameters.button6)
                        {
                            SendStr[124] = (byte)(SendStr[124] | 4);
                        }
                        else
                        {
                            SendStr[124] = (byte)(SendStr[124] & 251);
                        }
                        if (!this.deviceParameters.button8)
                        {
                            SendStr[124] = (byte)(SendStr[124] | 8);
                        }
                        else
                        {
                            SendStr[124] = (byte)(SendStr[124] & 247);
                        }
                        SendStr[125] = 0;
                        SendStr[126] = 120;
                        SendStr[127] = 0;
                        SendStr[128] = 120;
                        this.crc16(ref SendStr, 128);
                        SendStr[129] = Module1.HiByte;
                        SendStr[130] = Module1.LoByte;
                        Module1.CmdLenth = 8;
                        this.SerialPort1.ReceivedBytesThreshold = 8;
                        this.SerialPort1.Write(SendStr, 0, 131);
                    }
                }
                else if (Module1.buscmd == 2)
                {
                    SendStr[0] = (byte)Module1.ID;
                    SendStr[1] = Module1.buscmd;
                    SendStr[2] = addressH;
                    SendStr[3] = addressL;
                    SendStr[4] = 0;
                    SendStr[5] = mylength;
                    SendStr[6] = Conversions.ToByte(mylength * 2);
                    int flashstartaddress = (int)addressL;
                    flashstartaddress *= 128;
                    int i = 0;
                    do
                    {
                        SendStr[7 + i] = Module1.flashback[i + flashstartaddress];
                        i++;
                    }
                    while (i <= 127);
                    this.crc16(ref SendStr, 134);
                    SendStr[135] = Module1.HiByte;
                    SendStr[136] = Module1.LoByte;
                    Module1.CmdLenth = (int)(5 + SendStr[5] * 2);
                    this.SerialPort1.ReceivedBytesThreshold = 8;
                    this.SerialPort1.Write(SendStr, 0, 137);
                }
                else
                {
                    SendStr[0] = (byte)Module1.ID;
                    if (myaddress == 8192)
                    {
                        SendStr[0] = 0;
                    }
                    SendStr[1] = Module1.buscmd;
                    SendStr[2] = addressH;
                    SendStr[3] = addressL;
                    SendStr[4] = 0;
                    SendStr[5] = mylength;
                    this.crc16(ref SendStr, 5);
                    SendStr[6] = Module1.HiByte;
                    SendStr[7] = Module1.LoByte;
                    Module1.CmdLenth = (int)(5 + SendStr[5] * 2);
                    this.SerialPort1.ReceivedBytesThreshold = Module1.CmdLenth;
                    this.SerialPort1.Write(SendStr, 0, 8);
                }
                Thread.Sleep(1000);
                object Sendcmd = new object();
                return Sendcmd;
            }
        }

        private async Task<bool> Check_Device(int deviceId, string ComPort)
        {
            this.SerialPort1.PortName = ComPort;
            this.SerialPort1.ReadBufferSize = 1000;
            this.SerialPort1.ReceivedBytesThreshold = 64;
            this.SerialPort1.WriteBufferSize = 100;
            this.SerialPort1.Open();
            Module1.CmdLenth = 8;
            Module1.ID = deviceId;
            Module1.flag_checking = 1;
            object cmd = null;
            cmd = await this.Sendcmd(12544, 4, 32);


            this.recBuf = new byte[101];
            this.i = this.SerialPort1.BytesToRead;
            this.Textfault = Conversions.ToString(this.i);
            if (this.SerialPort1.BytesToRead > Module1.CmdLenth)
            {
                this.SerialPort1.DiscardInBuffer();
            }
            checked
            {
                if (this.SerialPort1.BytesToRead == Module1.CmdLenth)
                {
                    this.i = 0;
                    int? x = null;
                    x = this.SerialPort1.Read(recBuf, 0, Module1.CmdLenth);

                    if ((int)this.recBuf[0] == Module1.ID & this.recBuf[1] == Module1.buscmd)
                    {
                        this.crc16(ref this.recBuf, Module1.CmdLenth - 3);
                        if (this.recBuf[Module1.CmdLenth - 2] == Module1.HiByte & this.recBuf[Module1.CmdLenth - 1] == Module1.LoByte)
                        {
                            this.Flag_recok = 1;
                            if (this.cycle == 0)
                            {
                                this.BV_Real = (float)((int)this.recBuf[11] * 256);
                                unchecked
                                {
                                    this.SOC_Real = (int)this.recBuf[66];
                                    this.LI_Real = (float)(checked((int)this.recBuf[29] * 256));
                                    this.LI_Real += (float)this.recBuf[30];
                                }
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }


            }
            //this.PortClose();
        }
        

        private void Read_parameters()
        {
            checked
            {
                this.tickcount2 += 1;
                this.totaltickcount++;
                byte i = (byte)this.SerialPort1.BytesToRead;
                if ((int)i > Module1.CmdLenth)
                {
                    this.SerialPort1.DiscardInBuffer();
                }
                if ((int)i == Module1.CmdLenth)
                {
                    this.SerialPort1.Read(this.recbuf2, 0, Module1.CmdLenth);
                    if ((int)this.recbuf2[0] == Module1.ID & this.recbuf2[1] == Module1.buscmd)
                    {
                        this.crc16(ref this.recbuf2, Module1.CmdLenth - 3);
                        if (this.recbuf2[Module1.CmdLenth - 2] == Module1.HiByte & this.recbuf2[Module1.CmdLenth - 1] == Module1.LoByte)
                        {
                            this.Flag_recok2 = 1;
                            if (Module1.buscmd == 3)
                            {
                                if (this.flag_Pararead1 == 1)
                                {
                                    this.parameters.HVD = (float)((int)this.recbuf2[7] * 256);
                                    unchecked
                                    {
                                        this.parameters.HVD += (float)this.recbuf2[8];
                                        this.parameters.HVD /= 100f;
                                        this.parameters.CLV = (float)(checked((int)this.recbuf2[9] * 256));
                                        this.parameters.CLV += (float)this.recbuf2[10];
                                        this.parameters.CLV /= 100f;
                                        this.parameters.OVR = (float)(checked((int)this.recbuf2[11] * 256));
                                        this.parameters.OVR += (float)this.recbuf2[12];
                                        this.parameters.OVR /= 100f;
                                        this.parameters.ECV = (float)(checked((int)this.recbuf2[13] * 256));
                                        this.parameters.ECV += (float)this.recbuf2[14];
                                        this.parameters.ECV /= 100f;
                                        this.parameters.BCV = (float)(checked((int)this.recbuf2[15] * 256));
                                        this.parameters.BCV += (float)this.recbuf2[16];
                                        this.parameters.BCV /= 100f;
                                        this.parameters.FCV = (float)(checked((int)this.recbuf2[17] * 256));
                                        this.parameters.FCV += (float)this.recbuf2[18];
                                        this.parameters.FCV /= 100f;
                                        this.parameters.BVR = (float)(checked((int)this.recbuf2[19] * 256));
                                        this.parameters.BVR += (float)this.recbuf2[20];
                                        this.parameters.BVR /= 100f;
                                        this.parameters.LVR = (float)(checked((int)this.recbuf2[21] * 256));
                                        this.parameters.LVR += (float)this.recbuf2[22];
                                        this.parameters.LVR /= 100f;
                                        this.parameters.UVWR = (float)(checked((int)this.recbuf2[23] * 256));
                                        this.parameters.UVWR += (float)this.recbuf2[24];
                                        this.parameters.UVWR /= 100f;
                                        this.parameters.UVW = (float)(checked((int)this.recbuf2[25] * 256));
                                        this.parameters.UVW += (float)this.recbuf2[26];
                                        this.parameters.UVW /= 100f;
                                        this.parameters.LVD1 = (float)(checked((int)this.recbuf2[27] * 256));
                                        this.parameters.LVD1 += (float)this.recbuf2[28];
                                        this.parameters.LVD1 /= 100f;
                                        this.parameters.LVD2 = (float)(checked((int)this.recbuf2[29] * 256));
                                        this.parameters.LVD2 += (float)this.recbuf2[30];
                                        this.parameters.LVD2 /= 100f;
                                        this.parameters.LVD3 = (float)(checked((int)this.recbuf2[31] * 256));
                                        this.parameters.LVD3 += (float)this.recbuf2[32];
                                        this.parameters.LVD3 /= 100f;
                                        this.parameters.LVD4 = (float)(checked((int)this.recbuf2[33] * 256));
                                        this.parameters.LVD4 += (float)this.recbuf2[34];
                                        this.parameters.LVD4 /= 100f;
                                    }
                                    this.parameters.BV_TempH = (int)this.recbuf2[47] * 256;
                                    this.parameters.BV_TempH += (int)this.recbuf2[48];
                                    this.parameters.BV_TempL = 65496;
                                    this.parameters.CN_TempH = (int)this.recbuf2[51] * 256;
                                    this.parameters.CN_TempH += (int)this.recbuf2[52];
                                    this.parameters.CN_TempL = (int)this.recbuf2[53] * 256;
                                    this.parameters.CN_TempL += (int)this.recbuf2[54];
                                    this.parameters.NightVoltage = (float)((int)this.recbuf2[55] * 256);
                                    unchecked
                                    {
                                        this.parameters.NightVoltage += (float)this.recbuf2[56];
                                        this.parameters.NightVoltage /= 100f;
                                    }
                                    this.parameters.NightDelay = (int)this.recbuf2[57] * 256;
                                    this.parameters.NightDelay += (int)this.recbuf2[58];
                                    this.parameters.NightDelay = this.parameters.NightDelay;
                                    this.parameters.DayVoltege = (float)((int)this.recbuf2[59] * 256);
                                    unchecked
                                    {
                                        this.parameters.DayVoltege += (float)this.recbuf2[60];
                                        this.parameters.DayVoltege /= 100f;
                                    }
                                    this.parameters.DayDelay = (int)this.recbuf2[61] * 256;
                                    this.parameters.DayDelay += (int)this.recbuf2[62];
                                    this.parameters.DayDelay = this.parameters.DayDelay;
                                    this.parameters.LoadLOutCtrlMod = this.recbuf2[64];
                                    this.parameters.Load1OutCtrlMod = this.recbuf2[66];
                                    this.parameters.Load2OutCtrlMod = this.recbuf2[68];
                                    this.parameters.Load3OutCtrlMod = this.recbuf2[70];
                                    this.parameters.LoadLWorkTime1H = this.recbuf2[71];
                                    this.parameters.LoadLWorkTime1M = this.recbuf2[72];
                                    this.parameters.LoadLWorkTime2H = this.recbuf2[73];
                                    this.parameters.LoadLWorkTime2M = this.recbuf2[74];
                                    this.parameters.LoadLWorkTime3H = this.recbuf2[75];
                                    this.parameters.LoadLWorkTime3M = this.recbuf2[76];
                                    this.parameters.LoadLWorkTime4H = this.recbuf2[77];
                                    this.parameters.LoadLWorkTime4M = this.recbuf2[78];
                                    this.parameters.LoadLPower1 = (int)this.recbuf2[79] * 256;
                                    this.parameters.LoadLPower1 += (int)this.recbuf2[80];
                                    this.parameters.LoadLPower2 = (int)this.recbuf2[81] * 256;
                                    this.parameters.LoadLPower2 += (int)this.recbuf2[82];
                                    this.parameters.LoadLPower3 = (int)this.recbuf2[83] * 256;
                                    this.parameters.LoadLPower3 += (int)this.recbuf2[84];
                                    this.parameters.LoadLPower4 = (int)this.recbuf2[85] * 256;
                                    this.parameters.LoadLPower4 += (int)this.recbuf2[86];
                                    this.parameters.LoadLOnTimeS1 = this.recbuf2[88];
                                    this.parameters.LoadLOnTimeM1 = this.recbuf2[90];
                                    this.parameters.LoadLOnTimeH1 = this.recbuf2[92];
                                    this.parameters.LoadLOffTimeS1 = this.recbuf2[94];
                                    this.parameters.LoadLOffTimeM1 = this.recbuf2[96];
                                    this.parameters.LoadLOffTimeH1 = this.recbuf2[98];
                                    this.parameters.LoadLOnTimeS2 = this.recbuf2[100];
                                    this.parameters.LoadLOnTimeM2 = this.recbuf2[102];
                                    this.parameters.LoadLOnTimeH2 = this.recbuf2[104];
                                    this.parameters.LoadLOffTimeS2 = this.recbuf2[106];
                                    this.parameters.LoadLOffTimeM2 = this.recbuf2[108];
                                    this.parameters.LoadLOffTimeH2 = this.recbuf2[110];
                                    this.parameters.Load1WorkTime1H = this.recbuf2[111];
                                    this.parameters.Load1WorkTime1M = this.recbuf2[112];
                                    this.parameters.Load1WorkTime2H = this.recbuf2[113];
                                    this.parameters.Load1WorkTime2M = this.recbuf2[114];
                                    this.parameters.Load1WorkTime3H = this.recbuf2[115];
                                    this.parameters.Load1WorkTime3M = this.recbuf2[116];
                                    this.parameters.Load1WorkTime4H = this.recbuf2[117];
                                    this.parameters.Load1WorkTime4M = this.recbuf2[118];
                                    this.parameters.Load1Power1 = (int)this.recbuf2[119] * 256;
                                    this.parameters.Load1Power1 += (int)this.recbuf2[120];
                                    this.parameters.Load1Power1 = this.parameters.Load1Power1;
                                    this.parameters.Load1Power2 = (int)this.recbuf2[121] * 256;
                                    this.parameters.Load1Power2 += (int)this.recbuf2[122];
                                    this.parameters.Load1Power2 = this.parameters.Load1Power2;
                                    this.parameters.Load1Power3 = (int)this.recbuf2[123] * 256;
                                    this.parameters.Load1Power3 += (int)this.recbuf2[124];
                                    this.parameters.Load1Power3 = this.parameters.Load1Power3;
                                }
                                else
                                {
                                    this.parameters.Load1Power4 = (int)this.recbuf2[3] * 256;
                                    this.parameters.Load1Power4 += (int)this.recbuf2[4];
                                    this.parameters.Load1Power4 = this.parameters.Load1Power4;
                                    this.parameters.Load1OnTimeS1 = this.recbuf2[6];
                                    this.parameters.Load1OnTimeM1 = this.recbuf2[8];
                                    this.parameters.Load1OnTimeH1 = this.recbuf2[10];
                                    this.parameters.Load1OffTimeS1 = this.recbuf2[12];
                                    this.parameters.Load1OffTimeM1 = this.recbuf2[14];
                                    this.parameters.Load1OffTimeH1 = this.recbuf2[16];
                                    this.parameters.Load1OnTimeS2 = this.recbuf2[18];
                                    this.parameters.Load1OnTimeM2 = this.recbuf2[20];
                                    this.parameters.Load1OnTimeH2 = this.recbuf2[22];
                                    this.parameters.Load1OffTimeS2 = this.recbuf2[24];
                                    this.parameters.Load1OffTimeM2 = this.recbuf2[26];
                                    this.parameters.Load1OffTimeH2 = this.recbuf2[28];
                                    this.parameters.Load2WorkTime1H = this.recbuf2[29];
                                    this.parameters.Load2WorkTime1M = this.recbuf2[30];
                                    this.parameters.Load2WorkTime2H = this.recbuf2[31];
                                    this.parameters.Load2WorkTime2M = this.recbuf2[32];
                                    this.parameters.Load2WorkTime3H = this.recbuf2[33];
                                    this.parameters.Load2WorkTime3M = this.recbuf2[34];
                                    this.parameters.Load2WorkTime4H = this.recbuf2[35];
                                    this.parameters.Load2WorkTime4M = this.recbuf2[36];
                                    this.parameters.Load2Power1 = (int)this.recbuf2[37] * 256;
                                    this.parameters.Load2Power1 += (int)this.recbuf2[38];
                                    this.parameters.Load2Power1 = this.parameters.Load2Power1;
                                    this.parameters.Load2Power2 = (int)this.recbuf2[39] * 256;
                                    this.parameters.Load2Power2 += (int)this.recbuf2[40];
                                    this.parameters.Load2Power2 = this.parameters.Load2Power2;
                                    this.parameters.Load2Power3 = (int)this.recbuf2[41] * 256;
                                    this.parameters.Load2Power3 += (int)this.recbuf2[42];
                                    this.parameters.Load2Power3 = this.parameters.Load2Power3;
                                    this.parameters.Load2Power4 = (int)this.recbuf2[43] * 256;
                                    this.parameters.Load2Power4 += (int)this.recbuf2[44];
                                    this.parameters.Load2Power4 = this.parameters.Load2Power4;
                                    this.parameters.Load2OnTimeS1 = this.recbuf2[46];
                                    this.parameters.Load2OnTimeM1 = this.recbuf2[48];
                                    this.parameters.Load2OnTimeH1 = this.recbuf2[50];
                                    this.parameters.Load2OffTimeS1 = this.recbuf2[52];
                                    this.parameters.Load2OffTimeM1 = this.recbuf2[54];
                                    this.parameters.Load2OffTimeH1 = this.recbuf2[56];
                                    this.parameters.Load2OnTimeS2 = this.recbuf2[58];
                                    this.parameters.Load2OnTimeM2 = this.recbuf2[60];
                                    this.parameters.Load2OnTimeH2 = this.recbuf2[62];
                                    this.parameters.Load2OffTimeS2 = this.recbuf2[64];
                                    this.parameters.Load2OffTimeM2 = this.recbuf2[66];
                                    this.parameters.Load2OffTimeH2 = this.recbuf2[68];
                                    this.parameters.Load3WorkTime1H = this.recbuf2[69];
                                    this.parameters.Load3WorkTime1M = this.recbuf2[70];
                                    this.parameters.Load3WorkTime2H = this.recbuf2[71];
                                    this.parameters.Load3WorkTime2M = this.recbuf2[72];
                                    this.parameters.Load3WorkTime3H = this.recbuf2[73];
                                    this.parameters.Load3WorkTime3M = this.recbuf2[74];
                                    this.parameters.Load3WorkTime4H = this.recbuf2[75];
                                    this.parameters.Load3WorkTime4M = this.recbuf2[76];
                                    this.parameters.Load3Power1 = (int)this.recbuf2[77] * 256;
                                    this.parameters.Load3Power1 += (int)this.recbuf2[78];
                                    this.parameters.Load3Power1 = this.parameters.Load3Power1;
                                    this.parameters.Load3Power2 = (int)this.recbuf2[79] * 256;
                                    this.parameters.Load3Power2 += (int)this.recbuf2[80];
                                    this.parameters.Load3Power2 = this.parameters.Load3Power2;
                                    this.parameters.Load3Power3 = (int)this.recbuf2[81] * 256;
                                    this.parameters.Load3Power3 += (int)this.recbuf2[82];
                                    this.parameters.Load3Power3 = this.parameters.Load3Power3;
                                    this.parameters.Load3Power4 = (int)this.recbuf2[83] * 256;
                                    this.parameters.Load3Power4 += (int)this.recbuf2[84];
                                    this.parameters.Load3Power4 = this.parameters.Load3Power4;
                                    this.parameters.Load3OnTimeS1 = this.recbuf2[86];
                                    this.parameters.Load3OnTimeM1 = this.recbuf2[88];
                                    this.parameters.Load3OnTimeH1 = this.recbuf2[90];
                                    this.parameters.Load3OffTimeS1 = this.recbuf2[92];
                                    this.parameters.Load3OffTimeM1 = this.recbuf2[94];
                                    this.parameters.Load3OffTimeH1 = this.recbuf2[96];
                                    this.parameters.Load3OnTimeS2 = this.recbuf2[98];
                                    this.parameters.Load3OnTimeM2 = this.recbuf2[100];
                                    this.parameters.Load3OnTimeH2 = this.recbuf2[102];
                                    this.parameters.Load3OffTimeS2 = this.recbuf2[104];
                                    this.parameters.Load3OffTimeM2 = this.recbuf2[106];
                                    this.parameters.Load3OffTimeH2 = this.recbuf2[108];
                                    this.parameters.XintControl = this.recbuf2[110];
                                    this.parameters.XintDelay = this.recbuf2[112];
                                    this.parameters.XintPower1 = (int)this.recbuf2[113] * 256;
                                    this.parameters.XintPower1 += (int)this.recbuf2[114];
                                    this.parameters.XintPower2 = (int)this.recbuf2[115] * 256;
                                    this.parameters.XintPower2 += (int)this.recbuf2[116];
                                    this.parameters.XintPower3 = (int)this.recbuf2[117] * 256;
                                    this.parameters.XintPower3 += (int)this.recbuf2[118];
                                    this.parameters.XintPower4 = (int)this.recbuf2[119] * 256;
                                    this.parameters.XintPower4 += (int)this.recbuf2[120];
                                    this.parameters.ManualControl = this.recbuf2[122];
                                }
                            }
                        }
                    }
                }
                if (this.Flag_recok2 == 1)
                {
                    this.Flag_recok2 = 0;
                    if (Module1.buscmd == 3)
                    {
                        if (this.flag_Pararead1 == 1)
                        {
                            this.flag_Pararead1 = 2;
                            this.Sendcmd(36925, Module1.buscmd, 62);
                            Thread.Sleep(1000);
                            this.Read_parameters();
                        }
                        else
                        {
                            this.deviceParameters.textbox1 = this.parameters.HVD.ToString("F2");
                            this.deviceParameters.textbox2 = this.parameters.CLV.ToString("F2");
                            this.deviceParameters.textbox3 = this.parameters.OVR.ToString("F2");
                            this.deviceParameters.textbox4 = this.parameters.ECV.ToString("F2");
                            this.deviceParameters.textbox5 = this.parameters.BCV.ToString("F2");
                            this.deviceParameters.textbox6 = this.parameters.FCV.ToString("F2");
                            this.deviceParameters.textbox7 = this.parameters.BVR.ToString("F2");
                            this.deviceParameters.textbox8 = this.parameters.LVR.ToString("F2");
                            this.deviceParameters.textbox9 = this.parameters.UVWR.ToString("F2");
                            this.deviceParameters.textbox10 = this.parameters.UVW.ToString("F2");
                            this.deviceParameters.textbox11 = this.parameters.LVD1.ToString("F2");
                            this.deviceParameters.textbox12 = this.parameters.LVD2.ToString("F2");
                            this.deviceParameters.textbox13 = this.parameters.LVD3.ToString("F2");
                            this.deviceParameters.textbox14 = this.parameters.LVD4.ToString("F2");
                            this.deviceParameters.textbox18 = this.parameters.BV_TempH.ToString("F0");
                            this.deviceParameters.textbox20 = this.parameters.CN_TempH.ToString("F0");
                            this.deviceParameters.textbox21 = this.parameters.CN_TempL.ToString("F0");
                            this.deviceParameters.textbox22 = this.parameters.NightVoltage.ToString("F2");
                            this.deviceParameters.textbox23 = this.parameters.NightDelay.ToString("F0");
                            this.deviceParameters.textbox25 = this.parameters.DayVoltege.ToString("F2");
                            this.deviceParameters.textbox24 = this.parameters.DayDelay.ToString("F0");
                            if (this.parameters.LoadLOutCtrlMod == 0)
                            {
                                this.deviceParameters.radiobutton1 = true;
                                this.deviceParameters.radiobutton2 = false;
                                this.deviceParameters.radiobutton3 = false;
                                this.deviceParameters.radiobutton4 = false;
                            }
                            else if (this.parameters.LoadLOutCtrlMod == 1)
                            {
                                this.deviceParameters.radiobutton1 = false;
                                this.deviceParameters.radiobutton2 = true;
                                this.deviceParameters.radiobutton3 = false;
                                this.deviceParameters.radiobutton4 = false;
                            }
                            else if (this.parameters.LoadLOutCtrlMod == 2)
                            {
                                this.deviceParameters.radiobutton1 = false;
                                this.deviceParameters.radiobutton2 = false;
                                this.deviceParameters.radiobutton3 = true;
                                this.deviceParameters.radiobutton4 = false;
                            }
                            else
                            {
                                this.deviceParameters.radiobutton1 = false;
                                this.deviceParameters.radiobutton2 = false;
                                this.deviceParameters.radiobutton3 = false;
                                this.deviceParameters.radiobutton4 = true;
                            }
                            this.deviceParameters.textbox26 = this.parameters.LoadLWorkTime1H.ToString("F0");
                            this.deviceParameters.textbox27 = this.parameters.LoadLWorkTime1M.ToString("F0");
                            this.deviceParameters.textbox28 = this.parameters.LoadLPower1.ToString("F0");
                            this.deviceParameters.textbox31 = this.parameters.LoadLWorkTime2H.ToString("F0");
                            this.deviceParameters.textbox30 = this.parameters.LoadLWorkTime2M.ToString("F0");
                            this.deviceParameters.textbox29 = this.parameters.LoadLPower2.ToString("F0");
                            this.deviceParameters.textbox34 = this.parameters.LoadLWorkTime3H.ToString("F0");
                            this.deviceParameters.textbox33 = this.parameters.LoadLWorkTime3M.ToString("F0");
                            this.deviceParameters.textbox32 = this.parameters.LoadLPower3.ToString("F0");
                            this.deviceParameters.textbox37 = this.parameters.LoadLWorkTime4H.ToString("F0");
                            this.deviceParameters.textbox36 = this.parameters.LoadLWorkTime4M.ToString("F0");
                            this.deviceParameters.textbox35 = this.parameters.LoadLPower4.ToString("F0");
                            this.deviceParameters.textbox38 = this.parameters.LoadLOnTimeH1.ToString("F0");
                            this.deviceParameters.textbox39 = this.parameters.LoadLOnTimeM1.ToString("F0");
                            this.deviceParameters.textbox40 = this.parameters.LoadLOnTimeS1.ToString("F0");
                            this.deviceParameters.textbox43 = this.parameters.LoadLOffTimeH1.ToString("F0");
                            this.deviceParameters.textbox42 = this.parameters.LoadLOffTimeM1.ToString("F0");
                            this.deviceParameters.textbox41 = this.parameters.LoadLOffTimeS1.ToString("F0");
                            this.deviceParameters.textbox49 = this.parameters.LoadLOnTimeH2.ToString("F0");
                            this.deviceParameters.textbox48 = this.parameters.LoadLOnTimeM2.ToString("F0");
                            this.deviceParameters.textbox47 = this.parameters.LoadLOnTimeS2.ToString("F0");
                            this.deviceParameters.textbox46 = this.parameters.LoadLOffTimeH2.ToString("F0");
                            this.deviceParameters.textbox45 = this.parameters.LoadLOffTimeM2.ToString("F0");
                            this.deviceParameters.textbox44 = this.parameters.LoadLOffTimeS2.ToString("F0");
                            if (this.parameters.Load1OutCtrlMod == 0)
                            {
                                this.deviceParameters.radiobutton8 = true;
                                this.deviceParameters.radiobutton7 = false;
                                this.deviceParameters.radiobutton6 = false;
                                this.deviceParameters.radiobutton5 = false;
                            }
                            else if (this.parameters.Load1OutCtrlMod == 1)
                            {
                                this.deviceParameters.radiobutton8 = false;
                                this.deviceParameters.radiobutton7 = true;
                                this.deviceParameters.radiobutton6 = false;
                                this.deviceParameters.radiobutton5 = false;
                            }
                            else if (this.parameters.Load1OutCtrlMod == 2)
                            {
                                this.deviceParameters.radiobutton8 = false;
                                this.deviceParameters.radiobutton7 = false;
                                this.deviceParameters.radiobutton6 = true;
                                this.deviceParameters.radiobutton5 = false;
                            }
                            else
                            {
                                this.deviceParameters.radiobutton8 = false;
                                this.deviceParameters.radiobutton7 = false;
                                this.deviceParameters.radiobutton6 = false;
                                this.deviceParameters.radiobutton5 = true;
                            }
                            this.deviceParameters.textbox75 = this.parameters.Load1WorkTime1H.ToString("F0");
                            this.deviceParameters.textbox72 = this.parameters.Load1WorkTime1M.ToString("F0");
                            if (this.parameters.Load1Power1 == 1)
                            {
                                this.deviceParameters.button11 = false;
                                this.deviceParameters.button12 = true;
                            }
                            else
                            {
                                this.deviceParameters.button12 = false;
                                this.deviceParameters.button11 = true;
                            }
                            this.deviceParameters.textbox68 = this.parameters.Load1WorkTime2H.ToString("F0");
                            this.deviceParameters.textbox65 = this.parameters.Load1WorkTime2M.ToString("F0");
                            if (this.parameters.Load1Power2 == 1)
                            {
                                this.deviceParameters.button14 = false;
                                this.deviceParameters.button13 = true;
                            }
                            else
                            {
                                this.deviceParameters.button13 = false;
                                this.deviceParameters.button14 = true;
                            }
                            this.deviceParameters.textbox82 = this.parameters.Load1WorkTime3H.ToString("F0");
                            this.deviceParameters.textbox84 = this.parameters.Load1WorkTime3M.ToString("F0");
                            if (this.parameters.Load1Power3 == 1)
                            {
                                this.deviceParameters.button16 = false;
                                this.deviceParameters.button15 = true;
                            }
                            else
                            {
                                this.deviceParameters.button15 = false;
                                this.deviceParameters.button16 = true;
                            }
                            this.deviceParameters.textbox87 = this.parameters.Load1WorkTime4H.ToString("F0");
                            this.deviceParameters.textbox89 = this.parameters.Load1WorkTime4M.ToString("F0");
                            if (this.parameters.Load1Power4 == 1)
                            {
                                this.deviceParameters.button18 = false;
                                this.deviceParameters.button17 = true;
                            }
                            else
                            {
                                this.deviceParameters.button17 = false;
                                this.deviceParameters.button18 = true;
                            }
                            this.deviceParameters.textbox93 = this.parameters.Load1OnTimeH1.ToString("F0");
                            this.deviceParameters.textbox95 = this.parameters.Load1OnTimeM1.ToString("F0");
                            this.deviceParameters.textbox96 = this.parameters.Load1OnTimeS1.ToString("F0");
                            this.deviceParameters.textbox94 = this.parameters.Load1OffTimeH1.ToString("F0");
                            this.deviceParameters.textbox92 = this.parameters.Load1OffTimeM1.ToString("F0");
                            this.deviceParameters.textbox91 = this.parameters.Load1OffTimeS1.ToString("F0");
                            this.deviceParameters.textbox88 = this.parameters.Load1OnTimeH2.ToString("F0");
                            this.deviceParameters.textbox86 = this.parameters.Load1OnTimeM2.ToString("F0");
                            this.deviceParameters.textbox85 = this.parameters.Load1OnTimeS2.ToString("F0");
                            this.deviceParameters.textbox83 = this.parameters.Load1OffTimeH2.ToString("F0");
                            this.deviceParameters.textbox81 = this.parameters.Load1OffTimeM2.ToString("F0");
                            this.deviceParameters.textbox80 = this.parameters.Load1OffTimeS2.ToString("F0");
                            if (this.parameters.Load2OutCtrlMod == 0)
                            {
                                this.deviceParameters.radiobutton20 = true;
                                this.deviceParameters.radiobutton19 = false;
                                this.deviceParameters.radiobutton18 = false;
                                this.deviceParameters.radiobutton17 = false;
                            }
                            else if (this.parameters.Load2OutCtrlMod == 1)
                            {
                                this.deviceParameters.radiobutton20 = false;
                                this.deviceParameters.radiobutton19 = true;
                                this.deviceParameters.radiobutton18 = false;
                                this.deviceParameters.radiobutton17 = false;
                            }
                            else if (this.parameters.Load2OutCtrlMod == 2)
                            {
                                this.deviceParameters.radiobutton20 = false;
                                this.deviceParameters.radiobutton19 = false;
                                this.deviceParameters.radiobutton18 = true;
                                this.deviceParameters.radiobutton17 = false;
                            }
                            else
                            {
                                this.deviceParameters.radiobutton20 = false;
                                this.deviceParameters.radiobutton19 = false;
                                this.deviceParameters.radiobutton18 = false;
                                this.deviceParameters.radiobutton17 = true;
                            }
                            this.deviceParameters.textbox105 = this.parameters.Load2WorkTime1H.ToString("F0");
                            this.deviceParameters.textbox102 = this.parameters.Load2WorkTime1M.ToString("F0");
                            if (this.parameters.Load2Power1 == 1)
                            {
                                this.deviceParameters.button34 = false;
                                this.deviceParameters.button33 = true;
                            }
                            else
                            {
                                this.deviceParameters.button33 = false;
                                this.deviceParameters.button34 = true;
                            }
                            this.deviceParameters.textbox99 = this.parameters.Load2WorkTime2H.ToString("F0");
                            this.deviceParameters.textbox90 = this.parameters.Load2WorkTime2M.ToString("F0");
                            if (this.parameters.Load2Power2 == 1)
                            {
                                this.deviceParameters.button32 = false;
                                this.deviceParameters.button31 = true;
                            }
                            else
                            {
                                this.deviceParameters.button31 = false;
                                this.deviceParameters.button32 = true;
                            }
                            this.deviceParameters.textbox111 = this.parameters.Load2WorkTime3H.ToString("F0");
                            this.deviceParameters.textbox113 = this.parameters.Load2WorkTime3M.ToString("F0");
                            if (this.parameters.Load2Power3 == 1)
                            {
                                this.deviceParameters.button30 = false;
                                this.deviceParameters.button29 = true;
                            }
                            else
                            {
                                this.deviceParameters.button29 = false;
                                this.deviceParameters.button30 = true;
                            }
                            this.deviceParameters.textbox116 = this.parameters.Load2WorkTime4H.ToString("F0");
                            this.deviceParameters.textbox118 = this.parameters.Load2WorkTime4M.ToString("F0");
                            if (this.parameters.Load2Power4 == 1)
                            {
                                this.deviceParameters.button28 = false;
                                this.deviceParameters.button27 = true;
                            }
                            else
                            {
                                this.deviceParameters.button27 = false;
                                this.deviceParameters.button28 = true;
                            }
                            this.deviceParameters.textbox121 = this.parameters.Load2OnTimeH1.ToString("F0");
                            this.deviceParameters.textbox123 = this.parameters.Load2OnTimeM1.ToString("F0");
                            this.deviceParameters.textbox124 = this.parameters.Load2OnTimeS1.ToString("F0");
                            this.deviceParameters.textbox122 = this.parameters.Load2OffTimeH1.ToString("F0");
                            this.deviceParameters.textbox120 = this.parameters.Load2OffTimeM1.ToString("F0");
                            this.deviceParameters.textbox119 = this.parameters.Load2OffTimeS1.ToString("F0");
                            this.deviceParameters.textbox117 = this.parameters.Load2OnTimeH2.ToString("F0");
                            this.deviceParameters.textbox115 = this.parameters.Load2OnTimeM2.ToString("F0");
                            this.deviceParameters.textbox114 = this.parameters.Load2OnTimeS2.ToString("F0");
                            this.deviceParameters.textbox112 = this.parameters.Load2OffTimeH2.ToString("F0");
                            this.deviceParameters.textbox110 = this.parameters.Load2OffTimeM2.ToString("F0");
                            this.deviceParameters.textbox109 = this.parameters.Load2OffTimeS2.ToString("F0");
                            if (this.parameters.Load3OutCtrlMod == 0)
                            {
                                this.deviceParameters.radiobutton28 = true;
                                this.deviceParameters.radiobutton27 = false;
                                this.deviceParameters.radiobutton26 = false;
                                this.deviceParameters.radiobutton25 = false;
                            }
                            else if (this.parameters.Load3OutCtrlMod == 1)
                            {
                                this.deviceParameters.radiobutton28 = false;
                                this.deviceParameters.radiobutton27 = true;
                                this.deviceParameters.radiobutton26 = false;
                                this.deviceParameters.radiobutton25 = false;
                            }
                            else if (this.parameters.Load3OutCtrlMod == 2)
                            {
                                this.deviceParameters.radiobutton28 = false;
                                this.deviceParameters.radiobutton27 = false;
                                this.deviceParameters.radiobutton26 = true;
                                this.deviceParameters.radiobutton25 = false;
                            }
                            else
                            {
                                this.deviceParameters.radiobutton28 = false;
                                this.deviceParameters.radiobutton27 = false;
                                this.deviceParameters.radiobutton26 = false;
                                this.deviceParameters.radiobutton25 = true;
                            }
                            this.deviceParameters.textbox137 = this.parameters.Load3WorkTime1H.ToString("F0");
                            this.deviceParameters.textbox134 = this.parameters.Load3WorkTime1M.ToString("F0");
                            if (this.parameters.Load3Power1 == 1)
                            {
                                this.deviceParameters.button26 = false;
                                this.deviceParameters.button25 = true;
                            }
                            else
                            {
                                this.deviceParameters.button25 = false;
                                this.deviceParameters.button26 = true;
                            }
                            this.deviceParameters.textbox131 = this.parameters.Load3WorkTime2H.ToString("F0");
                            this.deviceParameters.textbox128 = this.parameters.Load3WorkTime2M.ToString("F0");
                            if (this.parameters.Load3Power2 == 1)
                            {
                                this.deviceParameters.button24 = false;
                                this.deviceParameters.button23 = true;
                            }
                            else
                            {
                                this.deviceParameters.button23 = false;
                                this.deviceParameters.button24 = true;
                            }
                            this.deviceParameters.textbox143 = this.parameters.Load3WorkTime3H.ToString("F0");
                            this.deviceParameters.textbox145 = this.parameters.Load3WorkTime3M.ToString("F0");
                            if (this.parameters.Load3Power3 == 1)
                            {
                                this.deviceParameters.button22 = false;
                                this.deviceParameters.button21 = true;
                            }
                            else
                            {
                                this.deviceParameters.button21 = false;
                                this.deviceParameters.button22 = true;
                            }
                            this.deviceParameters.textbox148 = this.parameters.Load3WorkTime4H.ToString("F0");
                            this.deviceParameters.textbox150 = this.parameters.Load3WorkTime4M.ToString("F0");
                            if (this.parameters.Load3Power4 == 1)
                            {
                                this.deviceParameters.button20 = false;
                                this.deviceParameters.button19 = true;
                            }
                            else
                            {
                                this.deviceParameters.button19 = false;
                                this.deviceParameters.button20 = true;
                            }
                            this.deviceParameters.textbox153 = this.parameters.Load3OnTimeH1.ToString("F0");
                            this.deviceParameters.textbox155 = this.parameters.Load3OnTimeM1.ToString("F0");
                            this.deviceParameters.textbox156 = this.parameters.Load3OnTimeS1.ToString("F0");
                            this.deviceParameters.textbox154 = this.parameters.Load3OffTimeH1.ToString("F0");
                            this.deviceParameters.textbox152 = this.parameters.Load3OffTimeM1.ToString("F0");
                            this.deviceParameters.textbox151 = this.parameters.Load3OffTimeS1.ToString("F0");
                            this.deviceParameters.textbox149 = this.parameters.Load3OnTimeH2.ToString("F0");
                            this.deviceParameters.textbox147 = this.parameters.Load3OnTimeM2.ToString("F0");
                            this.deviceParameters.textbox146 = this.parameters.Load3OnTimeS2.ToString("F0");
                            this.deviceParameters.textbox144 = this.parameters.Load3OffTimeH2.ToString("F0");
                            this.deviceParameters.textbox142 = this.parameters.Load3OffTimeM2.ToString("F0");
                            this.deviceParameters.textbox141 = this.parameters.Load3OffTimeS2.ToString("F0");
                            if (((int)this.parameters.XintControl & -1) != 0)
                            {
                                this.deviceParameters.button36 = false;
                                this.deviceParameters.button35 = true;
                            }
                            else
                            {
                                this.deviceParameters.button35 = false;
                                this.deviceParameters.button36 = true;
                            }
                            if (((int)this.parameters.XintControl & -1) != 0)
                            {
                                this.deviceParameters.button40 = false;
                                this.deviceParameters.button39 = true;
                            }
                            else
                            {
                                this.deviceParameters.button39 = false;
                                this.deviceParameters.button40 = true;
                            }
                            if (((int)this.parameters.XintControl & -1) != 0)
                            {
                                this.deviceParameters.button38 = false;
                                this.deviceParameters.button37 = true;
                            }
                            else
                            {
                                this.deviceParameters.button37 = false;
                                this.deviceParameters.button38 = true;
                            }
                            if (((int)this.parameters.XintControl & -1) != 0)
                            {
                                this.deviceParameters.button42 = false;
                                this.deviceParameters.button41 = true;
                            }
                            else
                            {
                                this.deviceParameters.button41 = false;
                                this.deviceParameters.button42 = true;
                            }
                            this.deviceParameters.textbox53 = this.parameters.XintDelay.ToString("F0");
                            this.deviceParameters.textbox15 = this.parameters.XintPower1.ToString("F0");
                            this.deviceParameters.textbox50 = this.parameters.XintPower2.ToString("F0");
                            this.deviceParameters.textbox51 = this.parameters.XintPower3.ToString("F0");
                            this.deviceParameters.textbox52 = this.parameters.XintPower4.ToString("F0");
                            if ((this.parameters.ManualControl & 1) > 0)
                            {
                                this.deviceParameters.button1 = false;
                                this.deviceParameters.button2 = true;
                            }
                            else
                            {
                                this.deviceParameters.button2 = false;
                                this.deviceParameters.button1 = true;
                            }
                            if ((this.parameters.ManualControl & 2) > 0)
                            {
                                this.deviceParameters.button4 = false;
                                this.deviceParameters.button3 = true;
                            }
                            else
                            {
                                this.deviceParameters.button3 = false;
                                this.deviceParameters.button4 = true;
                            }
                            if ((this.parameters.ManualControl & 4) > 0)
                            {
                                this.deviceParameters.button6 = false;
                                this.deviceParameters.button5 = true;
                            }
                            else
                            {
                                this.deviceParameters.button5 = false;
                                this.deviceParameters.button6 = true;
                            }
                            if ((this.parameters.ManualControl & 8) > 0)
                            {
                                this.deviceParameters.button8 = false;
                                this.deviceParameters.button7 = true;
                            }
                            else
                            {
                                this.deviceParameters.button7 = false;
                                this.deviceParameters.button8 = true;

                              
                            }
                            //this.Timer1.Enabled = false;
                            //this.Button9.Enabled = true;
                            //this.Button10.Enabled = true;
                            this.tickcount2 = 0;
                            this.totaltickcount = 0;
                            this.Alert = "Read OK , OK！";
                        }
                    }
                    else if (this.flag_Parawrite1 == 1)
                    {
                        this.flag_Parawrite1 = 2;
                        this.Sendcmd(36926, Module1.buscmd, 61);
                    }
                    else
                    {
                        //this.Timer1.Enabled = false;
                        //this.Button9.Enabled = true;
                        //this.Button10.Enabled = true;
                        this.tickcount2 = 0;
                        this.totaltickcount = 0;
                        this.Alert = "Write OK , OK！";
                    }
                }
                else
                {
                    this.tickcount2 += 1;
                    if (this.tickcount2 > 6)
                    {
                        this.tickcount2 = 0;
                        if (Module1.buscmd == 3)
                        {
                            if (this.flag_Pararead1 == 1)
                            {
                                this.Sendcmd(36864, Module1.buscmd, 61);
                            }
                            else
                            {
                                this.Sendcmd(36925, Module1.buscmd, 62);
                            }
                        }
                        else if (this.flag_Parawrite1 == 1)
                        {
                            this.Sendcmd(36864, Module1.buscmd, 62);
                        }
                        else
                        {
                            this.Sendcmd(36926, Module1.buscmd, 61);
                        }
                    }
                }
                if (this.totaltickcount > 21)
                {
                    //this.Timer1.Enabled = false;
                    //this.Button9.Enabled = true;
                    //this.Button10.Enabled = true;
                    this.totaltickcount = 0;
                    this.Alert = "Timeout , Error！";
                }
            }
        }

        private async Task<bool> SaveParameters()
        {
            if (Operators.CompareString(this.deviceParameters.textbox1, "", false) != 0)
            {
                float inputnum = Conversions.ToSingle(this.deviceParameters.textbox1);
                if (inputnum < 9f | inputnum > 34f)
                {
                    this.Alert = "HVD setup is incorrect，Please input again(9~34) , Error！";
                }
                else if (Operators.CompareString(this.deviceParameters.textbox2, "", false) != 0)
                {
                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox2);
                    if (inputnum < 9f | inputnum > 34f)
                    {
                        this.Alert = "Lion charge setup is incorrect，Please input again(9~34) , Error！";
                    }
                    else if (Operators.CompareString(this.deviceParameters.textbox3, "", false) != 0)
                    {
                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox3);
                        if (inputnum < 9f | inputnum > 34f)
                        {
                            this.Alert = "HVDR setup is incorrect，Please input again(9~34) , Error！";
                        }
                        else if (Operators.CompareString(this.deviceParameters.textbox4, "", false) != 0)
                        {
                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox4);
                            if (inputnum < 9f | inputnum > 34f)
                            {
                                this.Alert = "Balance setup is incorrect，Please input again(9~34) , Error！";
                            }
                            else if (Operators.CompareString(this.deviceParameters.textbox5, "", false) != 0)
                            {
                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox5);
                                if (inputnum < 9f | inputnum > 34f)
                                {
                                    this.Alert = "Boost setup is incorrect，Please input again(9~34) , Error！";
                                }
                                else if (Operators.CompareString(this.deviceParameters.textbox6, "", false) != 0)
                                {
                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox6);
                                    if (inputnum < 9f | inputnum > 34f)
                                    {
                                        this.Alert = "Float setup is incorrect，Please input again(9~34) , Error！";
                                    }
                                    else if (Operators.CompareString(this.deviceParameters.textbox7, "", false) != 0)
                                    {
                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox7);
                                        if (inputnum < 9f | inputnum > 34f)
                                        {
                                            this.Alert = "Boost recover setup is incorrect，Please input again(9~34) , Error！";
                                        }
                                        else if (Operators.CompareString(this.deviceParameters.textbox8, "", false) != 0)
                                        {
                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox8);
                                            if (inputnum < 9f | inputnum > 34f)
                                            {
                                                this.Alert = "LVDR is incorrect，Please input again(9~34) , Error！";
                                            }
                                            else if (Operators.CompareString(this.deviceParameters.textbox9, "", false) != 0)
                                            {
                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox9);
                                                if (inputnum < 9f | inputnum > 34f)
                                                {
                                                    this.Alert = "Voltage of ECOFF is incorrect，Please input again(9~34) , Error！";
                                                }
                                                else if (Operators.CompareString(this.deviceParameters.textbox10, "", false) != 0)
                                                {
                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox10);
                                                    if (inputnum < 9f | inputnum > 34f)
                                                    {
                                                        this.Alert = "Voltage of ECON is incorrect，Please input again(9~34) , Error！";
                                                    }
                                                    else if (Operators.CompareString(this.deviceParameters.textbox18, "", false) != 0)
                                                    {
                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox18);
                                                        if (inputnum < 30f | inputnum > 90f)
                                                        {
                                                            this.Alert = "Max. Temp. of Bat is incorrect，Please input again(30~90) , Error！";
                                                        }
                                                        else if (Operators.CompareString(this.deviceParameters.textbox20, "", false) != 0)
                                                        {
                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox20);
                                                            if (inputnum < 30f | inputnum > 90f)
                                                            {
                                                                this.Alert = "Max. Temp. of Controller is incorrect，Please input again(30~90) , Error！";
                                                            }
                                                            else if (Operators.CompareString(this.deviceParameters.textbox21, "", false) != 0)
                                                            {
                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox21);
                                                                if (inputnum < 30f | inputnum > 90f)
                                                                {
                                                                    this.Alert = "Work again Temp. of Controller is incorrect，Please input again(30~90) , Error！";
                                                                }
                                                                else if (Operators.CompareString(this.deviceParameters.textbox22, "", false) != 0)
                                                                {
                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox22);
                                                                    if (inputnum < 3f | inputnum > 10f)
                                                                    {
                                                                        this.Alert = "Night Voltage is incorrect，Please input again(3~10) , Error！";
                                                                    }
                                                                    else if (Operators.CompareString(this.deviceParameters.textbox25, "", false) != 0)
                                                                    {
                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox25);
                                                                        if (inputnum < 12f | inputnum > 50f)
                                                                        {
                                                                            this.Alert = "Day Voltage is incorrect，Please input again(12~50) , Error！";
                                                                        }
                                                                        else if (Operators.CompareString(this.deviceParameters.textbox23, "", false) != 0)
                                                                        {
                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox23);
                                                                            if (inputnum < 1f | inputnum > 10f)
                                                                            {
                                                                                this.Alert = "Night Delay is incorrect，Please input again(1~10) , Error！";
                                                                            }
                                                                            else if (Operators.CompareString(this.deviceParameters.textbox24, "", false) != 0)
                                                                            {
                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox24);
                                                                                if (inputnum < 1f | inputnum > 10f)
                                                                                {
                                                                                    this.Alert = "Day Delay is incorrect，Please input again(1~10) , Error！";
                                                                                }
                                                                                else if (Operators.CompareString(this.deviceParameters.textbox26, "", false) != 0)
                                                                                {
                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox26);
                                                                                    if (inputnum < 0f | inputnum > 12f)
                                                                                    {
                                                                                        this.Alert = "LED Phase1 is incorrect，Please input again(0~12) , Error！";
                                                                                    }
                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox27, "", false) != 0)
                                                                                    {
                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox27);
                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                        {
                                                                                            this.Alert = "LED Phase1 is incorrect，Please input again(0~59) , Error！";
                                                                                        }
                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox31, "", false) != 0)
                                                                                        {
                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox31);
                                                                                            if (inputnum < 0f | inputnum > 12f)
                                                                                            {
                                                                                                this.Alert = "LED Phase2 is incorrect，Please input again(0~12) , Error！";
                                                                                            }
                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox30, "", false) != 0)
                                                                                            {
                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox30);
                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                {
                                                                                                    this.Alert = "LED Phase2 is incorrect，Please input again(0~59) , Error！";
                                                                                                }
                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox34, "", false) != 0)
                                                                                                {
                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox34);
                                                                                                    if (inputnum < 0f | inputnum > 12f)
                                                                                                    {
                                                                                                        this.Alert = "LED Phase3 is incorrect，Please input again(0~12) , Error！";
                                                                                                    }
                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox33, "", false) != 0)
                                                                                                    {
                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox33);
                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                        {
                                                                                                            this.Alert = "LED Phase3 is incorrect，Please input again(0~59) , Error！";
                                                                                                        }
                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox37, "", false) != 0)
                                                                                                        {
                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox37);
                                                                                                            if (inputnum < 0f | inputnum > 12f)
                                                                                                            {
                                                                                                                this.Alert = "LED Phase4 is incorrect，Please input again(0~12) , Error！";
                                                                                                            }
                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox36, "", false) != 0)
                                                                                                            {
                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox36);
                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                {
                                                                                                                    this.Alert = "LED Phase4 is incorrect，Please input again(0~59) , Error！";
                                                                                                                }
                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox28, "", false) != 0)
                                                                                                                {
                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox28);
                                                                                                                    if (inputnum < 0f | inputnum > 3300f)
                                                                                                                    {
                                                                                                                        this.Alert = "LED Phase1 currect is incorrect，Please input again(0~3300) , Error！";
                                                                                                                    }
                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox29, "", false) != 0)
                                                                                                                    {
                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox29);
                                                                                                                        if (inputnum < 0f | inputnum > 3300f)
                                                                                                                        {
                                                                                                                            this.Alert = "LED Phase2 currect is incorrect，Please input again(0~3300) , Error！";
                                                                                                                        }
                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox32, "", false) != 0)
                                                                                                                        {
                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox32);
                                                                                                                            if (inputnum < 0f | inputnum > 3300f)
                                                                                                                            {
                                                                                                                                this.Alert = "LED Phase3 currect is incorrect，Please input again(0~3300) , Error！";
                                                                                                                            }
                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox35, "", false) != 0)
                                                                                                                            {
                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox35);
                                                                                                                                if (inputnum < 0f | inputnum > 3300f)
                                                                                                                                {
                                                                                                                                    this.Alert = "LED Phase4 currect is incorrect，Please input again(0~3300) , Error！";
                                                                                                                                }
                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox38, "", false) != 0)
                                                                                                                                {
                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox38);
                                                                                                                                    if (inputnum < 0f | inputnum > 23f)
                                                                                                                                    {
                                                                                                                                        this.Alert = "LED Start Clock1 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                    }
                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox39, "", false) != 0)
                                                                                                                                    {
                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox39);
                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                        {
                                                                                                                                            this.Alert = "LED Start Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                        }
                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox40, "", false) != 0)
                                                                                                                                        {
                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox40);
                                                                                                                                            if (inputnum < 0f | inputnum > 59f)
                                                                                                                                            {
                                                                                                                                                this.Alert = "LED Start Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                            }
                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox43, "", false) != 0)
                                                                                                                                            {
                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox43);
                                                                                                                                                if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                {
                                                                                                                                                    this.Alert = "LED Stop Clock1 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                }
                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox42, "", false) != 0)
                                                                                                                                                {
                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox42);
                                                                                                                                                    if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                    {
                                                                                                                                                        this.Alert = "LED Stop Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                    }
                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox41, "", false) != 0)
                                                                                                                                                    {
                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox41);
                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                        {
                                                                                                                                                            this.Alert = "LED Stop Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                        }
                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox49, "", false) != 0)
                                                                                                                                                        {
                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox49);
                                                                                                                                                            if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                            {
                                                                                                                                                                this.Alert = "LED Start Clock2 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                            }
                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox48, "", false) != 0)
                                                                                                                                                            {
                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox48);
                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                {
                                                                                                                                                                    this.Alert = "LED Start Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                }
                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox47, "", false) != 0)
                                                                                                                                                                {
                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox47);
                                                                                                                                                                    if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                    {
                                                                                                                                                                        this.Alert = "LED Start Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                    }
                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox46, "", false) != 0)
                                                                                                                                                                    {
                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox46);
                                                                                                                                                                        if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                        {
                                                                                                                                                                            this.Alert = "LED Stop Clock2 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                        }
                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox45, "", false) != 0)
                                                                                                                                                                        {
                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox45);
                                                                                                                                                                            if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                            {
                                                                                                                                                                                this.Alert = "LED Stop Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                            }
                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox44, "", false) != 0)
                                                                                                                                                                            {
                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox44);
                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                {
                                                                                                                                                                                    this.Alert = "LED Stop Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                }
                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox75, "", false) != 0)
                                                                                                                                                                                {
                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox75);
                                                                                                                                                                                    if (inputnum < 0f | inputnum > 12f)
                                                                                                                                                                                    {
                                                                                                                                                                                        this.Alert = "Load1 Phase1 is incorrect，Please input again(0~12) , Error！";
                                                                                                                                                                                    }
                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox72, "", false) != 0)
                                                                                                                                                                                    {
                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox72);
                                                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                        {
                                                                                                                                                                                            this.Alert = "Load1 Phase1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                        }
                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox68, "", false) != 0)
                                                                                                                                                                                        {
                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox68);
                                                                                                                                                                                            if (inputnum < 0f | inputnum > 12f)
                                                                                                                                                                                            {
                                                                                                                                                                                                this.Alert = "Load1 Phase2 is incorrect，Please input again(0~12) , Error！";
                                                                                                                                                                                            }
                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox65, "", false) != 0)
                                                                                                                                                                                            {
                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox65);
                                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                {
                                                                                                                                                                                                    this.Alert = "Load1 Phase2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                }
                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox82, "", false) != 0)
                                                                                                                                                                                                {
                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox82);
                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 12f)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        this.Alert = "Load1 Phase3 is incorrect，Please input again(0~12) , Error！";
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox84, "", false) != 0)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox84);
                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            this.Alert = "Load1 Phase3 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox87, "", false) != 0)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox87);
                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 12f)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                this.Alert = "Load1 Phase4 is incorrect，Please input again(0~12) , Error！";
                                                                                                                                                                                                            }
                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox89, "", false) != 0)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox89);
                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    this.Alert = "Load1 Phase4 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                }
                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox93, "", false) != 0)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox93);
                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        this.Alert = "Load1 Start Clock1 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox95, "", false) != 0)
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox95);
                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            this.Alert = "Load1 Start Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox96, "", false) != 0)
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox96);
                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                this.Alert = "Load1 Start Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox94, "", false) != 0)
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox94);
                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    this.Alert = "Load1 Stop Clock1 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox92, "", false) != 0)
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox92);
                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        this.Alert = "Load1 Stop Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox91, "", false) != 0)
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox91);
                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            this.Alert = "Load1 Stop Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox88, "", false) != 0)
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox88);
                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                this.Alert = "Load1 Start Clock2 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox86, "", false) != 0)
                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox86);
                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                    this.Alert = "Load1 Start Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox85, "", false) != 0)
                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox85);
                                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                        this.Alert = "Load1 Start Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox83, "", false) != 0)
                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox83);
                                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                            this.Alert = "Load1 Stop Clock2 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox81, "", false) != 0)
                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox81);
                                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                this.Alert = "Load1 Stop Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox80, "", false) != 0)
                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox80);
                                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    this.Alert = "Load1 Stop Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox105, "", false) != 0)
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox105);
                                                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 12f)
                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                        this.Alert = "Load2 Phase1 is incorrect，Please input again(0~12) , Error！";
                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox102, "", false) != 0)
                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox102);
                                                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                            this.Alert = "Load2 Phase1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox99, "", false) != 0)
                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox99);
                                                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 12f)
                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                this.Alert = "Load2 Phase2 is incorrect，Please input again(0~12) , Error！";
                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox90, "", false) != 0)
                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox90);
                                                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                    this.Alert = "Load2 Phase2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox111, "", false) != 0)
                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox111);
                                                                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 12f)
                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                        this.Alert = "Load2 Phase3 is incorrect，Please input again(0~12) , Error！";
                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox113, "", false) != 0)
                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox113);
                                                                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                            this.Alert = "Load2 Phase3 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox116, "", false) != 0)
                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox116);
                                                                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 12f)
                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                this.Alert = "Load2 Phase4 is incorrect，Please input again(0~12) , Error！";
                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox118, "", false) != 0)
                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox118);
                                                                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                    this.Alert = "Load2 Phase4 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox121, "", false) != 0)
                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox121);
                                                                                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                        this.Alert = "Load2 Start Clock1 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox123, "", false) != 0)
                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox123);
                                                                                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                            this.Alert = "Load2 Start Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox124, "", false) != 0)
                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox124);
                                                                                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                this.Alert = "Load2 Start Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox122, "", false) != 0)
                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox122);
                                                                                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                    this.Alert = "Load2 Stop Clock1 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox120, "", false) != 0)
                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox120);
                                                                                                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                        this.Alert = "Load2 Stop Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox119, "", false) != 0)
                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox119);
                                                                                                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                            this.Alert = "Load2 Stop Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox117, "", false) != 0)
                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox117);
                                                                                                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                this.Alert = "Load2 Start Clock2 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox115, "", false) != 0)
                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox115);
                                                                                                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                    this.Alert = "Load2 Start Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox114, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox114);
                                                                                                                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                        this.Alert = "Load2 Start Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox112, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox112);
                                                                                                                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                            this.Alert = "Load2 Stop Clock2 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox110, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox110);
                                                                                                                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                this.Alert = "Load2 Stop Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox109, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox109);
                                                                                                                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                    this.Alert = "Load2 Stop Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox137, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox137);
                                                                                                                                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 12f)
                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                        this.Alert = "Load3 Phase1 is incorrect，Please input again(0~12) , Error！";
                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox134, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox134);
                                                                                                                                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                            this.Alert = "Load3 Phase1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox131, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox131);
                                                                                                                                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 12f)
                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                this.Alert = "Load3 Phase2 is incorrect，Please input again(0~12) , Error！";
                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox128, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox128);
                                                                                                                                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                    this.Alert = "Load3 Phase2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox143, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox143);
                                                                                                                                                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 12f)
                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                        this.Alert = "Load3 Phase3 is incorrect，Please input again(0~12) , Error！";
                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox145, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox145);
                                                                                                                                                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                            this.Alert = "Load3 Phase3 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox148, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox148);
                                                                                                                                                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 12f)
                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                this.Alert = "Load3 Phase4 is incorrect，Please input again(0~12) , Error！";
                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox150, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox150);
                                                                                                                                                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                    this.Alert = "Load3 Phase4 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox153, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox153);
                                                                                                                                                                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                        this.Alert = "Load3 Start Clock1 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox155, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox155);
                                                                                                                                                                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                                            this.Alert = "Load3 Start Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox156, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox156);
                                                                                                                                                                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                                this.Alert = "Load3 Start Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox154, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox154);
                                                                                                                                                                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                                    this.Alert = "Load3 Stop Clock1 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox152, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox152);
                                                                                                                                                                                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                                        this.Alert = "Load3 Stop Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox151, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox151);
                                                                                                                                                                                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                                                            this.Alert = "Load3 Stop Clock1 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox149, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox149);
                                                                                                                                                                                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                                                this.Alert = "Load3 Start Clock2 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox147, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox147);
                                                                                                                                                                                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                                                    this.Alert = "Load3 Start Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox146, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox146);
                                                                                                                                                                                                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                                                        this.Alert = "Load3 Start Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox144, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox144);
                                                                                                                                                                                                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 23f)
                                                                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                                                                            this.Alert = "Load3 Stop Clock2 is incorrect，Please input again(0~23) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox142, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox142);
                                                                                                                                                                                                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                                                                this.Alert = "Load3 Stop Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox141, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox141);
                                                                                                                                                                                                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 59f)
                                                                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                                                                    this.Alert = "Load3 Stop Clock2 is incorrect，Please input again(0~59) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox53, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox53);
                                                                                                                                                                                                                                                                                                                                                                                                                                    if (inputnum < 1f | inputnum > 10f)
                                                                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                                                                        this.Alert = "External Control Delay is incorrect，Please input again(1~10) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                                                                    else if (Operators.CompareString(this.deviceParameters.textbox15, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                                                                        inputnum = Conversions.ToSingle(this.deviceParameters.textbox15);
                                                                                                                                                                                                                                                                                                                                                                                                                                        if (inputnum < 0f | inputnum > 3300f)
                                                                                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                                                                                            this.Alert = "External Control Phase1 current is incorrect，Please input again(0~3300) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                                                                                        else if (Operators.CompareString(this.deviceParameters.textbox50, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                                                                                            inputnum = Conversions.ToSingle(this.deviceParameters.textbox50);
                                                                                                                                                                                                                                                                                                                                                                                                                                            if (inputnum < 0f | inputnum > 3300f)
                                                                                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                                                                                this.Alert = "External Control Phase2 current is incorrect，Please input again(0~3300) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                                                                                            else if (Operators.CompareString(this.deviceParameters.textbox51, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                                                                                inputnum = Conversions.ToSingle(this.deviceParameters.textbox51);
                                                                                                                                                                                                                                                                                                                                                                                                                                                if (inputnum < 0f | inputnum > 3300f)
                                                                                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                                                                                    this.Alert = "External Control Phase3 current is incorrect，Please input again(0~3300) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                                                                                else if (Operators.CompareString(this.deviceParameters.textbox52, "", false) != 0)
                                                                                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                                                                                    inputnum = Conversions.ToSingle(this.deviceParameters.textbox52);
                                                                                                                                                                                                                                                                                                                                                                                                                                                    if (inputnum < 0f | inputnum > 3300f)
                                                                                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                                                                                        this.Alert = "External Control Phase4 current is incorrect，Please input again(0~3300) , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                                                                                        Module1.buscmd = 16;
                                                                                                                                                                                                                                                                                                                                                                                                                                                        await this.Sendcmd(36864, 16, 62);
                                                                                                                                                                                                                                                                                                                                                                                                                                                        this.flag_Parawrite1 = 1;
                                                                                                                                                                                                                                                                                                                                                                                                                                                        //Thread.Sleep(1500);
                                                                                                                                                                                                                                                                                                                                                                                                                                                        this.flag_Parawrite1 = 2;
                                                                                                                                                                                                                                                                                                                                                                                                                                                        await this.Sendcmd(36926, 16, 61);
                                                                                                                                                                                                                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            else
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        else
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    else
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                }
                                                                                                                                                                                                                else
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                                }
                                                                                                                                                                                                            }
                                                                                                                                                                                                            else
                                                                                                                                                                                                            {
                                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else
                                                                                                                                                                                                        {
                                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else
                                                                                                                                                                                                    {
                                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                        else
                                                                                                                                                                                        {
                                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                        else
                                                                                                                                                                        {
                                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                    {
                                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                {
                                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
                                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                                }
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                                        }
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                                    }
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                this.Alert = "The data can't be empty , Error！";
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            this.Alert = "The data can't be empty , Error！";
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        this.Alert = "The data can't be empty , Error！";
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    this.Alert = "The data can't be empty , Error！";
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                this.Alert = "The data can't be empty , Error！";
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            this.Alert = "The data can't be empty , Error！";
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        this.Alert = "The data can't be empty , Error！";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    this.Alert = "The data can't be empty , Error！";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                this.Alert = "The data can't be empty , Error！";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            this.Alert = "The data can't be empty , Error！";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        this.Alert = "The data can't be empty , Error！";
                                                    }
                                                }
                                                else
                                                {
                                                    this.Alert = "The data can't be empty , Error！";
                                                }
                                            }
                                            else
                                            {
                                                this.Alert = "The data can't be empty , Error！";
                                            }
                                        }
                                        else
                                        {
                                            this.Alert = "The data can't be empty , Error！";
                                        }
                                    }
                                    else
                                    {
                                        this.Alert = "The data can't be empty , Error！";
                                    }
                                }
                                else
                                {
                                    this.Alert = "The data can't be empty , Error！";
                                }
                            }
                            else
                            {
                                this.Alert = "The data can't be empty , Error！";
                            }
                        }
                        else
                        {
                            this.Alert = "The data can't be empty , Error！";
                        }
                    }
                    else
                    {
                        this.Alert = "The data can't be empty , Error！";
                    }
                }
                else
                {
                    this.Alert = "The data can't be empty , Error！";
                }
            }
            else
            {
                this.Alert = "The data can't be empty , Error！";
            }
            return true;
        }

    }
}