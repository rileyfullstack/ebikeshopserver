using System;
using System.Net;
using System.Security.Claims;
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
            var userClaims = context.User.Claims;
            var roleClaim = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

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

            HttpStatusCode httpStatusCode = (HttpStatusCode)statusCode;
            string statusDescription = httpStatusCode.ToString();

            Console.WriteLine($"Time: [{dateTime}] - Origin: {origin} - Path: {path} - Method: {method} - Status: {statusCode} ({statusDescription}) - ResTime: {responseTime}ms - Role: {roleClaim}");
            Console.ResetColor();
        }

    }
}

