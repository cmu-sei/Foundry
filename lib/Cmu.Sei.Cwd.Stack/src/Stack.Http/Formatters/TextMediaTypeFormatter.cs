/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Stack.Http.Formatters
{
    public class TextMediaTypeFormatter : IInputFormatter
    {
        public bool CanRead(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var contentType = context.HttpContext.Request.ContentType;
            if (contentType == null || contentType == "text/plain")
                return true;
            return false;
        }

        public async Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var request = context.HttpContext.Request;

            if (request.ContentLength == 0)
            {
                if (context.ModelType.GetTypeInfo().IsValueType)
                    return await InputFormatterResult.SuccessAsync(Activator.CreateInstance(context.ModelType));

                return await InputFormatterResult.SuccessAsync(null);
            }

            using (var reader = new StreamReader(context.HttpContext.Request.Body))
            {
                var model = reader.ReadToEnd();
                return await InputFormatterResult.SuccessAsync(model);
            }
        }
    }
}
