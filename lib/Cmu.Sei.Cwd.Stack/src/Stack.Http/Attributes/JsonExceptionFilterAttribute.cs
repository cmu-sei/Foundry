/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Stack.Http.Exceptions;
using Stack.Http.Options;
using System;
using System.Net;

namespace Stack.Http.Attributes
{
    /// <summary>
    /// filter
    /// </summary>
    public class JsonExceptionFilterAttribute : TypeFilterAttribute
    {
        public JsonExceptionFilterAttribute() : base(typeof(JsonExceptionFilter)) { }

        private class JsonExceptionFilter : IExceptionFilter
        {
            IHostingEnvironment _hostingEnvironment;
            ErrorHandlingOptions _errorHandlingOptions;

            public JsonExceptionFilter(IHostingEnvironment hostingEnvironment, ErrorHandlingOptions errorHandlingOptions)
            {
                _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
                _errorHandlingOptions = errorHandlingOptions ?? throw new ArgumentNullException(nameof(errorHandlingOptions));
            }

            public void OnException(ExceptionContext context)
            {
                JsonResult result = null;
                if (_hostingEnvironment.IsDevelopment() || _errorHandlingOptions.ShowDeveloperExceptions)
                {
                    result = new JsonResult(context.Exception.GetBaseException());
                }
                else
                {
                    result = new JsonResult(new
                    {
                        context.Exception.GetBaseException().Message
                    });
                }

                result.StatusCode = context.Exception.GetStatusCode();

                context.Result = result;
            }
        }
    }
}
