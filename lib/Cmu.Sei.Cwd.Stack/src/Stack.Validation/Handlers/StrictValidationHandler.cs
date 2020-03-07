/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Stack.Validation.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stack.Validation.Handlers
{
    /// <summary>
    /// validation handler to be used to construct <see cref="IValidationRule"/> types using an explicit set of services
    /// </summary>
    public class StrictValidationHandler : ValidationHandler, IValidationHandler
    {
        List<object> Services { get; set; } = new List<object>();

        public StrictValidationHandler(params object[] services)
        {
            Services.AddRange(services);
        }

        public override object GetService(Type type)
        {
            object service = null;

            service = Services.SingleOrDefault(s => type == s.GetType());

            if (service == null)
                service = Services.SingleOrDefault(s => type.IsAssignableFrom(s.GetType()));

            if (service == null)
                service = Services.SingleOrDefault(s => s.GetType().GetInterfaces().Contains(type));

            if (service == null)
                service = Services.SingleOrDefault(s => s.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type));

            if (service == null)
                throw new ValidationHandlerException($"Could not provide service for {type.Name}.");

            return service;
        }
    }
}

