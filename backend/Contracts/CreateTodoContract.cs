namespace backend.Contracts;

public class CreateTodoContract
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required int AccountId { get; set; }
}