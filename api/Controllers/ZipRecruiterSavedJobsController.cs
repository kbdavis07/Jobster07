using App.Library.ZipRecruiter.Data.Contexts;
using App.Library.ZipRecruiter.Data.Entities;
using App.Library.ZipRecruiter.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static Microsoft.IO.RecyclableMemoryStreamManager;

[ApiController]
[Route("api/[controller]")]
public class ZipRecruiterSavedJobsController : ControllerBase
{
    private readonly ZipRecruiterContext context;

    public ZipRecruiterSavedJobsController(ZipRecruiterContext context)
    {
        this.context = context;
    }

    [HttpPost("bulk")]
    public async Task<ActionResult> PostBulkJobs([FromBody] BulkJobsRequest request)
    {
        if (request == null || request.Jobs == null || !request.Jobs.Any())
        {
            return BadRequest("The jobs field is required.");
        }

        var existingJobs = await context.Jobs.Include(j => j.Company).Include(j => j.Location).AsNoTracking().ToListAsync();

        var existingLocations = existingJobs.Select(j => j.Location).Distinct().ToList();
        var existingCompanies = existingJobs.Select(j => j.Company).Distinct().ToList();

        foreach (var job in request.Jobs)
        {
            var existingJob = existingJobs.FirstOrDefault(j => j.Id == job.Id);

            if (existingJob != null) { continue; }

            var existingLocation = existingLocations.FirstOrDefault(l => l.Text == job.Location.Text);
            if (existingLocation != null)
            {
                var trackedLocation = context.ChangeTracker.Entries<Location>().FirstOrDefault(e => e.Entity.Text == existingLocation.Text)?.Entity;
                if (trackedLocation != null)
                {
                    job.Location = trackedLocation;
                }
                else
                {
                    context.Attach(existingLocation);
                    job.Location = existingLocation;
                }
            }

            var existingCompany = existingCompanies.FirstOrDefault(c => c.Id == job.Company.Id);
            if (existingCompany != null)
            {
                var trackedCompany = context.ChangeTracker.Entries<Company>().FirstOrDefault(e => e.Entity.Id == existingCompany.Id)?.Entity;
                if (trackedCompany != null)
                {
                    job.Company = trackedCompany;
                }
                else
                {
                    context.Attach(existingCompany);
                    job.Company = existingCompany;
                }
            }

            try
            {
                context.Jobs.Add(job);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }

        return Ok($"Saved {request.Jobs.Count} Jobs to database.");
    }
}
