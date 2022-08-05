using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Auction_Manager
{
    class DBConnectMySQL
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private string port;

        //Constructor
        public DBConnectMySQL()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server =ConfigVariables.server;
            port = ConfigVariables.port;
            database = "ExampleName";                       //database name
            uid = "Username";                               //database user id 
            password = "Example Password";                  //database password 
            string connectionString;
            connectionString = "SERVER=" + server + ";"+ "PORT="+ port + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            connection = new MySqlConnection(connectionString);                     //connection string creation 
            
        }

        #region Connections

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {                
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        #endregion
                        
        public bool TestConnection()
        {            
            if (this.OpenConnection() == true)
            {                
                this.CloseConnection();
                return true;
            }
            else
            {
                return false;
            }
        }

        
        //retrieve User Info
        public List<string>[] SelectUser(bool admin,string userID)
        {
            //Create a list to store the result
            List<string>[] list = new List<string>[6];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();
            list[3] = new List<string>();
            list[4] = new List<string>();
            list[5] = new List<string>();


            string query;
            if (admin == true)
            {
                query = "SELECT * FROM admin WHERE AdminID= @username";
                //Open connection
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@username", userID);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        list[0].Add(dataReader["AdminID"] + "");
                        list[1].Add(dataReader["AdminName"] + "");
                        list[2].Add(dataReader["AdminSurname"] + "");
                        list[3].Add(dataReader["AdminIDnumber"] + "");
                        list[4].Add(dataReader["AdminPassword"] + "");
                    }

                    //close Data Reader
                    dataReader.Close();

                    //close Connection
                    this.CloseConnection();

                    //return list to be displayed
                    return list;
                }
                else
                {
                    return list;
                }

            }
            else
            {
                query = "SELECT * FROM employees WHERE EmployeeID= @username";
                //Open connection
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@username", userID);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        list[0].Add(dataReader["EmployeeID"] + "");
                        list[3].Add(dataReader["AdminID"] + "");
                        list[1].Add(dataReader["EmployeeName"] + "");
                        list[2].Add(dataReader["EmployeeSurname"] + "");                       
                        list[4].Add(dataReader["EmployeePassword"] + "");
                        list[5].Add(dataReader["EmployeeIDNumber"] + "");
                        
                    }

                    //close Data Reader
                    dataReader.Close();

                    //close Connection
                    this.CloseConnection();

                    //return list to be displayed
                    return list;
                }
                else
                {
                    return list;
                }

            }

           
        }

        #region Queries

        public DataTable GetEmployeeList()
        {
            string query = "SELECT * FROM employees";
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                MySqlDataReader dataReader = cmd.ExecuteReader();                   //Create a data reader and Execute the command


                if (dataReader.HasRows)
                {
                    dt.Load(dataReader);

                    dataReader.Close();
                    this.CloseConnection();
                    return dt;
                }
                else
                {
                    dataReader.Close();
                    this.CloseConnection();
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        //Insert statement
        public bool InsertEmployee(String empID, String adminId, String empName, String empSurn, String empIDnum, String empPass)
        {
            string query = "INSERT INTO employees (EmployeeID,AdminID,EmployeeName,EmployeeSurname,EmployeeIDNumber,EmployeePassword) VALUES(@empID,@adminID,@empName,@empSurn,@empIdnum,@empPass)";

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@empID", empID);
                cmd.Parameters.AddWithValue("@adminID", adminId);
                cmd.Parameters.AddWithValue("@empName", empName);
                cmd.Parameters.AddWithValue("@empSurn", empSurn);
                cmd.Parameters.AddWithValue("@empIdnum", empIDnum);
                cmd.Parameters.AddWithValue("@empPass", empPass);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
                return true;
            }
            else
            {
                return false;
            }
        }
        
        //Update statements
        public bool EmployeeIDCheck(string empID)
        {
            string query = "SELECT * FROM employees WHERE EmployeeID= @empID";
            //Open connection
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);             //Create Command
                cmd.Parameters.AddWithValue("@empID", empID);                     //InsertVariable                
                MySqlDataReader dataReader = cmd.ExecuteReader();                   //Create a data reader and Execute the command

                if (dataReader.HasRows)
                {
                    dataReader.Close();
                    this.CloseConnection();
                    return true;
                }
                else
                {
                    dataReader.Close();
                    this.CloseConnection();
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
               
        public void UpdateEmpName(string empName, string empID)
        {
            string query = "UPDATE employees SET EmployeeName=@empName WHERE EmployeeID=@empID";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@empID", empID);
                cmd.Parameters.AddWithValue("@empName", empName);


                cmd.ExecuteNonQuery();

                this.CloseConnection();

            }
        }

        public void UpdateEmpSurname(string empSurname, string empID)
        {
            string query = "UPDATE employees SET EmployeeSurname=@empSurname WHERE EmployeeID=@empID";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@empID", empID);
                cmd.Parameters.AddWithValue("@empSurname", empSurname);

                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        public void UpdateEmpIDNum(string empIDNum, string empID)
        {
            string query = "UPDATE employees SET EmployeeIDNumber=@empIDNum WHERE EmployeeID=@empID";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@empID", empID);
                cmd.Parameters.AddWithValue("@empIDNum", empIDNum);

                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        public void UpdateEmpPass(string empPass, string empID)
        {
            string query = "UPDATE employees SET EmployeePassword=@empPass WHERE EmployeeID=@empID";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@empID", empID);
                cmd.Parameters.AddWithValue("@empPass", empPass);

                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }
        
        //Delete statement
        public void Delete(string empID)
        {
            string query = "DELETE FROM employees WHERE EmployeeID=@empID";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@empID", empID);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        #endregion




        //Backup
        public void Backup()
        {
            try
            {
                DateTime Time = DateTime.Now;
                int year = Time.Year;
                int month = Time.Month;
                int day = Time.Day;
                int hour = Time.Hour;
                int minute = Time.Minute;
                int second = Time.Second;
                int millisecond = Time.Millisecond;

                //Save file to C:\ with the current date as a filename
                string path;
                path = "C:\\MySqlBackup" + year + "-" + month + "-" + day +
            "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
                StreamWriter file = new StreamWriter(path);


                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysqldump";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}",
                    uid, password, server, database);
                psi.UseShellExecute = false;

                Process process = Process.Start(psi);

                string output;
                output = process.StandardOutput.ReadToEnd();
                file.WriteLine(output);
                process.WaitForExit();
                file.Close();
                process.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error "+ex+"  , unable to backup!");
            }
        }

        //Restore
        public void Restore()
        {
            try
            {
                //Read file from C:\
                string path;
                path = "C:\\MySqlBackup.sql";
                StreamReader file = new StreamReader(path);
                string input = file.ReadToEnd();
                file.Close();

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysql";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = false;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}",
                    uid, password, server, database);
                psi.UseShellExecute = false;


                Process process = Process.Start(psi);
                process.StandardInput.WriteLine(input);
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error "+ex+", unable to Restore!");
            }
        }
    }

    
}
