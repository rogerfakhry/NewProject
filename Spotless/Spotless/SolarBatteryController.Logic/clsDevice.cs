using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace SolarBatteryController.Logic
{
    class clsDevice
    {
        public int id; 
        public byte slaveId;

        public decimal PVVoltage;
        public decimal PVCurrent;
        public decimal PVPowerL;
        public decimal PVPowerH;
        public decimal BAVoltage;
        public decimal BACurrent;
        public decimal Reserve;
        public decimal Reserve1;
        public decimal ACVoltage;
        public decimal ACCurrent;
        public decimal ACPowerL;
        public decimal ACPowerH;
        public decimal LEDLoadVoltage;
        public decimal LEDLoadCurrent;
        public decimal LEDLoadPowerL;
        public decimal LEDLoadPowerH;
        public decimal Load1Voltage;
        public decimal Load1Current;
        public decimal Load1PowerL;
        public decimal Load1PowerH;
        public decimal Load2Voltage;
        public decimal Load2Current;
        public decimal Load2PowerL;
        public decimal Load2PowerH;
        public decimal Load3Voltage;
        public decimal Load3Current;
        public decimal Load3PowerL;
        public decimal Load3PowerH;
        public decimal BatteryTemp;
        public decimal ControllerTemp;
        public decimal Reserve2;
        public decimal BatterySOC;
        public decimal MaximumInputVoltageOfPVOfDay;
        public decimal MinimumInputVoltageOfPVOfDay;
        public decimal MaximumBatteryVoltageOfDay;
        public decimal MinimumBatteryVoltageOfDay;
        public decimal TotalPowerConsumptionOfDayL;
        public decimal TotalPowerConsumptionOfDayH;
        public decimal TotalPowerConsumptionOfMonthL;
        public decimal TotalPowerConsumptionOfMonthH;
        public decimal TotalPowerConsumptionOfYearL;
        public decimal TotalPowerConsumptionOfYearH;
        public decimal TotalPowerConsumptionL;
        public decimal TotalPowerConsumptionH;
        public decimal TotalPowerGenerationOfDayL;
        public decimal TotalPowerGenerationOfDayH;
        public decimal TotalPowerGenerationOfMonthL;
        public decimal TotalPowerGenerationOfMonthH;
        public decimal TotalPowerGenerationOfYearL;
        public decimal TotalPowerGenerationOfYearH;
        public decimal TotalPowerGenerationL;
        public decimal TotalPowerGenerationH;
        public decimal BatteryState;
        public decimal ChargeState;
        public decimal DischargeState;
        public bool isMonitored = false;

        public List<clsDevice> listAllDevicesMonitor;
       

        private SqlCommand cmd;
        private SqlConnection cnn;
       
        public void InsertHistory()
        {
            cnn = new SqlConnection();
            cnn.ConnectionString = Properties.Settings.Default.DBConnectionString;
            cnn.Open();
            cmd = new SqlCommand();

            cmd.Connection = cnn;
            cmd.CommandText = "uspInsertDeviceHistory";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;           
            cmd.Parameters.Add("@slaveId", SqlDbType.Int).Value = this.slaveId;
            cmd.Parameters.Add("@deviceId", SqlDbType.Int).Value = this.id;
            cmd.Parameters.Add("@PVVoltage", SqlDbType.Decimal).Value = this.PVVoltage;
            cmd.Parameters.Add("@PVCurrent", SqlDbType.Decimal).Value = this.PVCurrent;
            cmd.Parameters.Add("@PVPowerL", SqlDbType.Decimal).Value = this.PVPowerL;
            cmd.Parameters.Add("@PVPowerH", SqlDbType.Decimal).Value = this.PVPowerH;
            cmd.Parameters.Add("@BAVoltage", SqlDbType.Decimal).Value = this.BAVoltage;
            cmd.Parameters.Add("@BACurrent", SqlDbType.Decimal).Value = this.BACurrent;
            cmd.Parameters.Add("@Reserve", SqlDbType.Decimal).Value = this.Reserve;
            cmd.Parameters.Add("@Reserve1", SqlDbType.Decimal).Value = this.Reserve1;
            cmd.Parameters.Add("@ACVoltage", SqlDbType.Decimal).Value = this.ACVoltage;
            cmd.Parameters.Add("@ACCurrent", SqlDbType.Decimal).Value = this.ACCurrent;
            cmd.Parameters.Add("@ACPowerL", SqlDbType.Decimal).Value = this.ACPowerL;
            cmd.Parameters.Add("@ACPowerH", SqlDbType.Decimal).Value = this.ACPowerH;
            cmd.Parameters.Add("@LEDLoadVoltage", SqlDbType.Decimal).Value = this.LEDLoadVoltage;
            cmd.Parameters.Add("@LEDLoadCurrent", SqlDbType.Decimal).Value = this.LEDLoadCurrent;
            cmd.Parameters.Add("@LEDLoadPowerL", SqlDbType.Decimal).Value = this.LEDLoadPowerL;
            cmd.Parameters.Add("@LEDLoadPowerH", SqlDbType.Decimal).Value = this.LEDLoadPowerH;
            cmd.Parameters.Add("@Load1Voltage", SqlDbType.Decimal).Value = this.Load1Voltage;
            cmd.Parameters.Add("@Load1Current", SqlDbType.Decimal).Value = this.Load1Current;
            cmd.Parameters.Add("@Load1PowerL", SqlDbType.Decimal).Value = this.Load1PowerL;
            cmd.Parameters.Add("@Load1PowerH", SqlDbType.Decimal).Value = this.Load1PowerH;
            cmd.Parameters.Add("@Load2Voltage", SqlDbType.Decimal).Value = this.Load2Voltage;
            cmd.Parameters.Add("@Load2Current", SqlDbType.Decimal).Value = this.Load2Current;
            cmd.Parameters.Add("@Load2PowerL", SqlDbType.Decimal).Value = this.Load2PowerL;
            cmd.Parameters.Add("@Load2PowerH", SqlDbType.Decimal).Value = this.Load2PowerH;
            cmd.Parameters.Add("@Load3Voltage", SqlDbType.Decimal).Value = this.Load3Voltage;
            cmd.Parameters.Add("@Load3Current", SqlDbType.Decimal).Value = this.Load3Current;
            cmd.Parameters.Add("@Load3PowerL", SqlDbType.Decimal).Value = this.Load3PowerL;
            cmd.Parameters.Add("@Load3PowerH", SqlDbType.Decimal).Value = this.Load3PowerH;
            cmd.Parameters.Add("@BatteryTemp", SqlDbType.Decimal).Value = this.BatteryTemp;
            cmd.Parameters.Add("@ControllerTemp", SqlDbType.Decimal).Value = this.ControllerTemp;
            cmd.Parameters.Add("@Reserve2", SqlDbType.Decimal).Value = this.Reserve2;
            cmd.Parameters.Add("@BatterySOC", SqlDbType.Decimal).Value = this.BatterySOC;
            cmd.Parameters.Add("@MaximumInputVoltageOfPVOfDay", SqlDbType.Decimal).Value = this.MaximumInputVoltageOfPVOfDay;
            cmd.Parameters.Add("@MinimumInputVoltageOfPVOfDay", SqlDbType.Decimal).Value = this.MinimumInputVoltageOfPVOfDay;
            cmd.Parameters.Add("@MaximumBatteryVoltageOfDay", SqlDbType.Decimal).Value = this.MaximumBatteryVoltageOfDay;
            cmd.Parameters.Add("@MinimumBatteryVoltageOfDay", SqlDbType.Decimal).Value = this.MinimumBatteryVoltageOfDay;
            cmd.Parameters.Add("@TotalPowerConsumptionOfDayL", SqlDbType.Decimal).Value = this.TotalPowerConsumptionOfDayL;
            cmd.Parameters.Add("@TotalPowerConsumptionOfDayH", SqlDbType.Decimal).Value = this.TotalPowerConsumptionOfDayH;
            cmd.Parameters.Add("@TotalPowerConsumptionOfMonthL", SqlDbType.Decimal).Value = this.TotalPowerConsumptionOfMonthL;
            cmd.Parameters.Add("@TotalPowerConsumptionOfMonthH", SqlDbType.Decimal).Value = this.TotalPowerConsumptionOfMonthH;
            cmd.Parameters.Add("@TotalPowerConsumptionOfYearL", SqlDbType.Decimal).Value = this.TotalPowerConsumptionOfYearL;
            cmd.Parameters.Add("@TotalPowerConsumptionOfYearH", SqlDbType.Decimal).Value = this.TotalPowerConsumptionOfYearH;
            cmd.Parameters.Add("@TotalPowerConsumptionL", SqlDbType.Decimal).Value = this.TotalPowerConsumptionL;
            cmd.Parameters.Add("@TotalPowerConsumptionH", SqlDbType.Decimal).Value = this.TotalPowerConsumptionH;
            cmd.Parameters.Add("@TotalPowerGenerationOfDayL", SqlDbType.Decimal).Value = this.TotalPowerGenerationOfDayL;
            cmd.Parameters.Add("@TotalPowerGenerationOfDayH", SqlDbType.Decimal).Value = this.TotalPowerGenerationOfDayH;
            cmd.Parameters.Add("@TotalPowerGenerationOfMonthL", SqlDbType.Decimal).Value = this.TotalPowerGenerationOfMonthL;
            cmd.Parameters.Add("@TotalPowerGenerationOfMonthH", SqlDbType.Decimal).Value = this.TotalPowerGenerationOfMonthH;
            cmd.Parameters.Add("@TotalPowerGenerationOfYearL", SqlDbType.Decimal).Value = this.TotalPowerGenerationOfYearL;
            cmd.Parameters.Add("@TotalPowerGenerationOfYearH", SqlDbType.Decimal).Value = this.TotalPowerGenerationOfYearH;
            cmd.Parameters.Add("@TotalPowerGenerationL", SqlDbType.Decimal).Value = this.TotalPowerGenerationL;
            cmd.Parameters.Add("@TotalPowerGenerationH", SqlDbType.Decimal).Value = this.TotalPowerGenerationH;
            //cmd.Parameters.Add("@maximumPVVoltageToday", SqlDbType.Decimal).Value = this.maximumPVVoltageToday;
            //cmd.Parameters.Add("@minimumPVVoltageToday", SqlDbType.Decimal).Value = this.minimumPVVoltageToday;
           
            cmd.ExecuteNonQuery();

            cnn.Close();

            cmd.Dispose();
            cnn.Dispose();
        }//end function inserthistory

        public void GetAllDevices()
        {

            this.listAllDevicesMonitor = new List<clsDevice>();
            cnn = new SqlConnection();
            SqlDataReader dr;
            clsDevice device = new clsDevice();


            cnn.ConnectionString = Properties.Settings.Default.DBConnectionString;
            cnn.Open();
            cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = "uspGetAllDevices";
            dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while(dr.Read())
                {
                   
                    device.slaveId = byte.Parse(dr["slaveId"].ToString());
                    device.id = int.Parse(dr["id"].ToString());
                    this.listAllDevicesMonitor.Add(device);
                    device = new clsDevice();
                }
            }
           

            cnn.Close();
            dr.Close();

            cmd.Dispose();
            cnn.Dispose();
        } //end function get all devices

        public string GetBatteryStatusDescription(string batteryStatus)
        {
            string battery = "";

            switch (batteryStatus)
            {
                case "1":
                    battery = "Not Normal";
                    break;
                default:
                    battery = "Normal";
                    break;
            }

            return battery;

        }//end function get battery status description


        public string GetChargingEquipementStatus(string chargingStatus)
        {
            string charging = "";
            switch(chargingStatus)
            {
                case "1":
                    charging = "Not Charging";
                    break;
                default:
                    charging = "Charging";
                    break;
            }
            return charging;
        }//end function GetChargingEquipementStatus

        public string GetDischargingEquipementStatus(string dischargingStatus)
        {
            string discharing = "";

            switch (dischargingStatus)
            {
                case "1":
                    discharing = "Fault";
                    break;
                default:
                    discharing = "Normal";
                    break;
            }
            return discharing;
        }


        public string GetCSVDeviceHistory()
        {
            List<clsDevice> clsDevice = new List<SolarBatteryController.Logic.clsDevice>();
            cnn = new SqlConnection();
            SqlDataReader dr;


            cnn.ConnectionString = Properties.Settings.Default.DBConnectionString;
            cnn.Open();
            cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = "select * from dbo.DeviceHistory";
            dr = cmd.ExecuteReader();

            StringBuilder sb = new StringBuilder();
            var columnNames = Enumerable.Range(0, dr.FieldCount)
                        .Select(dr.GetName)
                        .ToList();

            sb.Append(string.Join(",", columnNames));
            sb.AppendLine();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(","))
                            value = "\"" + value + "\"";

                        sb.Append(value.Replace(Environment.NewLine, " ") + ",");
                    }
                    sb.Length--; // Remove the last comma
                    sb.AppendLine();
                }
            }

            var s = sb.ToString();
             
            cnn.Close();
            dr.Close();

            cmd.Dispose();
            cnn.Dispose();
            return s;

        } 

        



    }
}
