/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Mvc;
using Foundry.Analytics.Identity;
using Foundry.Analytics.Options;
using Foundry.Analytics.ViewModels;
using Foundry.Analytics.xApi;
using Stack.Data.Options;
using Stack.Http.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using AuthorizationOptions = Stack.Http.Options.AuthorizationOptions;

namespace Foundry.Analytics.Controllers
{
    /// <summary>
    /// lrs api endpoints
    /// </summary>
    public class ConfigurationController : StackController
    {
        AuthorizationOptions _authorizationOptions;
        DatabaseOptions _databaseOptions;
        LearningRecordStoreOptions _learningRecordStoreOptions;
        IntegrationsOptions _integrationsOptions;

        string PlatformUrl
        {
            get
            {
                var request = HttpContext.Request;
                return string.Format("{0}://{1}{2}{3}", request.Scheme, request.Host, request.PathBase, request.Path);
            }
        }

        /// <summary>
        /// creates an instance of LrsController
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="lrsService"></param>
        public ConfigurationController(
            DatabaseOptions databaseOptions,
            AuthorizationOptions authorizationOptions,
            LearningRecordStoreOptions learningRecordStoreOptions,
            IntegrationsOptions integrationsOptions,
            SketchIdentityResolver identityResolver)
            : base(identityResolver)
        {
            _authorizationOptions = authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions));
            _databaseOptions = databaseOptions ?? throw new ArgumentNullException(nameof(databaseOptions));
            _learningRecordStoreOptions = learningRecordStoreOptions ?? throw new ArgumentNullException(nameof(learningRecordStoreOptions));
            _integrationsOptions = integrationsOptions ?? throw new ArgumentNullException(nameof(integrationsOptions));
        }

        /// <summary>
        /// get api configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/configuration")]
        [ProducesResponseType(typeof(Status), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetConfiguration()
        {
            var items = new List<ConfigurationItem>();

            items.Add(new ConfigurationItem("Database", new Dictionary<string, object> {
                { "Provider", _databaseOptions.Provider },
                { "Auto Migrate", _databaseOptions.AutoMigrate },
                { "Dev Mode Recreate", _databaseOptions.DevModeRecreate }
            }));

            items.Add(new ConfigurationItem("Learning Record Store", new Dictionary<string, object> {
                { "AccountName", _learningRecordStoreOptions.AccountName },
                { "Username", _learningRecordStoreOptions.Username },
                { "Password", _learningRecordStoreOptions.Password },
                { "Uri", _learningRecordStoreOptions.Uri }
            }));

            items.Add(new ConfigurationItem("Integrations", new Dictionary<string, object> {
                { "ExerciseLeaderboardUrl", _integrationsOptions.ExerciseLeaderboardUrl },
                { "STEPClientId", _integrationsOptions.STEPClientId },
                { "STEPAuthority", _integrationsOptions.STEPAuthority },
                { "STEPClientSecret", "Please see configuration settings." },
                { "STEPUserName", _integrationsOptions.STEPUserName },
                { "STEPPassword", "Please see configuration settings." },
                { "STEPScope", _integrationsOptions.STEPScope }
            }));

            return Ok(items.OrderBy(i => i.Name));
        }
    }
}
