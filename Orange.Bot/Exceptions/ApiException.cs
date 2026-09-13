using System.Net;
using System.Net.Http;

namespace Orange.Bot.Exceptions;

public class ApiException : HttpRequestException
{
    public HttpStatusCode StatusCode { get; }
    public string? ResponseContent { get; }
    
    public ApiException(string message, HttpStatusCode statusCode, string? responseContent = null) 
        : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }
}