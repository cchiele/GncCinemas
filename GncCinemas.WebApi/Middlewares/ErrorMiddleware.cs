using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GncCinemas.WebApi.Middlewares
{
    public static class SerializerSettings
    {
        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    // RFC 7807 - Problem Details for HTTP APIs
    // https://datatracker.ietf.org/doc/html/rfc7807
    public class ProblemDetails
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("detail")]
        public string Detail { get; set; }
        [JsonPropertyName("instance")]
        public string Instance { get; set; }
    }

    public class ErrorMiddleware
    {
        private const string _responseContentType = "application/problem+json";
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorMiddleware> _logger;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // TODO: Eu acredito que nem toda a exception deve gear um retorno 500, por isso coloquei TRY. Para esta aplicação, ValidationException significa erro nas regras de negócio então retorno 400.
            try
            {
                await _next(context);
            }
            catch (ValidationException  ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await HandleUpdateConcurrencyAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Internal Server Error",
                Instance = context.Request.HttpContext.Request.Path,
                Status = (int)HttpStatusCode.InternalServerError
            };

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                problemDetails.Detail = $"{ex.Message} {ex.StackTrace} {ex?.InnerException?.Message} {ex?.InnerException?.StackTrace}";
            }
            else
            {
                problemDetails.Detail = "An internal server error has occurred.";
            }

            _logger.LogError(problemDetails.Title);
            _logger.LogError("Instance: {instance}", problemDetails.Instance);
            _logger.LogError("Status {status}", problemDetails.Status);
            _logger.LogError("Detail: {Message}", $"{ex.Message} {ex?.InnerException?.Message}");
            _logger.LogError("StackTrace: {Trace}", $"{ex.StackTrace} {ex?.InnerException?.StackTrace}");

            var result = JsonConvert.SerializeObject(problemDetails, SerializerSettings.JsonSerializerSettings);

            context.Response.StatusCode = problemDetails.Status;
            context.Response.ContentType = _responseContentType;

            return context.Response.WriteAsync(result);
        }

        private Task HandleValidationExceptionAsync(HttpContext context, Exception ex)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Requisição inválida",
                Instance = context.Request.HttpContext.Request.Path,
                Status = (int)HttpStatusCode.BadRequest,
                Detail = ex.Message
            };

            _logger.LogError(problemDetails.Title);
            _logger.LogError("Instance: {instance}", problemDetails.Instance);
            _logger.LogError("Status {status}", problemDetails.Status);
            _logger.LogError("Detail: {Message}", problemDetails.Detail);

            var result = JsonConvert.SerializeObject(problemDetails, SerializerSettings.JsonSerializerSettings);

            context.Response.StatusCode = problemDetails.Status;
            context.Response.ContentType = _responseContentType;

            return context.Response.WriteAsync(result);
        }

        private Task HandleUpdateConcurrencyAsync(HttpContext context, Exception ex)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Conflito na atualização de recurso",
                Instance = context.Request.HttpContext.Request.Path,
                Status = (int)HttpStatusCode.Conflict
            };

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                problemDetails.Detail = $"{ex.Message} {ex.StackTrace} {ex?.InnerException?.Message} {ex?.InnerException?.StackTrace}";
            }
            else
            {
                problemDetails.Detail = "A solicitação não pode ser concluída, pois o recurso solicitado teve seu estado modificado. Carregue os dados do recurso e tente atualizar novamente.";
            }

            _logger.LogError(problemDetails.Title);
            _logger.LogError("Instance: {instance}", problemDetails.Instance);
            _logger.LogError("Status {status}", problemDetails.Status);
            _logger.LogError("Detail: {Message}", problemDetails.Detail);

            var result = JsonConvert.SerializeObject(problemDetails, SerializerSettings.JsonSerializerSettings);

            context.Response.StatusCode = problemDetails.Status;
            context.Response.ContentType = _responseContentType;

            return context.Response.WriteAsync(result);
        }
    }
}
