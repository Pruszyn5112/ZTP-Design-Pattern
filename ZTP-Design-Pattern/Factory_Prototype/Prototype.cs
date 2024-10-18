using System;
using System.Collections.Generic;
using System.Text;

// Klasa reprezentująca komórkę tabeli
class Cell
{
    private string value;

    public Cell(string value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value.PadRight(15);
    }
}

// Klasa reprezentująca nagłówek kolumny w tabeli
class Header
{
    public string Name { get; }

    public Header(string name)
    {
        Name = name;
    }
}

// Klasa reprezentująca tabelę
class Table
{
    private readonly List<Header> headers;  // Lista nagłówków kolumn
    private readonly List<List<Cell>> rows; // Lista wierszy (każdy wiersz to lista komórek)

    public Table()
    {
        headers = new List<Header>();
        rows = new List<List<Cell>>();
    }

    public void AddColumn(Header header)
    {
        headers.Add(header);

        // Dodajemy puste komórki do każdego z istniejących wierszy
        foreach (var row in rows)
        {
            row.Add(new Cell(""));
        }
    }

    public void AddRow(params string[] cellValues)
    {
        if (cellValues.Length != headers.Count)
        {
            throw new ArgumentException("Liczba wartości nie zgadza się z liczbą kolumn.");
        }

        // Dodajemy wiersz wypełniony komórkami z wartością
        var newRow = new List<Cell>();
        foreach (var value in cellValues)
        {
            newRow.Add(new Cell(value));
        }

        rows.Add(newRow);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        // Dodajemy nagłówki
        foreach (var header in headers)
        {
            sb.Append(header.Name.PadRight(15));
        }
        sb.AppendLine();

        // Dodajemy separator
        sb.AppendLine(new string('-', headers.Count * 15));

        // Dodajemy wiersze
        foreach (var row in rows)
        {
            foreach (var cell in row)
            {
                sb.Append(cell.ToString());
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Tworzymy nową tabelę
        Table table = new Table();

        // Dodajemy kolumny
        table.AddColumn(new Header("Name"));
        table.AddColumn(new Header("Age"));
        table.AddColumn(new Header("Is Student"));

        // Dodajemy wiersze
        table.AddRow("Alice", "30", "False");
        table.AddRow("Bob", "25", "True");
        table.AddRow("Charlie", "35", "False");

        // Wyświetlamy tabelę
        Console.WriteLine(table.ToString());

        Console.ReadKey();
    }
}
