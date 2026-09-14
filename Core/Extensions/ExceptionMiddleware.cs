using System;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Core.Utilities.Messages;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;

namespace Core.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;


        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(httpContext, e);
            }
        }


        private async Task HandleExceptionAsync(HttpContext httpContext, Exception e)
        {
            httpContext.Response.ContentType = "application/json";
            string message;
            
            // HATA AYIKLAMA İÇİN LOG EKLENDİ (Console log for dev)
            Console.WriteLine($"[Global Exception]: {e}");

            // Database constraint errors - En önce kontrol et
            if (e is DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? "";
                
                // PostgreSQL Error Code kontrolü (23505: Unique, 23503: Foreign Key, etc.)
                if (innerMessage.Contains("23505"))
                {
                    // Field adını çıkar
                    var fieldName = ExtractFieldNameFromConstraint(innerMessage);
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        message = $"Bu {fieldName} değeri zaten mevcut. Lütfen farklı bir {fieldName} girin.";
                    }
                    else
                    {
                        message = "errors.db.duplicateRecord";
                    }
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else if (innerMessage.Contains("23503"))
                {
                    message = "errors.db.foreignKeyConstraint";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else if (innerMessage.Contains("23502"))
                {
                    message = "errors.db.notNullConstraint";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else if (innerMessage.Contains("23514"))
                {
                    message = "errors.db.checkConstraint";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                // Fallback: String içerik kontrolü
                else if (innerMessage.ToLower().Contains("unique") || innerMessage.ToLower().Contains("duplicate"))
                {
                    message = "errors.db.duplicateRecord";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else if (innerMessage.ToLower().Contains("foreign key") || innerMessage.ToLower().Contains("constraint"))
                {
                    message = "errors.db.foreignKeyConstraint";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    message = "errors.db.generic";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            else if (e is ValidationException validationException)
            {
                message = string.Join(" | ", validationException.Errors.Select(error => error.ErrorMessage));
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (e is ApplicationException)
            {
                message = e.Message;
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (e is UnauthorizedAccessException)
            {
                message = "errors.unauthorized";
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else if (e is SecurityException)
            {
                message = "errors.unauthorized";
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else if (e is NotSupportedException)
            {
                message = e.Message;
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                message = ExceptionMessage.InternalServerError;
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            
            var result = JsonSerializer.Serialize(new { message });
            await httpContext.Response.WriteAsync(result);
        }
        
        // Field adını constraint message'dan çıkar
        private string ExtractFieldNameFromConstraint(string constraintMessage)
        {
            try
            {
                // PostgreSQL index formatı: IX_table_field
                if (constraintMessage.Contains("IX_"))
                {
                    var startIndex = constraintMessage.IndexOf("IX_") + 3; // "IX_" sonrası
                    var endIndex = constraintMessage.IndexOf("\"", startIndex);
                    if (endIndex == -1) endIndex = constraintMessage.Length;
                    
                    var indexName = constraintMessage.Substring(startIndex, endIndex - startIndex);
                    
                    if (indexName.Contains("_"))
                    {
                        var parts = indexName.Split('_');
                        if (parts.Length >= 2)
                        {
                            return parts[parts.Length - 1]; // Son kısım field adı
                        }
                    }
                }
                
                // Alternatif Türkçe mesaj kontrolleri
                if (constraintMessage.Contains("tekil kısıtlamasını ihlal"))
                {
                    if (constraintMessage.Contains("code")) return "kod";
                    if (constraintMessage.Contains("name")) return "isim";
                    if (constraintMessage.Contains("email")) return "e-posta";
                    if (constraintMessage.Contains("phone")) return "telefon";
                }
                
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}