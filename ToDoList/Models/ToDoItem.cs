namespace ToDoList.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string File { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
