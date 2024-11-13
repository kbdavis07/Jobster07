using App.Library.ZipRecruiter.Data.Entities;

namespace App.Library.ZipRecruiter.Models.Request;
public class BulkJobsRequest
{
  public required List<Job> Jobs { get; set; }
}


