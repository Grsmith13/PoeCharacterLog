using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

/*
    Gage Smith
    7/25/2024

    Application connects to a MsSQL in a docker container and allows the user to enter character details to build a database of their characters from Path of Exile.
    Currently things are a bit rough and need a lot of fixing up and optimization , but  will be periodically coming back and tidying things up. 
*/

namespace MySqlAPP
{

    class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu();
            menu.MainMenu();
        }
    }
    class Menu
    {
        string databaseName = "PoeHistory";
        string tableName = "PoeCharacters";
        public void MainMenu()
        {

            // using (SqlConnection connection = new SqlConnection())
            // {
            //     connection.ConnectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=GibJobPlease2024;TrustServerCertificate=True";
            //     //Sets up connection to MsSQL
            //     connection.Open();
            // }

            bool inMenu = true;

            while (inMenu)
            {
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Create Database");
                Console.WriteLine("2. Create  Table");
                Console.WriteLine("3. Display Table");
                Console.WriteLine("4. Drop Table");
                Console.WriteLine("5. Add Character");
                Console.WriteLine("6. Delete Character");
                Console.WriteLine("7. Exit");
                Console.Write("Choose an option: ");
                switch (Console.ReadLine())
                {
                    case "1":
                        CreateDatabase();
                        break;
                    case "2":
                        CreateTable();
                        break;
                    case "3":
                        DisplayTable();
                        break;
                    case "4":
                        DropTable();
                        break;
                    case "5":
                        AddCharacter();
                        break;
                    case "6":
                        DeleteCharacter();
                        break;
                    case "7":
                        inMenu = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        //Checks if the database exists and creates if it doesn't
        private void CreateDatabase()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=GibJobPlease2024;TrustServerCertificate=True";
                //Sets up connection to MsSQL
                connection.Open();

                using (SqlCommand createDbCommand = new SqlCommand($"IF DB_ID('{databaseName}') IS NULL CREATE DATABASE {databaseName}", connection))
                {
                    createDbCommand.ExecuteNonQuery();
                    Console.WriteLine($"Database '{databaseName}' created successfully(or already existed).");
                }
            }
        }


        private void CreateTable()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=GibJobPlease2024;TrustServerCertificate=True";
                //Sets up connection to MsSQL
                connection.Open();

                connection.ChangeDatabase("POEHistory");
                string createTableQuery = $@"
                    IF OBJECT_ID('{tableName}', 'U') IS NULL
                    CREATE TABLE [{tableName}] (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Name NVARCHAR(50) NOT NULL,
                        Level INT NOT NULL,
                        League NVARCHAR(30) NOT NULL,
                        Ascendancy NVARCHAR(50) NOT NULL
                    )";
                using (SqlCommand createTableCommand = new SqlCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                    Console.WriteLine($"Table '{tableName}' created successfully (or already existed).");
                }
            }
        }
        private void DisplayTable()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=GibJobPlease2024;TrustServerCertificate=True";
                //Sets up connection to MsSQL
                connection.Open();

                connection.ChangeDatabase("POEHistory");
                Console.WriteLine($"Contents of table '{tableName}':");
                string query = $"SELECT * FROM {tableName}";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Level: {reader["Level"]}, League: {reader["League"]}, Ascendancy: {reader["Ascendancy"]}");
                        }
                    }
                }
            }
        }


        private void DropTable()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=GibJobPlease2024;TrustServerCertificate=True";
                //Sets up connection to MsSQL
                connection.Open();

                connection.ChangeDatabase("POEHistory");
                Console.WriteLine($"Dropping table '{tableName}'...");
                string query = $"DROP TABLE IF EXISTS {tableName}";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Table '{tableName}' dropped successfully.");
                }
            }
        }
        private void AddCharacter()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=GibJobPlease2024;TrustServerCertificate=True";
                //Sets up connection to MsSQL
                connection.Open();
                connection.ChangeDatabase("POEHistory");

                Console.Write("Enter character name: ");
                string? name = Console.ReadLine();
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Character name cannot be empty.");
                    return;
                }

                Console.Write("Enter character level: ");
                if (!int.TryParse(Console.ReadLine(), out int level))
                {
                    Console.WriteLine("Invalid input for character level.");
                    return;
                }

                Console.Write("Enter character league: ");
                string? league = Console.ReadLine();
                if (string.IsNullOrEmpty(league))
                {
                    Console.WriteLine("Character league cannot be empty.");
                    return;
                }

                Console.Write("Enter character ascendancy: ");
                string? ascendancy = Console.ReadLine();
                if (string.IsNullOrEmpty(ascendancy))
                {
                    Console.WriteLine("Character ascendancy cannot be empty.");
                    return;
                }

                string insertQuery = $@"
            INSERT INTO {tableName} (Name, Level, League, Ascendancy)
            VALUES (@Name, @Level, @League, @Ascendancy)";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@Name", name);
                    insertCommand.Parameters.AddWithValue("@Level", level);
                    insertCommand.Parameters.AddWithValue("@League", league);
                    insertCommand.Parameters.AddWithValue("@Ascendancy", ascendancy);
                    insertCommand.ExecuteNonQuery();
                    Console.WriteLine("Character inserted successfully.");
                }
            }
        }

        private void DeleteCharacter()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=GibJobPlease2024;TrustServerCertificate=True";
                //Sets up connection to MsSQL
                connection.Open();
                connection.ChangeDatabase("POEHistory");

                Console.Write("Enter the name of the character to delete: ");
                string? name = Console.ReadLine();
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Character name cannot be empty.");
                    return;
                }


                string deleteQuery = $@" DELETE FROM {tableName}  WHERE Name = @Name";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@Name", name);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Character deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Character not found.");
                    }
                }
            }
        }
    }
}















