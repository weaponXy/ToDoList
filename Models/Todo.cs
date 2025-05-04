using Postgrest.Models;
using Postgrest.Attributes;

namespace ToDoList.Models
{
    public enum TodoCategory
    {
        Work,
        School,
        Personal
    }

    public enum TodoPriority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }

    [Table("todoitems")]
    public class Todo : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("iscompleted")]
        public bool IsCompleted { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("priority")]
        public int Priority { get; set; }

    }
}
