using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Core.Utilities.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Core.Aspects.Autofac.Logging
{
    /// <summary>
    /// LogAspect
    /// </summary>
    public class LogAspect : MethodInterception
    {
        private readonly LoggerServiceBase _loggerServiceBase;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogAspect(Type loggerService)
        {
            if (loggerService.BaseType != typeof(LoggerServiceBase))
            {
                throw new ArgumentException(AspectMessages.WrongLoggerType);
            }

            _loggerServiceBase = (LoggerServiceBase)ServiceTool.ServiceProvider.GetService(loggerService);
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
        }

        protected override void OnBefore(IInvocation invocation)
        {
            _loggerServiceBase?.Info(GetLogDetail(invocation));            
        }

        private string GetLogDetail(IInvocation invocation)
        {
            var logParameters = new List<LogParameter>();
            for (var i = 0; i < invocation.Arguments.Length; i++)
            {
                var arg = invocation.Arguments[i];
                if (arg is Microsoft.AspNetCore.Http.IFormFile file)
                {
                    arg = $"[IFormFile: {file.FileName}]";
                }
                else if (arg is System.IO.Stream)
                {
                    arg = "[Stream]";
                }

                logParameters.Add(new LogParameter
                {
                    Name = invocation.GetConcreteMethod().GetParameters()[i].Name,
                    Value = arg,
                    Type = invocation.Arguments[i]?.GetType().Name,
                });
            }

            var logDetail = new LogDetail
            {
                MethodName = invocation.Method.Name,
                Parameters = logParameters,
                User = (_httpContextAccessor.HttpContext == null ||
                        _httpContextAccessor.HttpContext.User.Identity.Name == null)
                    ? "?"
                    : _httpContextAccessor.HttpContext.User.Identity.Name
            };

            var serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Error = (sender, args) => { args.ErrorContext.Handled = true; }
            };
            return JsonConvert.SerializeObject(logDetail, serializerSettings);
        }
    }
}