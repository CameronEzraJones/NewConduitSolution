using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Validators
{
    public class GetArticlesValidatorAttribute : TypeFilterAttribute
    {
        public GetArticlesValidatorAttribute() : base(typeof(GetArticlesValidatorImpl)) { }

        private class GetArticlesValidatorImpl : ConduitValidatorUtils, IActionFilter
        {
            private readonly ILogger<GetArticlesValidatorImpl> _logger;

            public GetArticlesValidatorImpl(ILoggerFactory loggerFactory)
            {
                _logger = loggerFactory.CreateLogger<GetArticlesValidatorImpl>();
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var validKeys = new List<string> { "tag", "author", "favorited", "offset", "limit" };
                var queries = context.HttpContext.Request.Query;
                var keys = queries.Keys;
                if(keys.Except(validKeys).ToList().Count >= 1)
                {
                    InvalidateRequest(context, "Unrecognized query", _logger, 422);
                    return;
                }
                if(keys.Contains("offset"))
                {
                    try
                    {
                        if(Convert.ToInt32(queries["offset"]) < 0)
                        {
                            throw new Exception();
                        }
                    } catch (Exception)
                    {
                        InvalidateRequest(context, "Offset must be a positive integer", _logger, 422);
                        return;
                    }
                }
                if (keys.Contains("limit"))
                {
                    try
                    {
                        if (Convert.ToInt32(queries["limit"]) < 0)
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception)
                    {
                        InvalidateRequest(context, "Limit must be a positive integer", _logger, 422);
                        return;
                    }
                }
            }
        }
    }
}
