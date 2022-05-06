namespace DatabaseModels.Models;

public class Actor
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime LastUpdate { get; set; }
}