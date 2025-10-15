using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace Job_Tracker.API;

public class GetAllDTO
{
    public int id {get; set; }
    public string Title { get; set; }
    public string? CompanyName { get; set; }
    public string Link { get; set; }
}
public class GetAll
{
    private readonly ILogger<GetAll> _logger;

    public GetAll(ILogger<GetAll> logger)
    {
        _logger = logger;
    }

    [Function("GetAll")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        var connect = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=JobTracker;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        var jobList = new List<GetAllDTO>();
        using (var sql = new SqlConnection(connect))
        {
            sql.Open();
            var select = "SELECT * FROM [dbo].[JobTrackerJobs]";

            using (SqlCommand command = new SqlCommand(select, sql))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var job = new GetAllDTO
                        {
                            id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            CompanyName = reader.IsDBNull(reader.GetOrdinal("CompanyName")) ? null : reader.GetString(reader.GetOrdinal("CompanyName")),
                            Link = reader.GetString(reader.GetOrdinal("Link"))
                        };

                        jobList.Add(job);
                    }
                    return new OkObjectResult(jobList);
                }
            }
        }
    }
}