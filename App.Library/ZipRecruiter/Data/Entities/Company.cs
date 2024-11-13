namespace App.Library.ZipRecruiter.Data.Entities;
public class Company
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Logo { get; set; }
    public required string Url { get; set; }
}