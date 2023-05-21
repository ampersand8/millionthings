﻿using System.Text.Json.Serialization;

namespace MillionThings.Core;

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

    public string Id { get; init; }

    public string Description { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TodoStatus Status { get; init; }

    public override bool Equals(object? obj)
    {
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
