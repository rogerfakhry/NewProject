using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace SolarBatteryController.Logic
{
    class clsService
    {
        public int id;
        public string serviceName;
        public string comPort;
        public List<clsService> listServices;

        private SqlConnection cnn;
        private SqlCommand cmd;
        private SqlDataReader dr;
        private clsService service;
        public void GetServiceSettings()
        {
            this.listServices = new List<clsService>();
          
            cnn = new SqlConnection(Properties.Settings.Default.DBConnectionString);
            cnn.Open();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "uspGetServiceSettings";
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = Properties.Settings.Default.RS485ServiceId;
            dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    service = new clsService();
                    service.id = (int)dr["id"];
                    service.serviceName = (string)dr["ServiceName"];
                    service.comPort = (string)dr["comPort"];
                    listServices.Add(service);
                
                }//read the datareader
            }//end if condition
            cnn.Close(); //close the connection to database
            dr.Close();//close the datareader

            cmd.Dispose();
            cnn.Dispose();
        }

        public bool changeComPort(string ComPort,string ServiceId)
        {

            cnn = new SqlConnection();
            cnn.ConnectionString = Properties.Settings.Default.DBConnectionString;
            cnn.Open();
            cmd = new SqlCommand();

            cmd.Connection = cnn;
            string command = @"UPDATE [dbo].[Service]
            SET [comPort] = '{0}' 
            where [id] = {1} ";

            cmd.CommandText = string.Format(command, ComPort, ServiceId);
            cmd.CommandType = System.Data.CommandType.Text;
            try
            {
                cmd.ExecuteNonQuery();

                cnn.Close();

                cmd.Dispose();
                cnn.Dispose();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
                return false;
            }
           
        }
    }//end class clsService
}
