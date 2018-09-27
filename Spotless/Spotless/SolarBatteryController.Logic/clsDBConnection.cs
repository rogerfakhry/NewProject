using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace SolarBatteryController.Logic
{
    class clsDBConnection
    {
        public string sqlConnectionString = Properties.Settings.Default.DBConnectionString;
        private SqlConnection dbConn;

        public void OpenDBConnection()
        {
            dbConn = new SqlConnection(sqlConnectionString);
           if(dbConn.State == ConnectionState.Closed)
            {
                dbConn.ConnectionString = sqlConnectionString;
                dbConn.Open();
            }
        } //end open db connection

        public SqlConnection GetConnection()
        {
            return dbConn;
        }
        public void CloseDBConnection()
        {
            if(dbConn.State == ConnectionState.Open)
            {
                dbConn.Close();
            }
        }//end close db connection
      
    }
}
