using System;
using System.Collections.Generic;

interface IDatabaseConnection
{
    int AddRecord(string name, int age);
    void UpdateRecord(int id, string newName, int newAge);
    void DeleteRecord(int id);
    Record GetRecord(int id);
    void ShowAllRecords();
}

interface IConnectionManager
{
    IDatabaseConnection GetConnection(string databaseName);
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

// Klasa połączenia, która operuje na bazie danych
class DatabaseConnection : IDatabaseConnection
{
    private Database _database;

    public DatabaseConnection(Database database)
    {
        _database = database;
    }

    public int AddRecord(string name, int age)
    {
        var newRecord = new Record(_database.NextId++, name, age);
        _database.Records.Add(newRecord);
        Console.WriteLine("Inserted: " + newRecord);
        return newRecord.Id;
    }

    public Record GetRecord(int id)
    {
        return _database.Records.Find(record => record.Id == id);
    }

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

    public void DeleteRecord(int id)
    {
        var record = GetRecord(id);

        if (record != null)
        {
            _database.Records.Remove(record);
            Console.WriteLine("Deleted record with ID " + id);
        }
        else
        {
            Console.WriteLine("Record with ID " + id + " not found.");
        }
    }

    public void ShowAllRecords()
    {
        if (_database.Records.Count == 0)
        {
            Console.WriteLine("No records in the database.");
        }
        else
        {
            Console.WriteLine("All records:");
            _database.Records.ForEach(Console.WriteLine);
        }
    }
}

// Klasa Database (Multiton)
class Database
{
    private static Dictionary<string, Database> _instances = new Dictionary<string, Database>();

    public List<Record> Records { get; private set; }
    public int NextId { get; set; } = 1;

    private List<DatabaseConnection> _connectionPool;
    private int _maxConnections = 3;
    private int _currentConnectionIndex = 0;

    private Database()
    {
        Records = new List<Record>();
        _connectionPool = new List<DatabaseConnection>();
    }

    // Multiton - zwracamy jedną instancję dla danej nazwy bazy
    public static Database GetInstance(string name)
    {
        if (!_instances.ContainsKey(name))
        {
            _instances[name] = new Database();
        }
        return _instances[name];
    }

    // Zwraca połączenie z puli lub tworzy nowe
    public IDatabaseConnection GetConnection()
    {
        if (_connectionPool.Count < _maxConnections)
        {
            var newConnection = new DatabaseConnection(this);
            _connectionPool.Add(newConnection);
        }

        var connection = _connectionPool[_currentConnectionIndex];
        _currentConnectionIndex = (_currentConnectionIndex + 1) % _maxConnections; // cykliczny dostęp
        return connection;
    }
}

// Singleton - menadżer połączeń
class ConnectionManager : IConnectionManager
{
    private static ConnectionManager _instance;

    private ConnectionManager() { }

    public static ConnectionManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new ConnectionManager();
        }
        return _instance;
    }

    // Pobiera połączenie do danej bazy danych
    public IDatabaseConnection GetConnection(string databaseName)
    {
        Database db = Database.GetInstance(databaseName);
        return db.GetConnection();
    }
}

// Program testujący
class Program
{
    static void Main(string[] args)
    {
        IConnectionManager manager = ConnectionManager.GetInstance();

        // Pobieranie czterech połączeń do tej samej bazy danych
        IDatabaseConnection conn1 = manager.GetConnection("Baza1"); // Połączenie do bazy 1
        IDatabaseConnection conn2 = manager.GetConnection("Baza1"); // Drugie połączenie do bazy 1
        IDatabaseConnection conn3 = manager.GetConnection("Baza1"); // Trzecie połączenie do bazy 1
        IDatabaseConnection conn4 = manager.GetConnection("Baza1"); // Czwarte połączenie do bazy 1 (cykliczne, to samo co conn1)

        // Dodawanie rekordów
        conn1.AddRecord("Piotr", 25); // Do bazy 1
        conn2.AddRecord("Anna", 30);  // Do bazy 1

        // Pobieranie danych z bazy 1s  
        conn3.ShowAllRecords(); // Powinien pokazać oba rekordy

        // Wyświetlanie hash code'ów połączeń, aby sprawdzić cykliczność
        Console.WriteLine("Hash code conn1 (Baza1): " + conn1.GetHashCode());
        Console.WriteLine("Hash code conn2 (Baza1): " + conn2.GetHashCode());
        Console.WriteLine("Hash code conn3 (Baza1): " + conn3.GetHashCode());
        Console.WriteLine("Hash code conn4 (Baza1): " + conn4.GetHashCode());

        // Sprawdzanie, czy conn1 i conn4 są tym samym obiektem
        Console.WriteLine("Czy conn1 i conn4 to te same obiekty? " + (conn1.GetHashCode() == conn4.GetHashCode()));
    }
}
