using System.Text.Json;

namespace SocialNetworkBe.Middlewares
{
    public class ValidationErrorMiddleware
    {
        private readonly RequestDelegate _next;
        public ValidationErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBody = context.Response.Body;
            using var newBody = new MemoryStream();
            context.Response.Body = newBody;

            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status400BadRequest)
            {
                // Đọc nội dung trong response
                newBody.Seek(0, SeekOrigin.Begin);
                var bodyText = await new StreamReader(newBody).ReadToEndAsync();

                if (bodyText.Contains("errors"))
                {
                    var json = JsonDocument.Parse(bodyText);
                    var errors = json.RootElement.GetProperty("errors");
                    var messages = errors
                        .EnumerateObject() // duyệt qua các fields
                        .SelectMany(p => p.Value.EnumerateArray()) // duyệt qua danh sách lỗi trong mỗi field
                        .Select(v => v.GetString()) // lấy từng lỗi
                        .ToList();

                    var result = new
                    {
                        message = string.Join(";", messages)
                    };

                    context.Response.ContentType = "application/json";
                    context.Response.Body = originalBody;
                    await context.Response.WriteAsJsonAsync(result);
                    return;
                }
            }

            newBody.Seek(0, SeekOrigin.Begin);
            await newBody.CopyToAsync(originalBody);
        }
    }
}
