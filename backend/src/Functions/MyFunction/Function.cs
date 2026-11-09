using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MyFunction;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent envt, ILambdaContext context)
    {
        foreach (var record in envt.Records)
        {
            await ProcessRecord(record, context);
        }
    }

    private static async Task ProcessRecord(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        context.Logger.LogInformation($"MessageId={message.MessageId} Body={message.Body}");
    }
}
