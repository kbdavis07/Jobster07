namespace App.Library.ZipRecruiter.Data.Entities;
public class Job
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required Company Company { get; set; }
    public required Location Location { get; set; }
    public required string Type { get; set; }
    public List<string>? Benefits { get; set; } = null;
    public required string ClickUrl { get; set; }
}
