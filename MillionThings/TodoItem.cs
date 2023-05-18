using System.Text.Json.Serialization;

namespace MillionThings;

public enum TodoStatus
{
    Open,
    Done
}

public class TodoItem
{
    public TodoItem() : this(Guid.NewGuid().ToString(), "", TodoStatus.Open)
    {
    }

    public TodoItem(string id, string description, TodoStatus status = TodoStatus.Open)
    {
        Id = id;
        Description = description;
        Status = status;
    }

    public string Id { get; set; }

    public string Description { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TodoStatus Status { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj is TodoItem item)
        {
            return item.Description == Description && item.Id == Id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Description.GetHashCode();
    }

    public TodoItem Finish()
    {
        return new(Id, Description, TodoStatus.Done);
    }
}
