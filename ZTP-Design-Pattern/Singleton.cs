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

// rekord w bazie danych
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

// Klasa połączenia która operuje na bazie danych
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
        _currentConnectionIndex = (_currentConnectionIndex + 1) % _maxConnections;
        return connection;
    }
}

// Singleton menadżer połączeń
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

class Program
{
    static void Main(string[] args)
    {
        IConnectionManager manager = ConnectionManager.GetInstance();

        IDatabaseConnection conn1 = manager.GetConnection("Baza1");
        IDatabaseConnection conn2 = manager.GetConnection("Baza1"); 
        IDatabaseConnection conn3 = manager.GetConnection("Baza1"); 
        IDatabaseConnection conn4 = manager.GetConnection("Baza1"); 

        conn1.AddRecord("Piotr", 25); 
        conn2.AddRecord("Anna", 30); 

        conn3.ShowAllRecords(); // powinno pokazac oba rekordy

        Console.WriteLine("Hash code conn1 (Baza1): " + conn1.GetHashCode());
        Console.WriteLine("Hash code conn2 (Baza1): " + conn2.GetHashCode());
        Console.WriteLine("Hash code conn3 (Baza1): " + conn3.GetHashCode());
        Console.WriteLine("Hash code conn4 (Baza1): " + conn4.GetHashCode());

        // czy conn1 i conn4 są tym samym obiektem
        Console.WriteLine("Czy conn1 i conn4 to te same obiekty? " + (conn1.GetHashCode() == conn4.GetHashCode()));
    }

    /*

    static void Main(string[] args)
    {
        IConnectionManager manager = ConnectionManager.GetInstance();

        IDatabaseConnection conn1 = manager.GetConnection("Baza1");
        IDatabaseConnection conn2 = manager.GetConnection("Baza2");
        IDatabaseConnection conn3 = manager.GetConnection("Baza1");
        IDatabaseConnection conn4 = manager.GetConnection("Baza2"); 
        
        conn1.AddRecord("Piotr", 25);
        Console.WriteLine("Połączenie 1 - Dodano rekord do Baza1: Piotr");

        conn2.AddRecord("Anna", 30); 
        Console.WriteLine("Połączenie 2 - Dodano rekord do Baza2: Anna");

        Console.WriteLine("Połączenie 3 - Pobranie rekordu z Baza1 o ID 1:");
        Console.WriteLine(conn3.GetRecord(1).ToString());


        Console.WriteLine("Połączenie 4 - Pobranie rekordu z Baza2 o ID 1:");
        Console.WriteLine(conn4.GetRecord(1).ToString());


        Console.WriteLine("Hash code conn1: " + conn1.GetHashCode());
        Console.WriteLine("Hash code conn2: " + conn2.GetHashCode());
        Console.WriteLine("Hash code conn3: " + conn3.GetHashCode());
        Console.WriteLine("Hash code conn4: " + conn4.GetHashCode());

        Console.WriteLine("Czy conn1 i conn3 to ten sam obiekt? " + (conn1.GetHashCode() == conn3.GetHashCode()));
        Console.WriteLine("Czy conn2 i conn4 to ten sam obiekt? " + (conn2.GetHashCode() == conn4.GetHashCode()));
    }*/
}
