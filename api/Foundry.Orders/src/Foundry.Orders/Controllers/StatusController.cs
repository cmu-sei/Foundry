/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Foundry.Orders.Options;
using Foundry.Orders.ViewModels;
using Stack.Communication.Notifications;
using Stack.Data.Options;
using Stack.DomainEvents;
using Stack.Http.Identity;
using Stack.Http.Options;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Orders.Controllers
{
    /// <summary>
    /// status
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class StatusController : StackController
    {
        DatabaseOptions _databaseOptions;
        FileOptions _fileOptions;
        Stack.Http.Options.AuthorizationOptions _authorizationOptions;
        CommunicationOptions _communicationOptions;
        DomainEventDispatcherOptions _domainEventDispatcherOptions;

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="databaseOptions"></param>
        /// <param name="authorizationOptions"></param>
        /// <param name="fileOptions"></param>
        /// <param name="communicationOptions"></param>
        /// <param name="domainEventDispatcherOptions"></param>
        public StatusController(IStackIdentityResolver identityResolver,
            DatabaseOptions databaseOptions,
            Stack.Http.Options.AuthorizationOptions authorizationOptions,
            FileOptions fileOptions,
            CommunicationOptions communicationOptions,
            DomainEventDispatcherOptions domainEventDispatcherOptions)
            : base(identityResolver)
        {
            _databaseOptions = databaseOptions;
            _authorizationOptions = authorizationOptions;
            _fileOptions = fileOptions;
            _communicationOptions = communicationOptions;
            _domainEventDispatcherOptions = domainEventDispatcherOptions;
        }

        /// <summary>
        /// get api configuration
        /// </summary>
        /// <returns></returns>
        [Route("api/configuration")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ConfigurationItem>), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetConfiguration()
        {
            var items = new List<ConfigurationItem>();

            items.Add(new ConfigurationItem("Database", new Dictionary<string, object> {
                { "Provider", _databaseOptions.Provider },
                { "Auto Migrate", _databaseOptions.AutoMigrate },
                { "Dev Mode Recreate", _databaseOptions.DevModeRecreate }
            }));

            items.Add(new ConfigurationItem("Authorization", new Dictionary<string, object> {
                { "Authority", _authorizationOptions.Authority },
                { "Scope", _authorizationOptions.AuthorizationScope },
                { "Client Id", _authorizationOptions.ClientId },
                { "Client Name", _authorizationOptions.ClientName }
            }));

            items.Add(new ConfigurationItem("File", new Dictionary<string, object> {
                { "DestPath", _fileOptions.DestPath },
                { "MaxFileBytes", _fileOptions.MaxFileBytes },
                { "FileStorageUrl", _fileOptions.FileStorageUrl }
            }));

            items.Add(new ConfigurationItem("Communication", new Dictionary<string, object> {
                { "Communication Url", _communicationOptions.CommunicationUrl },
                { "Client Url", _communicationOptions.ClientUrl },
                { "Client Id", _communicationOptions.ClientId },
                { "Client Secret Set", string.IsNullOrWhiteSpace(_communicationOptions.ClientSecret) ? "No" : "Yes" }
            }));

            items.Add(new ConfigurationItem("Domain Event Dispatcher", new Dictionary<string, object> {
                { "Handler", _domainEventDispatcherOptions.Handler }
            }));

            return Ok(items.OrderBy(i => i.Name));
        }

        /// <summary>
        /// gets the status and module information for the api
        /// </summary>
        /// <returns></returns>
        [Route("api/status")]
        [HttpGet]
        [ProducesResponseType(typeof(Status), 200)]
        public IActionResult GetStatus()
        {
            var status = new Status("Foundry.Orders");
            return Ok(status);
        }
    }
}

