using System.Text.Json.Serialization;

namespace backend.Models;

public class TodoItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required bool IsCompleted { get; set; } //defaults to false
    public int AccountId { get; set; }
    
    [JsonIgnore]
    public Account? Account { get; set; }
}