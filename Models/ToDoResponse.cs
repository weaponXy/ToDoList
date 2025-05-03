namespace ToDoList.Models
{
    public class ToDoResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public string Category { get; set; }
        public int Priority { get; set; }

    }
}
