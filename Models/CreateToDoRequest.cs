namespace ToDoList.Models
{
    public class CreateToDoRequest
    {
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public string Category { get; set; }
        public int Priority { get; set; }
    }
}
