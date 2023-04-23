using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillionThings
{
    public class TodoItem
    {
        public TodoItem() : this(Guid.NewGuid().ToString(), "")
        {
        }

        public TodoItem(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public string Id { get; set; }

        public string Description { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is  TodoItem item)
            {
                return item.Description == Description && item.Id == Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Description.GetHashCode();
        }
    }
}
