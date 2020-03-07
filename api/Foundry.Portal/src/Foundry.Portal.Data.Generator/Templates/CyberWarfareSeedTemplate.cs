/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.Extensions.Logging;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Data.Generator.Models;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;

namespace Foundry.Portal.Data.Generator.Templates
{
    public class CyberWarfareSeedTemplate : SeedTemplate
    {
        public CyberWarfareSeedTemplate(CoreOptions options, SketchDbContext dbContext, ILoggerFactory loggerFactory, IValidationHandler validationHandler, AutoMapper.IMapper mapper)
            : base(options, dbContext, loggerFactory, validationHandler, mapper) { }

        public void Run()
        {
            DateTime dtNow = DateTime.UtcNow;

            var cyberTrainerElite = new ProfileSeedModel()
            {
                Name = "Cyber Trainer Elite",
                GlobalId = "b7977ce5-0a17-45e1-aa2e-55c57bfffeb6",
            };

            var cyberTrainerAdvanced = new ProfileSeedModel()
            {
                Name = "Cyber Trainer Advanced",
                GlobalId = "3269cb19-1d39-40d3-a55e-e3e9779b6e0b",
            };

            var traineeAdvanced = new ProfileSeedModel()
            {
                Name = "Trainee Advanced",
                GlobalId = "ac4d3e32-c2d6-4f99-9aef-0fcd62a568a6",
            };

            var traineeBeginner = new ProfileSeedModel()
            {
                Name = "Trainee Beginner",
                GlobalId = "1db2856b-7a3c-4b82-95d4-e41fb18de516",
            };

            // Bob Profile
            var bob = new ProfileSeedModel()
            {
                Name = "Bob K.",
                GlobalId = "9149f2ec-2e55-44f6-b92d-988ede6ca1f9",
            };

            List<ProfileSeedModel> seed = new List<ProfileSeedModel>()
            {
                cyberTrainerElite, cyberTrainerAdvanced, traineeAdvanced, traineeBeginner, bob
            };

            ProcessTemplate(seed);
        }
    }
}

