/***************************************************************
DataQuery

10/20/2017

Programmer: Ben Stockwell

Purpose: Emulate an SQL Database with Commands in a form

***************************************************************/

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace DataQuery
{
    public partial class Form1 : Form
    {
        #region Form Load

        public Form1()
        {
            InitializeComponent();
        }

        // Execute Code when the form loads
        // Sets up the Database for use with this program
        private void Form1_Load(object sender, EventArgs e)
        {
            // Try to connect to the SQL server
            try
            {
                // Try to create a connection to the SQL Server
                // This will not work unless the correct information is filled in for the database
                // The program should still open, slowly
                using (MySqlConnection connection = new MySqlConnection("Server=XXXXXX;Database=XXXXXX;Uid=XXXXXX;Pwd=XXXXXX;"))
                {
                    connection.Open(); // Open connection

                    // Delete the tables if they exist
                    Delete_tables(connection);

                    // Create new tables
                    Create_tables(connection);

                    // Fill the tables with data from the files
                    Fill_tables(connection);

                }

            }
            catch (Exception mye)
            {
                // Write the exeception to the console and listbox
                Out_Listbox.Items.Add(mye.Message);
                Console.WriteLine(mye.Message);
                Console.WriteLine(mye.Message);
                Console.WriteLine(mye.Message);
                MessageBox.Show("Exception Thrown:\n" + mye.Message);
                //MessageBox.Show("Table or Input file ERROR:\n" + mye.Message);
            }

            // Print Title
            Out_Listbox.Items.Clear();
            Out_Listbox.Items.Add("Welcome, Current Tables:");
            Out_Listbox.Items.Add("");

            // Print the tables in the DB
            PrntTables();

        }
        
        #endregion

        #region Table Operations

        // Reads the input files
        // Fills the TABLES with DATA from the files
        private void Fill_tables(MySqlConnection connection)
        {
            // Add data to RoomTable
            Console.WriteLine("We now add records to RoomTable\n");
            using (StreamReader SR = new StreamReader("Room.txt"))
            {
                String S = SR.ReadLine();

                // Loop through the file reading lines
                while (S != null)
                {
                    String[] SArray = S.Split(',');  // Room.txt contains tab-delimited fields.
                    int M = Convert.ToInt32(SArray[1]);
                    int Rm = Convert.ToInt32(SArray[0]);
                    String SqlString = "Insert Into RoomTable (Room, Capacity, Smart) Values(" +
                                       "'" + Rm.ToString() + "', " + M.ToString() + ", '" + SArray[2] + "')";
                    //Console.WriteLine(SqlString);

                    // Run the command
                    MySqlCommand Command2 = new MySqlCommand(SqlString, connection);
                    Command2.ExecuteNonQuery();
                    S = SR.ReadLine();
                }// end of while
            } // end of using for StreamReader

            // Add data to OfficeTable
            Console.WriteLine("We now add records to OfficeTable\n");
            using (StreamReader SR = new StreamReader("Office.txt"))
            {
                String S = SR.ReadLine();
                // Loop through the file reading lines
                while (S != null)
                {
                    String[] SArray = S.Split(',');  // Room.txt contains tab-delimited fields.
                    int M = Convert.ToInt32(SArray[1]);
                    String SqlString = "Insert Into OfficeTable (Teacher , Office) Values(" +
                                       "'" + SArray[0] + "', " + M.ToString() + ")";
                    //Console.WriteLine(SqlString);

                    // Run the command
                    MySqlCommand Command2 = new MySqlCommand(SqlString, connection);
                    Command2.ExecuteNonQuery();
                    S = SR.ReadLine();
                }// end of while
            } // end of using for StreamReader

            // Add data to ClassTable
            Console.WriteLine("We now add records to ClassTable\n");
            using (StreamReader SR = new StreamReader("Class.txt"))
            {
                // Ints to Convert strings to ints
                int M0;
                int M1;
                int M3;
                int M6;

                String S = SR.ReadLine();
                while (S != null)
                {
                    // Show the string in the console
                    //Console.WriteLine(S);


                    // THIS WAS FOR THE ORIGINAL INPUT FILE FOR CLASSES
                    // If the line does not have Section 1 - ADD IT
                    //if ( (S.IndexOf('-')) == -1 )
                    //{
                    //    S = S.Insert((S.IndexOf(',')), "-1");
                    //}

                    // This split was also for the original input, but still works
                    // Split the String and Commas AND Dashes
                    String[] SArray = S.Split(new Char[] { ',', '-' }, StringSplitOptions.RemoveEmptyEntries);  // Room.txt contains tab-delimited fields.


                    // Convert some stings to ints
                    // Check if any of the INTS are passed in as NULL
                    {
                        if (SArray[0] != "null")
                        {
                            M0 = Convert.ToInt32(SArray[0]);
                        }
                        else
                        {
                            M0 = -1;
                        }

                        if (SArray[1] != "null")
                        {
                            M1 = Convert.ToInt32(SArray[1]);
                        }
                        else
                        {
                            M1 = -1;
                        }

                        if (SArray[3] != "null")
                        {
                            M3 = Convert.ToInt32(SArray[3]);
                        }
                        else
                        {
                            M3 = -1;
                        }

                        if (SArray[6] != "null")
                        {
                            M6 = Convert.ToInt32(SArray[6]);
                        }
                        else
                        {
                            M6 = -1;
                        }
                    }

                    // If the file has NULL for primary keys - it will display a message box
                    // Except for the "Section"... because the new input file has "section" as null
                    // despite the fact that it MUST be part of the primary key
                    // because there are classes with the same Number

                    // Create the command String
                    String SqlString = "Insert Into ClassTable (Class, Section, Teacher, Room, Time, Days, Enrollment) Values(" +
                                       (M0 == -1 ? "NULL" : M0.ToString()) + ", " + 
                                       (M1 == -1 ? "1" : M1.ToString()) + ", " +
                                       (SArray[2] == "null" ? "NULL" : "'" + SArray[2] + "'") + ", " +
                                       (M3 == -1 ? "NULL" : M3.ToString()) + ", " +
                                       (SArray[4] == "null" ? "NULL" : "'" + SArray[4] + "'") + ", " +
                                       (SArray[5] == "null" ? "NULL" : "'" + SArray[5] + "'") + ", " +
                                       (M6 == -1 ? "NULL" : M6.ToString()) + ")";
                    
                    
                    // Output the command to console
                    //Console.WriteLine(SqlString);

                    // Make the command and RUN IT
                    MySqlCommand Command2 = new MySqlCommand(SqlString, connection);
                    Command2.ExecuteNonQuery();
                    // Get a new Line
                    S = SR.ReadLine();
                }// end of while
            } // end of using for StreamReader

        }

        // This method will print the names of the tables to the ListBox
        private void PrntTables()
        {
            // Try to create a connection to the SQL Server
            using (MySqlConnection connection = new MySqlConnection("Server=XXXXXX;Database=XXXXXX;Uid=XXXXXX;Pwd=XXXXXX;"))
            {
                connection.Open();

                // Get list of tables to show
                {
                    DataTable schema = connection.GetSchema("Tables");
                    List<string> TableNames = new List<string>();
                    foreach (DataRow row in schema.Rows)
                    {
                        Out_Listbox.Items.Add(row[2].ToString());
                        Console.WriteLine(row[2].ToString());
                    }
                }
            }
        }

        // Function Used to Delete the Tables
        // Needs MySql Connection to be passed as argument
        private void Delete_tables(MySqlConnection connection)
        {

            // Get list of tables to show
            {
                DataTable schema = connection.GetSchema("Tables");
                List<string> TableNames = new List<string>();
                Out_Listbox.Items.Add("Tables presently in the database\n");
                foreach (DataRow row in schema.Rows)
                {

                    // Print the table to the console
                    Console.WriteLine(row[2].ToString());

                    // Check if the tables exist
                    // If they exist Drop them
                    if (row[2].ToString() == "RoomTable")
                    {
                        MySqlCommand command00 = new MySqlCommand("DROP TABLE RoomTable", connection);
                        command00.ExecuteNonQuery();

                        Console.WriteLine(" Dropped " + row[2].ToString());
                    }
                    else if (row[2].ToString() == "OfficeTable")
                    {
                        MySqlCommand command01 = new MySqlCommand("Drop Table OfficeTable", connection);
                        command01.ExecuteNonQuery();

                        Console.WriteLine(" Dropped " + row[2].ToString());
                    }
                    else if (row[2].ToString() == "ClassTable")
                    {
                        MySqlCommand command02 = new MySqlCommand("Drop Table ClassTable", connection);
                        command02.ExecuteNonQuery();

                        Console.WriteLine(" Dropped " + row[2].ToString());
                    }

                }
            }

        }


        // Function Used to Create the Tables
        // Needs MySql Connection to be passed as argument
        private void Create_tables(MySqlConnection connection)
        {
            // Create the Commands and Execute them
            // Room Table
            MySqlCommand command1 = new MySqlCommand("Create Table RoomTable (Room int, Capacity int, Smart char(1), PRIMARY KEY (Room))", connection);
            command1.ExecuteNonQuery();
            // Office Table
            MySqlCommand command2 = new MySqlCommand("Create Table OfficeTable (Teacher char(20), Office int, PRIMARY KEY (Teacher))", connection);
            command2.ExecuteNonQuery();
            // ClassTable
            MySqlCommand command3 = new MySqlCommand("Create Table ClassTable (Class int, " +
                                                                              "Section int, " +
                                                                              "Teacher char(20), " +
                                                                              "Room int, " +
                                                                              "Time char(5), " +
                                                                              "Days char(5), " +
                                                                              "Enrollment int, " +
                                                                              "CONSTRAINT PK_Class PRIMARY KEY (Class, Section), " +
                                                                              "FOREIGN KEY (Teacher) REFERENCES OfficeTable(Teacher), " +
                                                                              "FOREIGN KEY (Room) REFERENCES RoomTable(Room))", connection);
            command3.ExecuteNonQuery();

        }

        #endregion

        #region Button Event Handlers

        // This is the EXECUTE BUTTON event handler
        // When the EXECUTE BUTTON is pressed it will call this function
        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            //Console.WriteLine(" ---- ---- Execute Button Clicked - ");
            // Clear the box
            Out_Listbox.Items.Clear();

            // If there is no command Print and Error
            if ( CommandInput.Text == "" )
            {
                // Print instructions to the List Box
                Out_Listbox.Items.Add("ERROR - NO COMMAND ENTERED!");
                Out_Listbox.Items.Add("Enter a Command!");
                Out_Listbox.Items.Add("");
                Out_Listbox.Items.Add("Working Commands:");
                Out_Listbox.Items.Add("   SELECT");
                Out_Listbox.Items.Add("   INSERT");
            }
            else
            {
                // If the command is not an Empty string 

                // Put the command in a string
                String InCommand = CommandInput.Text;

                // Check what command was entered
                if ( InCommand.IndexOf("Select") == 0 || InCommand.IndexOf("SELECT") == 0)
                {
                    Out_Listbox.Items.Add("You Tried");
                    Out_Listbox.Items.Add("   SELECT");

                    Select_CMD(InCommand);
                }
                else if ( InCommand.IndexOf("Insert") == 0 || InCommand.IndexOf("INSERT") == 0)
                {
                    Out_Listbox.Items.Add("You Tried");
                    Out_Listbox.Items.Add("   INSERT");

                    Insert_CMD(InCommand);
                }
                else
                {
                    // If the Command is wrong, print error
                    Out_Listbox.Items.Add("COMMAND INVALID or NOT IMPLEMENTED");
                    Out_Listbox.Items.Add("  Valid Commands:");
                    Out_Listbox.Items.Add("  SELECT  or  Select");
                    Out_Listbox.Items.Add("  INSERT  or  Insert");
                }

            }


            //Console.WriteLine(" ---- ---- Finished Execute - ");
        }

        // Handles the event for when a USER presses ENTER while typing in the
        // Command Box
        // Makes entering commands much more convenient
        private void CommandInput_KeyDown(object sender, KeyEventArgs e)
        {
            // If the enter key is pressed, Call the Execute event handler
            if (e.KeyCode == Keys.Enter)
            {
                ExecuteButton_Click(this, new EventArgs());
            }
        }

        // This is the CLEAR BUTTON event handler
        private void ClearListButton_Click(object sender, EventArgs e)
        {
            // Clear the command Box
            CommandInput.Text = "";

            // Clear the list
            Out_Listbox.Items.Clear();
        }

        // This is the EXIT BUTTON Event Handler
        private void ExitButton_Click(object sender, EventArgs e)
        {
            // Exit the app
            Application.Exit();
        }

        #endregion

        #region SQL Command Handlers

        // Handles when the User Enters a SELECT SQL Command
        // Opens connection to the SQL Server and Runs the command
        private void Select_CMD(string inCommand)
        {
            // Clear the listbox
            Out_Listbox.Items.Clear();
            try
            {
                using (MySqlConnection connection = new MySqlConnection("Server=XXXXXX;Database=XXXXXX;Uid=XXXXXX;Pwd=XXXXXX;"))
                {
                    // Make the command and open the connection
                    MySqlCommand command = new MySqlCommand(inCommand, connection);
                    connection.Open();

                    // Create a reader object for the data
                    MySqlDataReader reader = command.ExecuteReader();

                    // Variable for header
                    int i = 0;

                    // Loop through and add the items
                    while (reader.Read())
                    {
                        // This Prints the COLUMN HEADERS for EACH COLUMN
                        if (i == 0)
                        {
                            // Variables
                            int count = reader.FieldCount;
                            String Row = "";

                            // Loop and get the Row Names
                            for (int x = 0; x < count; x++)
                            {
                                Row = Row + String.Format("{0, -8} ", reader.GetName(x));
                            }
                            // Add the header to the Listbox
                            Out_Listbox.Items.Add(String.Format(Row));
                            i = 2; // Set the condition
                        }

                        // Read the next Row
                        ReadSingleRow((IDataRecord)reader);
                    }

                    // Call Close when done reading.
                    reader.Close();
                }
            }
            catch(Exception SQLe)
            {
                // If there is an error
                Out_Listbox.Items.Clear();
                // Print the message to the List Box
                Out_Listbox.Items.Add(" - ERROR - Exception Thrown - ");
                Out_Listbox.Items.Add(" - Your syntax is incorrect - ");
                Out_Listbox.Items.Add(SQLe.Message);
                Out_Listbox.Items.Add(SQLe.Message);
                Console.WriteLine(SQLe.Message);
                Console.WriteLine(SQLe.Message);
            }
        }

        // Handles when the User Enters an INSERT SQL Command
        // Opens connection to the SQL Server and Runs the command
        private void Insert_CMD(string inCommand)
        {
            Out_Listbox.Items.Clear();
            try
            {
                // Connect to the SQL server
                using (MySqlConnection connection = new MySqlConnection("Server=XXXXXX;Database=XXXXXX;Uid=XXXXXX;Pwd=XXXXXX;"))
                {
                    // Make the command and open the connection
                    MySqlCommand command = new MySqlCommand(inCommand, connection);
                    connection.Open();

                    // Try to run the command
                    command.ExecuteNonQuery();

                    Out_Listbox.Items.Add(" Command Executed ");
                    Out_Listbox.Items.Add("     -Insert Succesful");
                }
            }
            catch (Exception SQLe)
            {
                // If there is an error
                Out_Listbox.Items.Clear();
                // Print the message to the List Box
                Out_Listbox.Items.Add(" - ERROR - Exception Thrown - ");
                Out_Listbox.Items.Add(" - Your syntax is incorrect - ");
                Out_Listbox.Items.Add(SQLe.Message);
                Out_Listbox.Items.Add(SQLe.Message);
                Console.WriteLine(SQLe.Message);
                Console.WriteLine(SQLe.Message);
            }


        }

        // Formats how the Select command is output
        // Used by the Select SQL command Handler
        private void ReadSingleRow(IDataRecord record)
        {
            // Variables
            int count = record.FieldCount;
            String Row = "";

            // Loop to get EACH of the rows and add to a string
            for (int x = 0; x < count; x++)
            {
                Row = Row + String.Format("{0, -8} ", record[x]);
            }

            // Add the entry to the box
            Out_Listbox.Items.Add(Row);

        }// end of ReadSingleRow

        #endregion

    }
}
