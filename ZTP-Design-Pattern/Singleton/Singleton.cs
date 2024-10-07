using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTP_Design_Pattern.Singleton
{
interface IDatabaseConnection
    {
        int AddRecord(string name, int age);

        void UpdateRecord(int id, string newName, int newAge);

        void DeleteRecord(int id);

        Record GetRecord(int id);

        void ShowAllRecords();
    }

    // Prosty rekord w bazie danych
    class Record
    {
        public int Id { get; }
        public string Name { get; set; }
        public int Age { get; set; }

        public Record(int id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            return $"Record [ID={Id}, Name={Name}, Age={Age}]";
        }
    }

    // Prosta baza danych
    class Database
    {
        private List<Record> records; // Lista przechowująca rekordy
        private int nextId = 1; // Licznik do generowania unikalnych ID

        public Database()
        {
            records = new List<Record>();
        }

        // Zwraca implementację interfejsu IDatabaseConnection
        public IDatabaseConnection GetConnection()
        {
            return new DatabaseConnectionImpl(this);
        }

        // Prywatna klasa wewnętrzna implementująca interfejs IDatabaseConnection
        private class DatabaseConnectionImpl : IDatabaseConnection
        {
            private Database _database;

            public DatabaseConnectionImpl(Database database)
            {
                _database = database;
            }

            // Dodawanie nowego rekordu
            public int AddRecord(string name, int age)
            {
                var newRecord = new Record(_database.nextId++, name, age);
                _database.records.Add(newRecord);
                Console.WriteLine("Inserted: " + newRecord);
                return newRecord.Id;
            }

            // Pobieranie rekordu po ID
            public Record GetRecord(int id)
            {
                return _database.records.FirstOrDefault(record => record.Id == id);
            }

            // Aktualizowanie rekordu po ID
            public void UpdateRecord(int id, string newName, int newAge)
            {
                var record = GetRecord(id);

                if (record != null)
                {
                    record.Name = newName;
                    record.Age = newAge;
                    Console.WriteLine("Updated: " + record);
                }
                else
                {
                    Console.WriteLine("Record with ID " + id + " not found.");
                }
            }

            // Usuwanie rekordu po ID
            public void DeleteRecord(int id)
            {
                var record = GetRecord(id);

                if (record != null)
                {
                    _database.records.Remove(record);
                    Console.WriteLine("Deleted record with ID " + id);
                }
                else
                {
                    Console.WriteLine("Record with ID " + id + " not found.");
                }
            }

            // Wyświetlanie wszystkich rekordów
            public void ShowAllRecords()
            {
                if (!_database.records.Any())
                {
                    Console.WriteLine("No records in the database.");
                }
                else
                {
                    Console.WriteLine("All records:");
                    _database.records.ForEach(Console.WriteLine);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Tworzenie bazy danych
            var database = new Database();

            // Tworzenie połączenia z bazą danych (klient operuje na interfejsie)
            IDatabaseConnection connection = database.GetConnection();

            // Operacje na bazie danych za pośrednictwem połączenia
            connection.AddRecord("Piotr", 25);
            connection.ShowAllRecords();
        }
    }

}
