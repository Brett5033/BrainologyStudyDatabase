using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainologyStudyDatabase
{
    
    public class DatabaseHandler
    {
        /// <summary>
        /// Connection string for sql localhost
        /// Might be static only
        /// </summary>
        public string connetionString = Properties.Settings.Default.Database1ConnectionString;
        //@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True";

        /// <summary>
        /// Sql connection for database
        /// </summary>
        public SqlConnection cnn;

        /// <summary>
        /// Constructor
        /// </summary>
        public DatabaseHandler()
        {
            cnn = new SqlConnection(connetionString);
        }

        /// <summary>
        /// Takes a "void" SQL command and executes it
        /// </summary>
        /// <param name="command"></param>
        public void executeCommand(string command)
        {
            try
            {
                Console.WriteLine("Executing Command:\n" + command);
                using (SqlCommand dbCommand = new SqlCommand(command, cnn))
                {
                    dbCommand.CommandType = CommandType.Text;
                    cnn.Open();
                    int rowsAffected = dbCommand.ExecuteNonQuery();
                    cnn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cnn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// Takes a "void" parameterized SQL command and executes it
        /// </summary>
        /// <param name="command"></param>
        public void executeCommand(string command, SqlParameter[] parameters)
        {
            try
            {
                using (SqlCommand dbCommand = new SqlCommand(command, cnn))
                {
                    dbCommand.CommandType = CommandType.Text;

                    Console.WriteLine("Executing Command:\n" + command);
                    Console.WriteLine("Adding Params From List: ");
                    foreach(SqlParameter p in parameters)
                    {
                        dbCommand.Parameters.Add(p);
                        Console.WriteLine(p.ParameterName + "\tType: " + p.SqlDbType.ToString() + "\tVal: " + p.Value);
                    }
                    cnn.Open();
                    int rowsAffected = dbCommand.ExecuteNonQuery();
                    cnn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cnn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// Takes a "void" parameterized SQL command and executes it
        /// </summary>
        /// <param name="command"></param>
        public int executeScalar(string command, SqlParameter[] parameters)
        {
            try
            {
                using (SqlCommand dbCommand = new SqlCommand(command, cnn))
                {
                    dbCommand.CommandType = CommandType.Text;

                    Console.WriteLine("Executing Command:\n" + command);
                    Console.WriteLine("Adding Params From List: ");
                    foreach (SqlParameter p in parameters)
                    {
                        dbCommand.Parameters.Add(p);
                        Console.WriteLine(p.ParameterName + "\tType: " + p.SqlDbType.ToString() + "\tVal: " + p.Value);
                    }
                    cnn.Open();
                    int identity = Convert.ToInt32(dbCommand.ExecuteScalar());
                    cnn.Close();
                    return identity;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cnn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// Takes a query, executes it, and returns the dataTable representing the results
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable compileQuery(string query)
        {
            try
            {
                Console.WriteLine("Compiling Query:\n" + query);
                SqlCommand displayCommand = new SqlCommand(query, cnn);
                displayCommand.CommandType = CommandType.Text;
                
                DataTable dTable = new DataTable();

                cnn.Open();

                SqlDataReader myReader = displayCommand.ExecuteReader();
                dTable.Load(myReader);

                myReader.Close();
                cnn.Close();
                
                return dTable;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                cnn.Close();
                return new DataTable();
                //throw ex;
            }
        }

        public DataTable compileQuery(string query, SqlParameter[] parameters)
        {
            try
            {
                using (SqlCommand displayCommand = new SqlCommand(query, cnn))
                {
                    displayCommand.CommandType = CommandType.Text;

                    Console.WriteLine("Compiling Query:\n" + query);
                    Console.WriteLine("Adding Params From Query List: ");
                    foreach (SqlParameter p in parameters)
                    {
                        displayCommand.Parameters.Add(p);
                        Console.WriteLine(p.ParameterName + "\tType: " + p.SqlDbType.ToString() + "\tVal: " + p.Value);
                    }

                    DataTable dTable = new DataTable();

                    cnn.Open();

                    SqlDataReader myReader = displayCommand.ExecuteReader();
                    dTable.Load(myReader);


                    myReader.Close();
                    cnn.Close();

                    return dTable;
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cnn.Close();
                return new DataTable();
                //throw ex;
            }
        }

        

        public int mergeDataTable(DataTable table, DatabaseEnums.TABLES currentTable)
        {

            SqlDataAdapter viewAdapter = new SqlDataAdapter("Select * From " + currentTable, cnn);
            SqlCommandBuilder builder = new SqlCommandBuilder(viewAdapter);
            viewAdapter.UpdateCommand = builder.GetUpdateCommand();


            //table = new DataTable();
            //viewAdapter.Fill(table);
            foreach (DataRow user in table.Rows)
            {
                foreach (DataColumn c in table.Columns)
                {
                    Console.WriteLine(c.ColumnName);
                    if (c.ReadOnly)
                    {
                        Console.WriteLine("Read Only: " + c.ColumnName + " - " + user[c.ColumnName]);
                    }
                    else
                    {
                        if (c.DataType != typeof(DateTime))
                        {
                            // Clean up empty space around field entries
                            user[c.ColumnName] = user[c.ColumnName].ToString().Trim();
                        }
                    }

                }
                // user.AcceptChanges(); 
                // Do not do an accept changes for either the table or the row before your ViewAdapter Update. 
                // It will appear as though you do not have changes to push.
            }

            // Users.AcceptChanges();
            return viewAdapter.Update(table);
        }

        // "Probably" not efficient, but will save me time
        // Joins every table (ses, seg, p, eeg, sv, s)
        public static string JoinStudyKey = "SESSION ses LEFT JOIN PARTICIPANT p ON ses.PARTICIPANT_ID = p.PARTICIPANT_ID LEFT JOIN STUDY_VERSION sv ON ses.VERSION_ID = sv.VERSION_ID LEFT JOIN SEGMENT seg ON ses.VERSION_ID = seg.VERSION_ID LEFT JOIN EEG_AMPLITUDE eeg ON ses.SESSION_ID = eeg.SESSION_ID LEFT JOIN STUDY s ON sv.STUDY_ID = s.STUDY_ID";
        public DataTable getPrimaryKeyFromTable(DatabaseEnums.TABLES table)
        {
            string query = @"SELECT KU.table_name as TABLENAME, column_name as PRIMARYKEYCOLUMN FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC 

                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU
                    ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' 
                    AND TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME 
                    AND KU.table_name='" + table.ToString() + @"'

                ORDER BY 
                    KU.TABLE_NAME
                   ,KU.ORDINAL_POSITION;";
            DataTable result = compileQuery(query);

            return result;
        }

        public static string getValidValuesForForiegnKey(string col, string table)
        {
            string e = table + "_COL." + col;
            //Console.WriteLine("Looking for: " + e);
            switch (e)
            {
                case "STUDY_VERSION_COL.STUDY_ID":
                    {
                        //Console.WriteLine("Match " + e);
                        return @"SELECT STUDY_ID, NAME FROM STUDY";
                    }
                case "SEGMENT_COL.VERSION_ID":
                    {
                        return @"SELECT v.VERSION_ID, s.NAME, v.DESCRIPTION FROM STUDY_VERSION v LEFT JOIN STUDY s ON v.STUDY_ID = s.STUDY_ID";
                    }
                case "SESSION_COL.VERSION_ID":
                    {
                        return @"SELECT v.VERSION_ID, s.NAME, v.DESCRIPTION FROM STUDY_VERSION v LEFT JOIN STUDY s ON v.STUDY_ID = s.STUDY_ID";
                    }
                case "SESSION_COL.PARTICIPANT_ID":
                    {
                        return @"SELECT PARTICIPANT_ID, FIRST_NAME, LAST_NAME FROM PARTICIPANT";
                    }
                case "EEG_AMPLITUDE_COL.SESSION_ID":
                    {
                        return @"SELECT ses.SESSION_ID, p.FIRST_NAME, p.LAST_NAME, s.NAME, sv.DESCRIPTION FROM SESSION ses INNER JOIN STUDY_VERSION sv ON ses.VERSION_ID = sv.VERSION_ID INNER JOIN STUDY s ON sv.STUDY_ID = s.STUDY_ID INNER JOIN PARTICIPANT p ON ses.PARTICIPANT_ID = p.PARTICIPANT_ID";
                    }
                case "EEG_AMPLITUDE_COL.SEGMENT_ID":
                    {
                        return @"SELECT seg.SEGMENT_ID, seg.NAME FROM SEGMENT seg";
                    }
                case "VISIT_COL.PARTICIPANT_ID":
                    {
                        return @"SELECT PARTICIPANT_ID, FIRST_NAME, LAST_NAME FROM PARTICIPANT";
                    }
                case "VISIT_COL.LOCATION_ID":
                    {
                        return @"SELECT LOCATION_ID, NAME FROM LOCATION";
                    }
            }
            return "";
        }

        public static List<string> getValuesFromEnums(string col, string table)
        {
            string e = table + "." + col;
            Console.WriteLine("Looking for: " + e);

            switch (e)
            {
                case "PARTICIPANT.GENDER":
                    {
                        Console.WriteLine("Match " + e);
                        return new List<string>() { "Male", "Female", "Non-Binary" };
                    }
                case "PARTICIPANT.ETHNICITY":
                    {
                        Console.WriteLine("Match " + e);
                        return new List<string>() { "WHITE", "HISPANIC or LATINO", "ASIAN", "BLACK or AFRICAN AMERICAN", "AMERICAN INDIAN or ALASKAN NATIVE", "NATIVE HAWAIIAN or OTHER PACIFIC ISLANDER" };
                    }
            }
            return new List<string>();
        }

        public int getValueFromForeignKey(string value, DatabaseEnums.TABLES table, string ColName)
        {
            Console.WriteLine("Looking for ID of: " + value + " in table: " + table.ToString());
            string query = @"SELECT * FROM " + table.ToString() + " WHERE '" + value + "' IN (" + ColName + ")";

            // This query should return a index representing the index of the foreign key
            DataTable partResult = compileQuery(query);
            Console.WriteLine("Query result: " + query);
            int foreignIndex;
            if (partResult.Rows.Count > 0)
                foreignIndex = (int)partResult.Rows[0][0];
            else
                foreignIndex = -1;
            
            return foreignIndex;
        }
    }
}
