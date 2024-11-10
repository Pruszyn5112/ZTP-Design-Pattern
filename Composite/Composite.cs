using System;
using System.Collections.Generic;
using System.Linq;

public interface ITaskComponent
{
    string Name { get; }
    DateTime StartDate { get; }
    DateTime EndDate { get; }
    bool IsCompleted { get; }
    bool IsLate { get; }
    void MarkAsCompleted(DateTime completionDate);
    string GetStatus();
    void Display(int indentation = 0);
}

public class Task : ITaskComponent
{
    public string Name { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public bool IsCompleted { get; private set; } = false;
    public bool IsLate { get; private set; } = false;

    public Task(string name, DateTime startDate, DateTime endDate)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void MarkAsCompleted(DateTime completionDate)
    {
        IsCompleted = true;
        IsLate = completionDate > EndDate;
    }

    public string GetStatus()
    {
        if (IsCompleted)
            return IsLate ? "[Completed Late]" : "[Completed]";
        return "[Pending]";
    }

    public void Display(int indentation = 0)
    {
        Console.WriteLine(new string(' ', indentation) + $"{Name} ({StartDate:dd.MM.yyyy} to {EndDate:dd.MM.yyyy}) - Status: {GetStatus()}");
    }
}

public class TaskGroup : ITaskComponent
{
    protected readonly List<ITaskComponent> _components = new List<ITaskComponent>();
    public string Name { get; }

    public TaskGroup(string name)
    {
        Name = name;
    }

    public void AddComponent(ITaskComponent component)
    {
        _components.Add(component);
    }

    public DateTime StartDate => _components.Min(c => c.StartDate);
    public DateTime EndDate => _components.Max(c => c.EndDate);
    public virtual bool IsCompleted => _components.All(c => c.IsCompleted);
    public bool IsLate => _components.Any(c => c.IsLate);

    public virtual void MarkAsCompleted(DateTime completionDate)
    {
        foreach (var component in _components)
        {
            if (!component.IsCompleted)
                component.MarkAsCompleted(completionDate);
        }
    }

    public string GetStatus()
    {
        return IsCompleted ? (IsLate ? "[Completed Late]" : "[Completed]") : "[Pending]";
    }

    public void Display(int indentation = 0)
    {
        Console.WriteLine(new string(' ', indentation) + $"Group: {Name} - Status: {GetStatus()}");
        foreach (var component in _components)
        {
            component.Display(indentation + 2);
        }
    }
}

public class OptionalTaskGroup : TaskGroup
{
    public OptionalTaskGroup(string name) : base(name) { }

    public override bool IsCompleted => _components.Any(c => c.IsCompleted);

    public override void MarkAsCompleted(DateTime completionDate)
    {
        if (!IsCompleted)
        {
            var incompleteTasks = _components.Where(c => !c.IsCompleted).ToList();
            if (incompleteTasks.Count > 0)
            {
                var random = new Random();
                int index = random.Next(incompleteTasks.Count);
                incompleteTasks[index].MarkAsCompleted(completionDate);
            }
        }
    }
}

public class Program
{
    public static void Main()
    {
        var task1 = new Task("1A - Implementacja algorytmu sortowania", new DateTime(2024, 10, 21), new DateTime(2024, 10, 27));
        var task2 = new Task("1B - Analiza złożoności czasowej", new DateTime(2024, 10, 24), new DateTime(2024, 10, 31));

        var optionalTask1 = new Task("Opcjonalne - Dokumentacja techniczna", new DateTime(2024, 11, 1), new DateTime(2024, 11, 5));
        var optionalTask2 = new Task("Opcjonalne - Testy jednostkowe", new DateTime(2024, 11, 3), new DateTime(2024, 11, 10));

        var mainGroup = new TaskGroup("Projekt główny");
        mainGroup.AddComponent(task1);
        mainGroup.AddComponent(task2);

        var optionalGroup = new OptionalTaskGroup("Grupa Opcjonalna");
        optionalGroup.AddComponent(optionalTask1);
        optionalGroup.AddComponent(optionalTask2);

        mainGroup.AddComponent(optionalGroup);

        task1.MarkAsCompleted(new DateTime(2024, 10, 25));
        mainGroup.MarkAsCompleted(new DateTime(2024, 11, 8));

        Console.WriteLine("Lista zadań:");
        mainGroup.Display();

        Console.WriteLine("\nPodsumowanie zadań:");
        int completedOnTime = mainGroup.GetAllTasks().Count(t => t.IsCompleted && !t.IsLate);
        int completedLate = mainGroup.GetAllTasks().Count(t => t.IsCompleted && t.IsLate);
        int pending = mainGroup.GetAllTasks().Count(t => !t.IsCompleted);
        int pendingLate = mainGroup.GetAllTasks().Count(t => !t.IsCompleted && DateTime.Now > t.EndDate);

        Console.WriteLine($"Zadania wykonane na czas: {completedOnTime}");
        Console.WriteLine($"Zadania wykonane z opóźnieniem: {completedLate}");
        Console.WriteLine($"Zadania oczekujące: {pending}");
        Console.WriteLine($"Zadania oczekujące z przekroczonym terminem: {pendingLate}");
    }
}

// Rozszerzenie do łatwego pobierania wszystkich zadań z hierarchii
public static class TaskComponentExtensions
{
    public static IEnumerable<ITaskComponent> GetAllTasks(this ITaskComponent component)
    {
        if (component is Task)
        {
            yield return component;
        }
        else if (component is TaskGroup group)
        {
            foreach (var subComponent in group._components.SelectMany(GetAllTasks))
            {
                yield return subComponent;
            }
        }
    }
}
