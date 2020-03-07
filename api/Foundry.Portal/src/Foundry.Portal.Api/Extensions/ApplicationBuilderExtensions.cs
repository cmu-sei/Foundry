/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Foundry.Portal.Messages;

namespace Foundry.Portal.Api
{
    /// <summary>
    /// extensions to the IApplicationBuilder class
    /// </summary>
    public static class ApplicationBuilderExtentions
    {
        /// <summary>
        /// the message consumer
        /// </summary>
        public static IMessageConsumer MessageConsumer { get; set; }

        /// <summary>
        /// binds Start() and Stop() events for the MessageConsumer
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMessageConsumer(this IApplicationBuilder app)
        {
            MessageConsumer = app.ApplicationServices.GetService<IMessageConsumer>();
            var applicationLifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            applicationLifetime.ApplicationStarted.Register(OnStarted);
            applicationLifetime.ApplicationStopping.Register(OnStopping);

            return app;
        }

        static void OnStarted()
        {
            MessageConsumer.Start();
        }

        static void OnStopping()
        {
            MessageConsumer.Stop();
        }
    }
}

