/* Name: John Lain
 * Date: 11/14/2021
 * Program Name: SQLServerApplication.cs
 * Revision: -
 * 
 *                          #### Description and Usage ####
 * 
 * This program opens a server session with Microsoft SQL Server Management Studio 18.
 * The user is prompted to enter a userID and password before a server connection can be
 * opened. If either the userID and/or the password is incorrect, a system level exception
 * is thrown. This program uses a secure string to encrypt what the user types. After the 
 * session is opened, a new database called "SchoolDatabase" is created. A dialog box pops 
 * up letting the user know that the new database was created successfully. Next, a database
 * table called "Students" is created and header columns are added to the table. If there is 
 * an error creating the database table, a system level exception is thrown. If the database
 * table is successfully created, a dialog box pops up letting the user know the database 
 * table was created successfully. Finally, the rows of student information is inserted
 * into the database table.
 * 
 */

//Included namespaces
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Security;

//Using the Console Application namespace as default
namespace SQLServerApplication
{
    public partial class Form1 : Form
    {
        public static void Main()
        {
            //Get userID and password. We are using SQL login authentication
            //We are also using password encryption and decryption
            Console.Write("Enter UserID: ");
            string userID = Console.ReadLine();

            //Adds 1 line of whitespace
            Console.WriteLine();

            //Declare variable with no initialization; C# does this for us
            string passWord;

            //Instantiate the secure string object and encrypt password.
            SecureString securePwd = new SecureString();
            ConsoleKeyInfo key;

            //Prompt user for the password
            Console.Write("Enter Password: ");
            do
            {
                //Get user input from keyboard
                key = Console.ReadKey(true);

                //Append the character to the secure password.
                securePwd.AppendChar(key.KeyChar);

                //What the user sees when the password is typed
                Console.Write("*");

                //Convert secure string to regular string for server connection
                passWord = new System.Net.NetworkCredential(string.Empty, securePwd).Password;
                passWord = passWord.ToString();
              
                //Exit loop if Enter key is pressed.
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            
            //Declare variable with no initialization
            String str;

            //Create a new SQL connection object for the MS SQL MGMT server
            SqlConnection myConn = new SqlConnection

            //SQL server string with userID and password credential parameters
            ("Server=DESKTOP-U42MVMF\\SQLEXPRESS;Initial Catalog=master;User ID=" + userID + ";Password=" + passWord + "");
            str = "CREATE DATABASE SchoolDatabase";

            //Create new SQL command object
            SqlCommand myCommand = new SqlCommand(str, myConn);

            //Attempt to create the database | database table | populate database table rows
            try
            {
                //Open the server connection
                myConn.Open();

                //Send the command
                myCommand.ExecuteNonQuery();

                //Let the operator know the database was successfully created
                MessageBox.Show("Database Created Successfully", "Prompt",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
              
                //Create the database table "Students", populate column headers, and create new connection object
                myConn = new SqlConnection
                //Here we can use the generic connection string since we opened it with authentication.
                ("Server=DESKTOP-U42MVMF\\SQLEXPRESS;Integrated security=SSPI ;database=SchoolDatabase");

                //SQL syntax
                str = "CREATE TABLE Students (" +
                    " StudentID int NOT NULL," +
                    " LastName varchar(255) NOT NULL," +
                    " FirstName varchar(255) NOT NULL," +
                    " Major varchar(255) NOT NULL," +
                    " GPA float NOT NULL," +
                    ")";

                //Create a new command object
                SqlCommand myTableCommand = new SqlCommand(str, myConn);

                //Open the connection
                myConn.Open();
                
                //Send the command
                myTableCommand.ExecuteNonQuery();

                //Let the operator know the table was successfully created
                MessageBox.Show("Table Created Successfully", "Prompt",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

                //Create and initialize the student records data arrays
                string[] StudentLastName = new string[] {"'Shephard'", "'Smith'", "'Lain'", "'Deane'", "'Faha'"};
                string[] StudentFirstName = new string[] {"'Tom'", "'Dan'", "'John'", "'Seth'", "'David'"};
                string[] StudentMajor = new string[] {"'Math'", "'EE'","'Comp Sci'","'Math'","'EE'" };
                string[] StudentGPA = new string[] {"3.65", "3.47", "4.0", "3.96", "4.0"};
                string[] StudentID = new string[] {"134555", "134511", "134541", "134510", "134554"};

                //Loop and create database table entries from above arrays using SQL syntax
                for (int i = 0; i < StudentLastName.Length; i++)
                {
                    //Update the string with each loop iteration
                    str = "INSERT INTO Students (StudentID, LastName, FirstName, Major, GPA)" +
                          "VALUES (" + StudentID[i] + ", " + StudentLastName[i] + ", " + StudentFirstName[i] + 
                          ", " + StudentMajor[i] + ", " + StudentGPA[i] + ")";

                    //Create a new object with each loop iteration
                    SqlCommand myNewTableCommand = new SqlCommand(str, myConn);

                    //Send the command
                    myNewTableCommand.ExecuteNonQuery();
                }
            }

            //If unsuccessful throw an exception and alert the operator
            catch (System.Exception ex)
            {
                //Check to see if the server connection is open
                if (myConn.State == ConnectionState.Open)
                {
                    //Close the server connection/resourses
                    myConn.Close();
                }

                //Let the operator know an error occurred
                MessageBox.Show(ex.ToString(), "System Exception", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
            //If all goes well close the server connection
            finally
            {
                //Check to see if the server connection is open
                if (myConn.State == ConnectionState.Open)
                {
                    //Close the server connection/resourses
                    myConn.Close();
                }
            }
        }
    }
}
