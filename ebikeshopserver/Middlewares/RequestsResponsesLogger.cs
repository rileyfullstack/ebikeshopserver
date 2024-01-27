using System;
using Microsoft.AspNetCore.Http.Extensions;

namespace ebikeshopserver.Middlewares
{
	public class RequestsResponsesLogger
	{
        private readonly RequestDelegate _next;

        public RequestsResponsesLogger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<RequestsResponsesLogger> logger)
        {
            string origin = context.Request.GetDisplayUrl();
            string path = context.Request.Path;
            string method = context.Request.Method;
            DateTime dateTime = DateTime.Now;

            await _next(context);

            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - dateTime;
            int statusCode = context.Response.StatusCode;
            string responseTime = duration.TotalMilliseconds.ToString();

            if (statusCode >= 400)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.WriteLine($"Time: [{dateTime}] - Origin: {origin} - Path: {path} - Method: {method} - Status: {statusCode} - ResTime: {responseTime}ms");
            Console.ResetColor();
        }
    }
}

