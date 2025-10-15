using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace Job_Tracker.API;
public class GetDetailDTO
{
    public int id { get; set; }
    public string Title { get; set; }
    public string? CompanyName { get; set; }
    public string? Salary { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? ResumeSent { get; set; }
    public DateTime? FollowedUp { get; set; }
    public DateTime? Interview1 { get; set; }
    public DateTime? Interview2 { get; set; }
    public DateTime? Interview3 { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string Link { get; set; }
}
public class GetDetail
{
    private readonly ILogger<GetDetail> _logger;

    public GetDetail(ILogger<GetDetail> logger)
    {
        _logger = logger;
    }

    [Function("GetDetail")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "detail/{id:int}")] HttpRequest req, int id)
    {
        var connect = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=JobTracker;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        GetDetailDTO? jobDetail = null;
        using (var sql = new SqlConnection(connect))
        {
            await sql.OpenAsync();
            var select = "SELECT * FROM [dbo].[JobTrackerJobs] WHERE Id = @Id";

            using (SqlCommand command = new SqlCommand(select, sql))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        jobDetail = new GetDetailDTO
                        {
                            id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            CompanyName = reader.IsDBNull(reader.GetOrdinal("CompanyName")) ? null : reader.GetString(reader.GetOrdinal("CompanyName")),
                            Salary = reader.IsDBNull(reader.GetOrdinal("Salary")) ? null : reader.GetString(reader.GetOrdinal("Salary")),
                            Date = reader.IsDBNull(reader.GetOrdinal("Date")) ? null : reader.GetDateTime(reader.GetOrdinal("Date")),
                            ResumeSent = reader.IsDBNull(reader.GetOrdinal("ResumeSent")) ? null : reader.GetDateTime(reader.GetOrdinal("ResumeSent")),
                            FollowedUp = reader.IsDBNull(reader.GetOrdinal("FollowedUp")) ? null : reader.GetDateTime(reader.GetOrdinal("FollowedUp")),
                            Interview1 = reader.IsDBNull(reader.GetOrdinal("Interview1")) ? null : reader.GetDateTime(reader.GetOrdinal("Interview1")),
                            Interview2 = reader.IsDBNull(reader.GetOrdinal("Interview2")) ? null : reader.GetDateTime(reader.GetOrdinal("Interview2")),
                            Interview3 = reader.IsDBNull(reader.GetOrdinal("Interview3")) ? null : reader.GetDateTime(reader.GetOrdinal("Interview3")),
                            ContactName = reader.IsDBNull(reader.GetOrdinal("ContactName")) ? null : reader.GetString(reader.GetOrdinal("ContactName")),
                            ContactEmail = reader.IsDBNull(reader.GetOrdinal("ContactEmail")) ? null : reader.GetString(reader.GetOrdinal("ContactEmail")),
                            ContactPhone = reader.IsDBNull(reader.GetOrdinal("ContactPhone")) ? null : reader.GetString(reader.GetOrdinal("ContactPhone")),
                            Link = reader.GetString(reader.GetOrdinal("Link"))
                        };
                    }
                    if (jobDetail != null)
                    {
                        return new OkObjectResult(jobDetail);
                    }
                    else
                    {
                        return new NotFoundObjectResult($"No job detail found with ID: {id}");
                    }
                }
            }
        }
    }
}