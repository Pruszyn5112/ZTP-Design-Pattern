

abstract class Cell
{
    public abstract object Value { get; set; }

    public override string ToString()
    {
        return Value.ToString().PadRight(15);
    }
}

class TextCell : Cell
{
    public override object Value { get; set; }

    public TextCell(string value)
    {
        Value = value;
    }
}

class NumberCell : Cell
{
    public override object Value { get; set; }

    public NumberCell(int value)
    {
        Value = value;
    }
}

class BooleanCell : Cell
{
    public override object Value { get; set; }

    public BooleanCell(bool value)
    {
        Value = value;
    }
}

abstract class Header
{
    public string Name { get; }

    protected Header(string name)
    {
        Name = name;
    }

    // Komorki
    public abstract Cell CreateCell(object value);
    public abstract Cell CreateDefaultCell();
}

class TextHeader : Header
{
    public TextHeader(string name) : base(name) { }

    public override Cell CreateCell(object value)
    {
        return new TextCell(value.ToString());
    }

    public override Cell CreateDefaultCell()
    {
        return new TextCell("");
    }
}

class NumberHeader : Header
{
    public NumberHeader(string name) : base(name) { }

    public override Cell CreateCell(object value)
    {
        return new NumberCell(Convert.ToInt32(value));
    }

    public override Cell CreateDefaultCell()
    {
        return new NumberCell(0);
    }
}

class BooleanHeader : Header
{
    public BooleanHeader(string name) : base(name) { }

    public override Cell CreateCell(object value)
    {
        return new BooleanCell(Convert.ToBoolean(value));
    }

    public override Cell CreateDefaultCell()
    {
        return new BooleanCell(false);
    }
}

class Table
{
    private readonly List<Header> headers;
    private readonly List<List<Cell>> rows;

    public Table()
    {
        headers = new List<Header>();
        rows = new List<List<Cell>>();
    }

    public void AddColumn(Header header)
    {
        headers.Add(header);

        // Defaultowe komorki do wierszy
        foreach (var row in rows)
        {
            row.Add(header.CreateDefaultCell());
        }
    }

    public void AddRow(params object[] cellValues)
    {
        if (cellValues.Length != headers.Count)
        {
            throw new ArgumentException("Liczba wartości nie zgadza się z liczbą kolumn.");
        }

        // Wypełnione wiersze
        var newRow = new List<Cell>();
        for (int i = 0; i < cellValues.Length; i++)
        {
            newRow.Add(headers[i].CreateCell(cellValues[i]));
        }

        rows.Add(newRow);
    }

    public override string ToString()
    {
        string result = "";

        // Nagłówki
        foreach (var header in headers)
        {
            result += header.Name.PadRight(15);
        }
        result += "\n";

        result += new string('-', headers.Count * 15) + "\n";
        
        // Wiersze
        foreach (var row in rows)
        {
            foreach (var cell in row)
            {
                result += cell.ToString();
            }
            result += "\n";
        }

        return result;
    }
}

// Test
class Program
{
    static void Main(string[] args)
    {
        Table table = new Table();

        table.AddColumn(new TextHeader("Name"));
        table.AddColumn(new NumberHeader("Age"));
        table.AddColumn(new BooleanHeader("Is Student"));

        table.AddRow("Alice", 30, false);
        table.AddRow("Bob", 25, true);
        table.AddRow("Charlie", 35, false);

        Console.WriteLine(table.ToString());

        Console.ReadKey();
    }
}
