using Google.Api.Gax.Grpc;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using mail.Model;
using Microsoft.Extensions.Options;

namespace mail.Service;

public class Pubsub
{
    private readonly ILogger<Pubsub> _logger;
    private readonly string _projectId;
    private readonly string _topicId;

    public Pubsub(ILogger<Pubsub> logger, IOptions<CloudSetting> config)
    {
        _logger = logger;
        _projectId = config.Value.ProjectId;
        _topicId = config.Value.TopicId;
    }
    
    public async Task<string> PublishMessageWithRetrySettingsAsync(string messageText)
    {
        var topicName = TopicName.FromProjectTopic(_projectId, _topicId);
       
        const int maxAttempts = 3;  // Retry settings control how the publisher handles retry-able failures
        const double backoffMultiplier = 1.0; // default: 1.0
        var initialBackoff = TimeSpan.FromMilliseconds(100); // default: 100 ms
        var maxBackoff = TimeSpan.FromSeconds(60); // default : 60 seconds
        var totalTimeout = TimeSpan.FromSeconds(600); // default: 600 seconds

        var publisher = await new PublisherClientBuilder
        {
            TopicName = topicName,
            ApiSettings = new PublisherServiceApiSettings
            {
                PublishSettings = CallSettings.FromRetry(RetrySettings.FromExponentialBackoff(
                        maxAttempts: maxAttempts,
                        initialBackoff: initialBackoff,
                        maxBackoff: maxBackoff,
                        backoffMultiplier: backoffMultiplier,
                        retryFilter: RetrySettings.FilterForStatusCodes(StatusCode.Unavailable)))
                    .WithTimeout(totalTimeout)
            }
        }.BuildAsync();
        var messageId = await publisher.PublishAsync(messageText);
        _logger.LogInformation("Published message {}", messageId);
        return messageId;
    }
}