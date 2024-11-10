using System;
using System.Collections.Generic;
using System.Linq;

public interface INewsService
{
    Response AddMessage(string title, string content);
    Response ReadMessage(int id);
    Response EditMessage(int id, string newContent);
    Response DeleteMessage(int id);
}

public class Response
{
    public string Status { get; set; }
    public string Message { get; set; }

    public Response(string status, string message)
    {
        Status = status;
        Message = message;
    }
}

public class User
{
    public string Name { get; set; }
    public UserRole Role { get; set; }

    public User(string name, UserRole role)
    {
        Name = name;
        Role = role;
    }
}

public enum UserRole
{
    Guest,
    User,
    Moderator,
    Admin
}
public class Message
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public Message(int id, string title, string content)
    {
        Id = id;
        Title = title;
        Content = content;
    }
}

public class NewsService : INewsService
{
    private List<Message> _messages;
    private int _nextId;

    public NewsService()
    {
        _messages = new List<Message>();
        _nextId = 1;
    }

    public Response AddMessage(string title, string content)
    {
        var message = new Message(_nextId++, title, content);
        _messages.Add(message);
        return new Response("Success", "Message added successfully.");
    }

    public Response ReadMessage(int id)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message != null)
        {
            return new Response("Success", $"{message.Title}: {message.Content}");
        }
        return new Response("Error", "Message not found.");
    }

    public Response EditMessage(int id, string newContent)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message == null)
        {
            return new Response("Error", "Message not found.");
        }

        message.Content = newContent;
        return new Response("Success", "Message edited successfully.");
    }

    public Response DeleteMessage(int id)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message == null)
        {
            return new Response("Error", "Message not found.");
        }

        _messages.Remove(message);
        return new Response("Success", "Message deleted successfully.");
    }
}
public class NewsServiceProxy : INewsService
{
    private readonly INewsService _newsService;
    private readonly User _user;
    private static Dictionary<int, Response> _cache = new();

    public NewsServiceProxy(INewsService newsService, User user)
    {
        _newsService = newsService;
        _user = user;
    }

    public Response AddMessage(string title, string content)
    {
        if (_user.Role < UserRole.User)
        {
            return new Response("Error", "Access denied: insufficient permissions to add a message.");
        }

        var response = _newsService.AddMessage(title, content);
        ClearCacheForMessage(-1);

        return response;
    }

    public Response ReadMessage(int id)
    {
        if (_user.Role < UserRole.Guest)
        {
            return new Response("Error", "Access denied: insufficient permissions to read a message.");
        }

        if (_cache.ContainsKey(id))
        {
            return _cache[id];
        }

        var response = _newsService.ReadMessage(id);
        if (response.Status == "Success")
        {
            _cache[id] = response;
        }

        return response;
    }

    public Response EditMessage(int id, string newContent)
    {
        if (_user.Role < UserRole.Moderator)
        {
            return new Response("Error", "Access denied: insufficient permissions to edit a message.");
        }

        var response = _newsService.EditMessage(id, newContent);
        ClearCacheForMessage(id);

        return response;
    }

    public Response DeleteMessage(int id)
    {
        if (_user.Role < UserRole.Admin)
        {
            return new Response("Error", "Access denied: insufficient permissions to delete a message.");
        }

        var response = _newsService.DeleteMessage(id);
        ClearCacheForMessage(id);

        return response;
    }

    private void ClearCacheForMessage(int id)
    {
        if (id == -1)
        {
            _cache.Clear();
        }
        else
        {
            _cache.Remove(id);
        }
    }
}


public class Program
{
    public static void Main()
    {
        var newsService = new NewsService();
        var user = new User("John", UserRole.User);
        var proxy = new NewsServiceProxy(newsService, user);

        var addResponse = proxy.AddMessage("Title", "Content");
        Console.WriteLine($"{addResponse.Status}: {addResponse.Message}");

        var readResponse = proxy.ReadMessage(1);
        Console.WriteLine($"{readResponse.Status}: {readResponse.Message}");

        var editResponse = proxy.EditMessage(1, "Updated content");
        Console.WriteLine($"{editResponse.Status}: {editResponse.Message}");

        var deleteResponse = proxy.DeleteMessage(1); // Moderator nie ma dostępu do usunięcia
        Console.WriteLine($"{deleteResponse.Status}: {deleteResponse.Message}");
    }
}
