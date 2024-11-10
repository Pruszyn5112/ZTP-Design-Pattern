using System;
using System.Collections.Generic;

// Klasa reprezentująca punkt
class Point
{
    // Współrzędne punktu
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

// Klasa reprezentująca figurę
class Figure
{
    // Punkty, które tworzą figurę
    public List<Point> Points { get; }

    public Figure(List<Point> points)
    {
        Points = points;
    }

    public override string ToString()
    {
        return $"Figura: {string.Join(", ", Points)}";
    }
}

// Klasa reprezentująca rysunek
class Drawing
{
    // Lista figur tworzących rysunek
    public List<Figure> Figures { get; }

    public Drawing(List<Figure> figures)
    {
        Figures = figures;
    }

    public override string ToString()
    {
        return $"Rysunek: \n{string.Join("\n", Figures)}";
    }
}

// zad 1
class DrawingBuilder
{
    private List<Figure> _figures;
    private List<Point>? _currentFigurePoints = null;

    public DrawingBuilder()
    {
        _figures = new List<Figure>();
    }

    public DrawingBuilder MoveTo(int x, int y)
    {
        // nowa figura od punktu początkowego
        _currentFigurePoints = new List<Point> { new Point(x, y) };
        return this;
    }

    public DrawingBuilder LineTo(int x, int y)
    {
        // punkt do aktualnej figury
        _currentFigurePoints?.Add(new Point(x, y));
        return this;
    }

    public DrawingBuilder Close()
    {
        if (_currentFigurePoints != null && _currentFigurePoints.Count > 0)
        {
            // ostatni punkt z pierwszym
            _currentFigurePoints.Add(_currentFigurePoints[0]);
            _figures.Add(new Figure(new List<Point>(_currentFigurePoints)));
            _currentFigurePoints = null;
        }
        return this;
    }

    public Drawing Build()
    {
        // czy pozostały punkty, które nie zostały dodane jako figura
        if (_currentFigurePoints != null)
        {
            _figures.Add(new Figure(new List<Point>(_currentFigurePoints)));
            _currentFigurePoints = null;
        }
        return new Drawing(_figures);
    }
}
class Director
{
    private readonly DrawingBuilder _builder;

    public Director(DrawingBuilder builder)
    {
        _builder = builder;
    }
    public void ConstructFromString(string commands) {
        var commandList = commands.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for(int i = 0; i<commandList.Length; i++)
        {
            var command = commandList[i];
            switch (command) {
                case "M":
                    int xMove = int.Parse(commandList[++i]);
                    int yMove = int.Parse(commandList[++i]);
                    _builder.MoveTo(xMove, yMove);
                    break;
                case "L":
                    int xLine = int.Parse(commandList[++i]);
                    int yLine = int.Parse(commandList[++i]);
                    _builder.LineTo(xLine, yLine);
                    break;
                case "Z":
                    _builder.Close();
                    break;
                default:
                    Console.WriteLine("Nieznana komenda: ${command}");
                    break;
            }
        }
    }
}
    class Builder
{
    static void Main(string[] args)
    {

        // zad 1
        var drawingBuilder = new DrawingBuilder();
        var rysunek = drawingBuilder
            .MoveTo(100, 400)
            .LineTo(200, 50)
            .LineTo(450, 300)
            .LineTo(250, 250)
            .Close()
            .MoveTo(300, 350)
            .LineTo(350, 100)
            .LineTo(50, 200)
            .Build();
        Console.WriteLine(rysunek);


        // zad2
        var drawingBuilder2 = new DrawingBuilder();
        Director director = new Director(drawingBuilder2);
        director.ConstructFromString("M 100 400 L 200 50 L 450 300 L 250 250 Z M 300 350 L 350 100 L 50 200");
        Drawing drawing = drawingBuilder2.Build();
        Console.WriteLine(drawing);
    }
}
