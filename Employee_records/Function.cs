using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Net;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Employee_records;

public class Function
{
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public static async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        DynamoDBContext dbcontext = new DynamoDBContext(client);

        if (request.RouteKey.Contains("GET /employees"))
        {
            var data = await dbcontext.ScanAsync<Employees>(default).GetRemainingAsync();
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = System.Text.Json.JsonSerializer.Serialize(data),
            };
        }
        else if (request.RouteKey.Contains("POST /employees") && request.Body != null)
        {
            context.Logger.LogLine($"Request Body: {request.Body}"); // Log the incoming JSON

            try
            {
                var employee = System.Text.Json.JsonSerializer.Deserialize<Employees>(request.Body);
                await dbcontext.SaveAsync(employee);
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Body = $"Employee with Id {employee?.Id} added successfully",
                };
            }
            catch (JsonException ex)
            {
                context.Logger.LogLine($"JSON Deserialization Error: {ex.Message}");
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Body = "Invalid JSON format",
                };
            }
        }
        else
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = "Unsupported route or missing body",
            };
        }
    }
}
