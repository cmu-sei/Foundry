/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Stack.Validation.Handlers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Stack.Validation.Tests
{
    public class ValidationHandlerTests
    {
        [Fact]
        public void ValidateHandler_CreateHasValidation()
        {
            using (var context = new TestContext())
            {
                var rules = context.ValidationHandler.GetValidationRulesFor<HamsterCreate>();
                var ruleTypes = rules.Select(r => r.GetType());

                Assert.Contains(typeof(HamsterColorIsValid), ruleTypes);
                Assert.Contains(typeof(HamsterNameIsRequired), ruleTypes);
                Assert.Contains(typeof(HamsterNameIsUnique), ruleTypes);
            }
        }

        [Fact]
        public void ValidateHandler_UpdateHasValidation()
        {
            using (var context = new TestContext())
            {
                var rules = context.ValidationHandler.GetValidationRulesFor<HamsterUpdate>();
                var ruleTypes = rules.Select(r => r.GetType());

                Assert.Contains(typeof(HamsterColorIsValid), ruleTypes);
                Assert.Contains(typeof(HamsterNameIsRequired), ruleTypes);
                Assert.Contains(typeof(HamsterNameIsUnique), ruleTypes);
                Assert.Contains(typeof(HamsterIdIsValid), ruleTypes);
            }
        }

        [Fact]
        public async Task ValidateHandler_ThrowsIfColorIsInvalid()
        {
            using (var context = new TestContext())
            {
                var hamster = new HamsterCreate { Color = ColorType.Blue, Name = "Jessup O'McShanahan" };

                await Assert.ThrowsAsync<InvalidHamsterColorException>(async () => await context.ValidationHandler.ValidateRulesFor(hamster));
            }
        }

        [Fact]
        public async Task ValidateHandler_ThrowsIfNameIsMissing()
        {
            using (var context = new TestContext())
            {
                var hamster = new HamsterCreate { Color = ColorType.Gray };

                await Assert.ThrowsAsync<ArgumentException>(async () => await context.ValidationHandler.ValidateRulesFor(hamster));
            }
        }

        [Fact]
        public async Task ValidateHandler_ThrowsIfNameIsTaken()
        {
            using (var context = new TestContext())
            {
                var name = "Elliot Cornfelder";

                await context.DbContext.Hamsters.AddAsync(new Hamster { Color = ColorType.Gray, Name = name });
                await context.DbContext.SaveChangesAsync();

                var hamster = new HamsterCreate { Color = ColorType.Gray, Name = name };

                await Assert.ThrowsAsync<DuplicateHamsterException>(async () => await context.ValidationHandler.ValidateRulesFor(hamster));
            }
        }

        [Fact]
        public async Task ValidateHandler_ThrowsIfNotOwner()
        {
            using (var context = new TestContext())
            {
                var hamster = new Hamster { Color = ColorType.Gray, Name = "Ronald Rump", CreatedById = Guid.NewGuid().ToString() };

                await context.DbContext.Hamsters.AddAsync(hamster);
                await context.DbContext.SaveChangesAsync();

                var update = new HamsterUpdate { Id = hamster.Id, Color = ColorType.Gray, Name = hamster.Name };

                await Assert.ThrowsAsync<HamsterAccessException>(async () => await context.ValidationHandler.ValidateRulesFor(update));
            }
        }
    }
}

