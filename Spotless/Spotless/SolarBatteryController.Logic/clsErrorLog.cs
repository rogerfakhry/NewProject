using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SolarBatteryController.Logic
{
    class clsErrorLog
    {
       public string errorDescription;
       public string errorType;
        public byte slaveId;
        SqlConnection cnn;
        SqlCommand cmd;

        public void LogError()
        {
            cnn = new SqlConnection();
            cmd = new SqlCommand();

            cnn.ConnectionString = Properties.Settings.Default.DBConnectionString;
            cnn.Open();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "uspInsertLog";
            cmd.Parameters.Add("@errorType", SqlDbType.NVarChar).Value = this.errorType;
            cmd.Parameters.Add("@errorDescription", SqlDbType.NVarChar).Value = this.errorDescription;
            cmd.Parameters.Add("@slaveId", SqlDbType.Int).Value = this.slaveId;
            cmd.ExecuteNonQuery();

            cnn.Close();
            cnn.Dispose();
            cmd.Dispose();


        }
    }
}
