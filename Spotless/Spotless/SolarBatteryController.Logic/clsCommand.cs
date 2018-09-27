using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace SolarBatteryController.Logic
{
    class clsCommand
    {
        public int id;
        public string command;
        public string users;
        public string slaveId;
        public ushort commandAddress = 100;
        public bool commandStatus;
        public string commandText = "";

        private SqlConnection cnn;
        private SqlCommand cmd;
        private SqlDataReader dr;


        public List<clsCommand> listCommandsMonitor;
        public List<clsCommand> listCommandsExecute;
        public int listCommandMonitorCount = 0;
        public int listCommandExecuteCount = 0;

        public void GetUserCommandsMonitor()
        {

            listCommandsMonitor = new List<clsCommand>();
            cmd = new SqlCommand();

            cnn = new SqlConnection(Properties.Settings.Default.DBConnectionString);
            cnn.Open();



            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "uspGetUserCommandMonitor";
            dr = cmd.ExecuteReader();


            clsCommand command = new clsCommand();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    command.command = (string)dr["Command"];
                    command.users = (string)dr["userId"];
                    command.slaveId = (string)dr["slaveId"];
                    command.id = int.Parse(dr["id"].ToString());
                    this.listCommandsMonitor.Add(command);
                    command = new clsCommand();

                }//end while
                this.listCommandMonitorCount = this.listCommandsMonitor.Count;
            }//end if
            else
            {
                this.listCommandMonitorCount = 0;
            }
            cnn.Close(); //close the connection
            dr.Close(); //close the data reader

            cmd.Dispose();
            cnn.Dispose();

        }// end function GetUsercommands



        public void GetuserCommandsExecute()
        {


            cmd = new SqlCommand();

            cnn = new SqlConnection(Properties.Settings.Default.DBConnectionString);
            cnn.Open();



            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "uspGetUserCommandsExecute";
            dr = cmd.ExecuteReader();


            clsCommand cmdRS485 = new clsCommand();
            this.listCommandsExecute = new List<clsCommand>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    cmdRS485 = new clsCommand();
                    cmdRS485.command = dr["Command"].ToString();
                    cmdRS485.users = dr["userId"].ToString();
                    cmdRS485.slaveId = dr["slaveId"].ToString();
                    cmdRS485.id = int.Parse(dr["id"].ToString());
                    ParseCommand(cmdRS485);
                    this.listCommandsExecute.Add(cmdRS485);
                }//end while
                this.listCommandExecuteCount = this.listCommandsExecute.Count;
            }//end if
            else
            {
                this.listCommandExecuteCount = 0;
            }

            cnn.Close(); //close the connection
            dr.Close(); //close the data reader

            cmd.Dispose();
            cnn.Dispose();

        }// end function GetuserCommandsExecute


        public void DeleteCommand()
        {
            cmd = new SqlCommand();

            cnn = new SqlConnection(Properties.Settings.Default.DBConnectionString);
            cnn.Open();



            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "uspDeleteCommand";
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = this.id;
            cmd.ExecuteNonQuery();

            cnn.Close();
            cmd.Dispose();
            cnn.Dispose();


        }//end function delete command


        public void UpdateRealTimeMonitor()
        {
            cmd = new SqlCommand();
            cnn = new SqlConnection(Properties.Settings.Default.DBConnectionString);
            cnn.Open();

            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "uspUpdateRealTimeMonitor";

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = this.id;
            cmd.Parameters.Add("@userId", SqlDbType.NVarChar).Value = this.users;
            cmd.Parameters.Add("@command", SqlDbType.NVarChar).Value = this.command;
            cmd.Parameters.Add("@slaveId", SqlDbType.NVarChar).Value = this.commandText;
            cmd.ExecuteNonQuery();

            cnn.Close();
            cnn.Dispose();
            cmd.Dispose();
            
        }
        private void ParseCommand(clsCommand cmd)
        {
            if (cmd.command.ToLower() == Properties.Settings.Default.TurnOnCommandDescription )
            {
                cmd.commandAddress = Properties.Settings.Default.RS485ManualControlLoadAddress;
                cmd.commandStatus = true;

            }

            if (cmd.command.ToLower() == Properties.Settings.Default.TurnOffCommandDescription)
            {
                cmd.commandAddress = Properties.Settings.Default.RS485ManualControlLoadAddress;
                cmd.commandStatus = false;

            }

        }//end private parse command
    }
}
