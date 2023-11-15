using Google.Cloud.Tasks.V2;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace mail.Service;

public class CloudTask
{
    private readonly ILogger<CloudTask> _logger;

    public CloudTask(ILogger<CloudTask> logger)
    {
        _logger = logger;
    }

    public static string CreateTask(
        string projectId = "YOUR-PROJECT-ID",
        string location = "us-central1",
        string queue = "my-queue",
        string url = "http://example.com/taskhandler",
        string payload = "Hello World!",
        string dateTimeString = "28.9.2022 09:50:00")
    {
        CloudTasksClient client = CloudTasksClient.Create();
        QueueName parent = new QueueName(projectId, location, queue);
        DateTime date = DateTime.ParseExact("28.9.2015 05:50:00", "dd.M.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        var response = client.CreateTask(new CreateTaskRequest
        {
            Parent = parent.ToString(),
            Task = new Google.Cloud.Tasks.V2.Task
            {
                HttpRequest = new Google.Cloud.Tasks.V2.HttpRequest
                {
                    HttpMethod = Google.Cloud.Tasks.V2.HttpMethod.Post,
                    Url = url,
                    Body = ByteString.CopyFromUtf8(payload)
                },
                ScheduleTime = date.ToTimestamp()
            }
        });
        return response.Name;
    }
}