using Personal_Website.Models;
using System.Data;
using System.Data.SQLite;
using System.Reflection;

namespace Personal_Website.Utilities
{
    public sealed class Database
    {
        static string db_path = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName.ToString() + "\\db\\database.db";

        string connection_string = $"Data Source={db_path}";

        public bool UpdateProfile_One(Profile profile)
        {
            try
            {
                SQLiteCommand sQLiteCommand = new SQLiteCommand();
                sQLiteCommand.CommandText =
                    "Update Profile SET Name=@name,Description=@description,Work_Experience=@work_experience,Email=@email,Twitter=@twitter,Instagram=@instagram,Linkedin=@linkedin,Image=@image WHERE ID=1";
                sQLiteCommand.Parameters.AddWithValue("name", profile.Name);
                sQLiteCommand.Parameters.AddWithValue("description", profile.Description);
                sQLiteCommand.Parameters.AddWithValue("work_experience", profile.Work_Experience);
                sQLiteCommand.Parameters.AddWithValue("email", profile.Email);
                sQLiteCommand.Parameters.AddWithValue("twitter", profile.Twitter);
                sQLiteCommand.Parameters.AddWithValue("instagram", profile.Instagram);
                sQLiteCommand.Parameters.AddWithValue("linkedin", profile.Linkedin);
                sQLiteCommand.Parameters.AddWithValue("image", profile.Image);
                ExecuteCommand(sQLiteCommand);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsExistRecord(string table, int id)
        {
            DataTable record = ReadData(table, id);
            if (record.Rows.Count < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool IsExistDatabase()
        {
            return File.Exists(db_path);
        }
        private bool ExecuteCommand(SQLiteCommand sQLiteCommand)
        {
            try
            {
                SQLiteConnection connection = new SQLiteConnection(connection_string);
                sQLiteCommand.Connection = connection;
                connection.Open();
                sQLiteCommand.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddData(Profile profile)
        {
            try
            {
                // just one profile exist this applicaiton, so only one profile exist in data base
                profile.Id = 1;


                SQLiteCommand sQLiteCommand = new SQLiteCommand();
                sQLiteCommand.CommandText = "INSERT INTO Profile (Id, Name,Description, Work_Experience,Email, Twitter, Instagram,Linkedin,Image) VALUES(@id,@name,@description,@work_experience,@email,@twitter,@instagram,@linkedin,@image)";
                sQLiteCommand.CommandType = CommandType.Text;
                sQLiteCommand.Parameters.AddWithValue("id", profile.Id);
                sQLiteCommand.Parameters.AddWithValue("name", profile.Name);
                sQLiteCommand.Parameters.AddWithValue("description", profile.Description);
                sQLiteCommand.Parameters.AddWithValue("work_experience", profile.Work_Experience);
                sQLiteCommand.Parameters.AddWithValue("email", profile.Email);
                sQLiteCommand.Parameters.AddWithValue("twitter", profile.Twitter);
                sQLiteCommand.Parameters.AddWithValue("instagram", profile.Instagram);
                sQLiteCommand.Parameters.AddWithValue("linkedin", profile.Linkedin);
                sQLiteCommand.Parameters.AddWithValue("image", profile.Image);
                ExecuteCommand(sQLiteCommand);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddData(Experience experience)
        {
            try
            {
                SQLiteCommand sQLiteCommand = new SQLiteCommand();
                sQLiteCommand.CommandText = "INSERT INTO Experience (Company_Name,Time_Period,Description) VALUES(@company_name,@time_period,@description)";
                sQLiteCommand.Parameters.AddWithValue("company_name", experience.Company_Name);
                sQLiteCommand.Parameters.AddWithValue("time_period", experience.Time_Period);
                sQLiteCommand.Parameters.AddWithValue("description", experience.Description);
                ExecuteCommand(sQLiteCommand);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddData(Skill skill)
        {
            try
            {
                SQLiteCommand sQLiteCommand = new SQLiteCommand();
                sQLiteCommand.CommandText = "INSERT INTO Skill (Name,Ability_Rate) VALUES(@name,@ability_rate)";
                sQLiteCommand.Parameters.AddWithValue("name", skill.Name);
                sQLiteCommand.Parameters.AddWithValue("ability_rate", skill.Ability_Rate);
                ExecuteCommand(sQLiteCommand);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddData(Education education)
        {
            try
            {
                SQLiteCommand sQLiteCommand = new SQLiteCommand();
                sQLiteCommand.CommandText = "INSERT INTO Education (Time_Period,University,Description) VALUES(@time_period,@university,@description)";
                sQLiteCommand.Parameters.AddWithValue("time_period", education.Time_Period);
                sQLiteCommand.Parameters.AddWithValue("university", education.University);
                sQLiteCommand.Parameters.AddWithValue("description", education.Description);
                ExecuteCommand(sQLiteCommand);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddData(Admin admin)
        {
            try
            {
                SQLiteCommand sQLiteCommand = new SQLiteCommand();
                sQLiteCommand.CommandText = "INSERT INTO Admin (Username,Password) VALUES(@username,@password)";
                sQLiteCommand.Parameters.AddWithValue("username", admin.Username);
                sQLiteCommand.Parameters.AddWithValue("password", admin.Password);
                ExecuteCommand(sQLiteCommand);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsExistAdmin(Admin admin)
        {
            string query = $"SELECT * FROM Admin WHERE Username='{admin.Username}' AND Password='{admin.Password}';";
            SQLiteCommand sQLiteCommand = new SQLiteCommand();
            sQLiteCommand.CommandText = query;
            DataTable data = ReadDataCommand(sQLiteCommand);

            if (data.Rows.Count < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool DeleteData(string table, int id)
        {
            try
            {
                SQLiteCommand sQLiteCommand = new SQLiteCommand();
                sQLiteCommand.CommandText = $"DELETE FROM {table} WHERE ID=@Id;";
                sQLiteCommand.Parameters.AddWithValue("Id", id);
                ExecuteCommand(sQLiteCommand);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private DataTable ReadDataCommand(SQLiteCommand sQLiteCommand)
        {
            SQLiteConnection connection = new SQLiteConnection(connection_string);
            sQLiteCommand.Connection = connection;
            connection.Open();
            SQLiteDataReader data = sQLiteCommand.ExecuteReader();
            DataTable datatable = new DataTable();
            datatable.Load(data);
            connection.Close();
            return datatable;
        }
        public DataTable ReadData(string table, int id = -1, int limit = -1)
        {
            string query = $"SELECT * FROM {table}";
            if (id != -1)
            {
                query = query + $" WHERE ID={id}";
            }
            if (limit != -1)
            {
                query = query + $" LIMIT {limit}";
            }
            query += ";";
            SQLiteCommand sQLiteCommand = new SQLiteCommand();
            sQLiteCommand.CommandText = query;
            return ReadDataCommand(sQLiteCommand);

        }
        public bool CreateDatabase()
        {
            try
            {
                if (File.Exists(db_path))
                    return true;

                // Create Database
                SQLiteConnection.CreateFile(db_path);

                List<string> commands = new List<string>()
                {
                "CREATE TABLE Profile (ID    INTEGER NOT NULL,Name  TEXT NOT NULL,Description  TEXT NOT NULL,Work_Experience   TEXT NOT NULL,Email TEXT NOT NULL,Twitter   TEXT NOT NULL,Instagram TEXT NOT NULL,Linkedin TEXT NOT NULL,Image TEXT," +
                "PRIMARY KEY(ID));",

                "CREATE TABLE Education (ID	INTEGER NOT NULL,Time_Period   TEXT NOT NULL,University    TEXT NOT NULL,Description   TEXT NOT NULL," +
                "PRIMARY KEY(ID));",
                "CREATE TABLE Experience (ID	INTEGER NOT NULL,Company_Name  TEXT NOT NULL,Time_Period   TEXT NOT NULL,Description   TEXT NOT NULL," +
                "PRIMARY KEY(ID));",
                "CREATE TABLE Skill (ID	INTEGER NOT NULL,Name  TEXT NOT NULL,Ability_Rate  INTEGER NOT NULL," +
                "PRIMARY KEY(ID));",
                "CREATE TABLE Admin (ID INTEGER NOT NULL,Username TEXT NOT NULL,Password TEXT NOT NULL," +
                "PRIMARY KEY(ID));"
                };

                foreach (var command in commands)
                {
                    // Create Tables
                    SQLiteCommand sQLiteCommand = new SQLiteCommand();
                    sQLiteCommand.CommandText = command;
                    ExecuteCommand(sQLiteCommand);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
