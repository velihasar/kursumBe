using System;
using System.Linq;
using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Core.Utilities.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Core.Aspects.Autofac.Exception
{
    /// <summary>
    /// ExceptionLogAspect
    /// </summary>
    public class ExceptionLogAspect : MethodInterception
    {
        private readonly LoggerServiceBase _loggerServiceBase;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExceptionLogAspect(Type loggerService)
        {
            if (loggerService.BaseType != typeof(LoggerServiceBase))
            {
                throw new ArgumentException(AspectMessages.WrongLoggerType);
            }

            _loggerServiceBase = (LoggerServiceBase)ServiceTool.ServiceProvider.GetService(loggerService);
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
        }

        protected override void OnException(IInvocation invocation, System.Exception e)
        {
            var logDetailWithException = GetLogDetail(invocation);

            logDetailWithException.ExceptionMessage = e is AggregateException
                ? string.Join(Environment.NewLine, (e as AggregateException).InnerExceptions.Select(x => x.Message))
                : e.ToString();

            var serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Error = (sender, args) => { args.ErrorContext.Handled = true; }
            };
            _loggerServiceBase.Error(JsonConvert.SerializeObject(logDetailWithException, serializerSettings));
        }

        private LogDetailWithException GetLogDetail(IInvocation invocation)
        {
            var logParameters = invocation.Arguments.Select((t, i) =>
            {
                object arg = t;
                if (arg is Microsoft.AspNetCore.Http.IFormFile file)
                {
                    arg = $"[IFormFile: {file.FileName}]";
                }
                else if (arg is System.IO.Stream)
                {
                    arg = "[Stream]";
                }
                return new LogParameter
                {
                    Name = invocation.GetConcreteMethod().GetParameters()[i].Name,
                    Value = arg,
                    Type = t?.GetType().Name
                };
            }).ToList();

            var logDetailWithException = new LogDetailWithException
            {
                MethodName = invocation.Method.Name,
                Parameters = logParameters,
                User = (_httpContextAccessor.HttpContext == null ||
                        _httpContextAccessor.HttpContext.User.Identity.Name == null)
                    ? "?"
                    : _httpContextAccessor.HttpContext.User.Identity.Name
            };
            return logDetailWithException;
        }
    }
}