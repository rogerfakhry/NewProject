using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SolarBatteryController.Logic
{
   public class Devices
    {
        private SqlCommand cmd;
        private SqlConnection cnn;


        public void InsertDevice(String deviceId)
        {
            
            cnn = new SqlConnection();
            cnn.ConnectionString = Properties.Settings.Default.DBConnectionString;
            cnn.Open();
            cmd = new SqlCommand();

            cmd.Connection = cnn;
            string command = @"INSERT INTO [dbo].[Devices]
           ([DeviceName]
           ,[Longitude]
           ,[Latitude]
           ,[deviceId]
           ,[serviceId]
           ,[isActive]
           ,[customerId]
           ,[isMonitored]
           ,[SlaveId])
            VALUES
           ({0},12,12,{1},1,0,4,1,{2})";

            cmd.CommandText = string.Format(command, deviceId, deviceId, deviceId);
            cmd.CommandType = System.Data.CommandType.Text;
            try
            {
                cmd.ExecuteNonQuery();

                cnn.Close();

                cmd.Dispose();
                cnn.Dispose();
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }
    }
}
