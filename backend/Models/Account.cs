namespace backend.Models

{
    public class Account
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string Email { get; set; }
        public List<TodoItem>? TodoItems { get; set; }
    }
}