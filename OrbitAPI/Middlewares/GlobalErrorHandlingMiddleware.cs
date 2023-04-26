using Orbit.Core.Exceptions;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OrbitAPI.Middlewares
{
	public static class GlobalErrorHandlingMiddlewareExtension
	{
		public static IApplicationBuilder UseGlobalErrorHandlingMiddleware(this IApplicationBuilder app)
		{
			app.UseMiddleware<GlobalErrorHandlingMiddleware>();

			return app;
		}

	}

	public class GlobalErrorHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		public GlobalErrorHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}
		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}
		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			HttpStatusCode status;
			var stackTrace = String.Empty;
			string message;
			var exceptionType = exception.GetType();
			if (exceptionType == typeof(BadRequestException))
			{
				message = exception.Message;
				status = HttpStatusCode.BadRequest;
				stackTrace = exception.StackTrace;
			}
			else if (exceptionType == typeof(NotFoundException))
			{
				message = exception.Message;
				status = HttpStatusCode.NotFound;
				stackTrace = exception.StackTrace;
			}
			else if (exceptionType == typeof(NotImplementedException))
			{
				status = HttpStatusCode.NotImplemented;
				message = exception.Message;
				stackTrace = exception.StackTrace;
			}
			else if (exceptionType == typeof(UnauthorizedAccessException))
			{
				status = HttpStatusCode.Unauthorized;
				message = exception.Message;
				stackTrace = exception.StackTrace;
			}
			else if (exceptionType == typeof(KeyNotFoundException))
			{
				status = HttpStatusCode.Unauthorized;
				message = exception.Message;
				stackTrace = exception.StackTrace;
			}
			else
			{
				status = HttpStatusCode.InternalServerError;
				message = exception.Message;
				stackTrace = exception.StackTrace;
			}
			var exceptionResult = new
			{
				message,
				stackTrace = Debugger.IsAttached ? stackTrace : null
			};
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)status;
			return context.Response.WriteAsJsonAsync(exceptionResult);
		}
	}
}
