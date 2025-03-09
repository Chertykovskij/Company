using Serilog.Context;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Company.WebApi.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Игнорируем Swagger (не логируем)
            if (context.Request.Path.StartsWithSegments("/swagger", StringComparison.InvariantCultureIgnoreCase))
            {
                await _next(context);
                return;
            }

            var requestId = Guid.NewGuid().ToString();

            LogContext.PushProperty("CorrelationId", requestId);

            await LogRequest(context);

            var watch = Stopwatch.StartNew();

            using (var buffer = new MemoryStream())
            {
                // Создание нового потока для чтения ответа
                var stream = context.Response.Body;
                context.Response.Body = buffer;

                // Выполняем запрос
                try
                {
                    await _next(context);
                }
                catch (Exception exception)
                {
                    int statusCode = 500;
                    string errorCode = "SERVER_ERROR";
                    string errorMessage = "Internal server error";
                    IDictionary parameters = null;

                    var errorResponse = "{\n" +
                    $"    \"Id\":  \"{requestId}\",\n" +
                    $"    \"ErrorCode\": \"{errorCode}\",\n" +
                    $"    \"ErrorMessage\": \"{errorMessage}\",\n" +
                    $"    \"Parameters\": \"{parameters}\"\n" +
                    "}";
                    var errorResponseText = Encoding.UTF8.GetBytes(errorResponse);

                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "application/json";

                    buffer.Write(errorResponseText, 0, errorResponseText.Length);
                    buffer.Seek(0, SeekOrigin.Begin);

                    _logger.LogError(exception, "Unhandled exception {ExceptionMsg}", exception.Message);
                }

                watch.Stop();

                buffer.Seek(0, SeekOrigin.Begin);
                using (var bufferReader = new StreamReader(buffer))
                {
                    string body = await bufferReader.ReadToEndAsync();

                    buffer.Seek(0, SeekOrigin.Begin);

                    await buffer.CopyToAsync(stream);
                    context.Response.Body = stream;

                    LogResponse(context, body, watch.ElapsedMilliseconds);
                }
            }
        }

        private async Task LogRequest(HttpContext context)
        {
            var request = context.Request;

            var result = new StringBuilder();

            result.AppendLine($"REQUEST {request.Method} {request.Path} {request.Protocol}");

            foreach (var header in request.Headers)
                result.Append(header.Key).Append(": ").AppendLine(string.Join("; ", header.Value));

            result.AppendLine($"Client IP: {context.Connection.RemoteIpAddress}");
            result.AppendLine(
                $"Route:{context.Request.Method} {context.Request.Host}{context.Request.Path}{context.Request.QueryString} {context.Request.Protocol}");

            string content = "";
            if (request.ContentLength != null && request.ContentLength > 0 && request.ContentType.StartsWith("application/json"))
            {
                var requestBodyStream = new MemoryStream();
                var originalRequestBody = context.Request.Body;

                await originalRequestBody.CopyToAsync(requestBodyStream);
                requestBodyStream.Seek(0, SeekOrigin.Begin);

                result.AppendLine("BODY:");

                content = await new StreamReader(requestBodyStream).ReadToEndAsync();
                result.AppendLine(content);

                requestBodyStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = requestBodyStream;
            }

            _logger.LogDebug(result.ToString());
            using (LogContext.PushProperty("request.Headers", request.Headers))
            using (LogContext.PushProperty("request.ContentType", request.ContentType))
            using (LogContext.PushProperty("request.ContentLength", request.ContentLength))
            using (LogContext.PushProperty("Body", content))
                _logger.LogDebug("* REQUEST {Method} {Path} {QueryString} {Protocol}  from {RemoteIpAddress}.",
                    request.Method,
                    request.Path.Value,
                    request.QueryString.ToString(),
                    request.Protocol,
                    context.Connection.RemoteIpAddress);
        }

        private void LogResponse(HttpContext context, string body, long elapsed)
        {
            var result = new StringBuilder();

            result.AppendLine($"RESPONSE {context.Response.StatusCode}");
            foreach (var header in context.Response.Headers)
                result.Append(header.Key).Append(": ").AppendLine(string.Join("; ", header.Value));

            result.AppendLine($"Duration: {elapsed}");

            string content = "";
            if (!string.IsNullOrEmpty(body) && context.Response.ContentType.StartsWith("application/json"))
            {
                content = body;
                result.AppendLine("BODY:").AppendLine(body);
            }

            _logger.LogInformation($"{context.Connection.RemoteIpAddress} {context.User?.Identity?.Name} {context.Request.Method} {context.Request.Path} {context.Response.StatusCode} {elapsed} msec");

            _logger.LogDebug(result.ToString());
            using (LogContext.PushProperty("response.Headers", context.Response.Headers))
            using (LogContext.PushProperty("response.ClientIP", context.Connection.RemoteIpAddress))
            using (LogContext.PushProperty("response.ContentType", context.Response.ContentType))
            using (LogContext.PushProperty("response.ContentLength", context.Response.ContentLength))
            using (LogContext.PushProperty("Body", content))
                _logger.LogDebug("* RESPONSE {Method} {Path} from {RemoteIpAddress}. Status {StatusCode} ({Duration} msec)",
                    context.Request.Method,
                    context.Request.Path,
                    context.Connection.RemoteIpAddress,
                    context.Response.StatusCode,
                    elapsed);
        }
    }

    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
