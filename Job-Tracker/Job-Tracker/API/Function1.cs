using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Job_Tracker.API
{
    public class AddJob
    {
        private readonly ILogger<AddJob> _logger;

        public AddJob(ILogger<AddJob> logger)
        {
            _logger = logger;
        }

        public class AddJobDTO
        {
            public string Title { get; set; }
            public string? CompanyName { get; set; }
            public string? Salary { get; set; }
            public DateTime? Date { get; set; }
            public DateTime? ResumeSent { get; set; }
            public DateTime? Interview1 { get; set; }
            public DateTime? Interview2 { get; set; }
            public DateTime? Interview3 { get; set; }
            public DateTime? FollowedUp { get; set; }
            public string? ContactName { get; set; }
            public string? ContactEmail { get; set; }
            public string? ContactPhone { get; set; }
            public string Link { get; set; }
        }

        [Function("AddJob")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            // --- Temporary Debugging Code ---
            // Enable buffering so the stream can be read more than once
            req.EnableBuffering();
            // Read the body content into a string
            using var reader = new StreamReader(req.Body, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
            var requestBody = await reader.ReadToEndAsync();
            // Rewind the stream to the beginning for the deserializer to read it
            req.Body.Position = 0;
            // Now, log the raw JSON string
            _logger.LogInformation($"Raw Request Body: {requestBody}");

            // Deserialize from the string instead of the stream
            var dto = System.Text.Json.JsonSerializer.Deserialize<AddJobDTO>(requestBody);
            // You can remove req.EnableBuffering() and the stream reading/rewinding once debugging is complete
            // ------------------------------------

            var connect = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=JobTracker;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            using (var sql = new SqlConnection(connect))
            {
                sql.Open();
                var insert = "INSERT INTO [dbo].[JobTrackerJobs] (Link, Title, CompanyName, Date, Salary, ResumeSent, Interview1, Interview2, Interview3, FollowedUp, ContactName, ContactEmail, ContactPhone) VALUES (@Link, @Title, @CompanyName, @Date, @Salary, @ResumeSent, @Interview1, @Interview2, @Interview3, @FollowedUp, @ContactName, @ContactEmail, @ContactPhone)";

                using (SqlCommand command = new SqlCommand(insert, sql))
                {
                    command.Parameters.AddWithValue("@Link", dto.Link);
                    command.Parameters.AddWithValue("@Title", dto.Title);
                    command.Parameters.AddWithValue("@CompanyName", (object)dto.CompanyName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Date", (object)dto.Date ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Salary", (object)dto.Salary ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ResumeSent", (object)dto.ResumeSent ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Interview1", (object)dto.Interview1 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Interview2", (object)dto.Interview2 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Interview3", (object)dto.Interview3 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FollowedUp", (object)dto.FollowedUp ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ContactName", (object)dto.ContactName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ContactEmail", (object)dto.ContactEmail ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ContactPhone", (object)dto.ContactPhone ?? DBNull.Value);
                    try
                    {
                        await command.ExecuteNonQueryAsync();

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        throw;
                    }
                 }
            }
            return new OkObjectResult($"Company name: {dto.CompanyName} added");
        }
    }
}