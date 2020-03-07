/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using Moq;
using Foundry.Orders.Data;
using Foundry.Orders.Data.Entities;
using Foundry.Orders.ViewModels;
using Stack;
using Stack.Http.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Foundry.Orders.Tests
{
    [Collection("AutoMapper")]
    public class OrderServiceTests : ServiceTest
    {
        async Task<OrderEdit> Stub(OrdersDbContext db)
        {
            var audience = await db.Audiences.Include(a => a.AudienceItems).FirstOrDefaultAsync();
            var branch = await db.Branches.FirstOrDefaultAsync();
            var rank = await db.Ranks.Where(r => r.BranchId == branch.Id).LastOrDefaultAsync();
            var classification = await db.Classifications.FirstOrDefaultAsync();
            var contentType = await db.ContentTypes.FirstOrDefaultAsync();
            var eventType = await db.EventTypes.FirstOrDefaultAsync();

            var create = new OrderEdit
            {
                Objectives = Guid.NewGuid().ToString(),
                AudienceId = audience.Id,
                BranchId = branch.Id,
                RankId = rank.Id,
                ClassificationId = classification.Id,
                ContentTypeId = contentType.Id,
                EventTypeId = eventType.Id
            };

            foreach (var x in audience.AudienceItems) { create.AudienceItems.Add(x.Id); }

            return create;
        }

        [Fact]
        public async Task GetAll_ReturnsResultsWithNoFilter()
        {
            using (var context = new TestContext())
            {
                var os = context.CreateOrderService();

                await os.Add(new OrderEdit { Description = "test 1" });
                await os.Add(new OrderEdit { Description = "test 2" });

                var all = await os.GetAll();

                Assert.Equal(2, all.Total);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsResultsMatchingTerm()
        {
            using (var context = new TestContext())
            {
                var os = context.CreateOrderService();

                await os.Add(new OrderEdit { Description = "test" });
                await os.Add(new OrderEdit { Description = "nope" });

                var all = await os.GetAll(new OrderDataFilter { Term = "test" });

                Assert.Equal(1, all.Total);
            }
        }

        [Fact]
        public async Task GetById_ReturnsResultWithValidId()
        {
            using (var context = new TestContext())
            {
                var os = context.CreateOrderService();
                var description = Guid.NewGuid().ToString();
                var order = await os.Add(new OrderEdit { Description = description });
                var found = await os.GetById(order.Id);
                Assert.NotNull(found);
            }
        }

        [Fact]
        public async Task GetById_DoesNotReturnInvalidId()
        {
            using (var context = new TestContext())
            {
                var os = context.CreateOrderService();
                var description = Guid.NewGuid().ToString();
                var order = await os.Add(new OrderEdit { Description = description });

                await Assert.ThrowsAsync<EntityNotFoundException>(() => os.GetById(order.Id + 1));
            }
        }

        [Fact]
        public async Task Add_CreatesOrderSuccessufully()
        {
            using (var context = new TestContext())
            {
                var os = context.CreateOrderService();
                var model = new Mock<OrderEdit>();
                model.SetupAllProperties();
                var order = await os.Add(model.Object);
                Assert.NotNull(order);
            }
        }

        [Fact]
        public async Task DatabaseSeeded()
        {
            using (var context = new TestContext())
            {
                var os = context.CreateOrderService();

                var audience = await context.DbContext.Audiences.Include(a => a.AudienceItems).FirstAsync();
                var assessmentTypes = await context.DbContext.AssessmentTypes.ToListAsync();
                var embeddedTeams = await context.DbContext.EmbeddedTeams.ToListAsync();

                Assert.NotNull(audience);
                Assert.True(audience.AudienceItems.Any());
                Assert.True(assessmentTypes.Any());
                Assert.True(embeddedTeams.Any());
            }
        }

        [Fact]
        public async Task Add_CreatesOrderWithOneToOneSuccessufully()
        {
            using (var context = new TestContext())
            {
                var os = context.CreateOrderService();

                var audience = await context.DbContext.Audiences.Include(a => a.AudienceItems).FirstOrDefaultAsync();
                var branch = await context.DbContext.Branches.FirstOrDefaultAsync();
                var rank = await context.DbContext.Ranks.Where(r => r.BranchId == branch.Id).LastOrDefaultAsync();
                var classification = await context.DbContext.Classifications.FirstOrDefaultAsync();
                var contentType = await context.DbContext.ContentTypes.FirstOrDefaultAsync();
                var eventType = await context.DbContext.EventTypes.FirstOrDefaultAsync();
                var facility = await context.DbContext.Facilities.FirstOrDefaultAsync();
                var producer = await context.DbContext.Producers.FirstOrDefaultAsync();

                Assert.NotNull(audience);
                Assert.True(audience.AudienceItems.Any());
                Assert.NotNull(branch);
                Assert.NotNull(rank);
                Assert.NotNull(classification);
                Assert.NotNull(contentType);
                Assert.NotNull(eventType);
                Assert.NotNull(facility);
                Assert.NotNull(producer);

                var create = new OrderEdit {
                    Objectives = Guid.NewGuid().ToString(),
                    AudienceId = audience.Id,
                    BranchId = branch.Id,
                    RankId = rank.Id,
                    ClassificationId = classification.Id,
                    ContentTypeId = contentType.Id,
                    EventTypeId = eventType.Id,
                    FacilityId = facility.Id,
                    ProducerId = producer.Id
                };

                foreach (var x in audience.AudienceItems) { create.AudienceItems.Add(x.Id); }

                var order = await os.Add(create);

                Assert.Equal(audience.Id, order.AudienceId);
                Assert.Equal(audience.AudienceItems.Count(), order.AudienceItems.Count());
                Assert.Equal(branch.Id, order.BranchId);
                Assert.Equal(rank.Id, order.RankId);
                Assert.Equal(classification.Id, order.ClassificationId);
                Assert.Equal(contentType.Id, order.ContentTypeId);
                Assert.Equal(eventType.Id, order.EventTypeId);
                Assert.Equal(facility.Id, order.FacilityId);
                Assert.Equal(producer.Id, order.ProducerId);
            }
        }

        [Fact]
        public async Task Add_CreatesOrderWithManyToManySuccessufully()
        {
            using (var context = new TestContext())
            {
                var os = context.CreateOrderService();

                var assessmentTypes = await context.DbContext.AssessmentTypes.ToListAsync();
                var embeddedTeams = await context.DbContext.EmbeddedTeams.ToListAsync();
                var operatingSystemTypes = await context.DbContext.OperatingSystemTypes.ToListAsync();
                var securityTools = await context.DbContext.SecurityTools.ToListAsync();
                var services = await context.DbContext.Services.ToListAsync();
                var simulators = await context.DbContext.Simulators.ToListAsync();
                var supports = await context.DbContext.Supports.ToListAsync();
                var threats = await context.DbContext.Threats.ToListAsync();
                var terrains = await context.DbContext.Terrains.ToListAsync();

                var create = new OrderEdit { Objectives = Guid.NewGuid().ToString() };

                foreach (var x in assessmentTypes) { create.AssessmentTypes.Add(x.Id); }
                foreach (var x in embeddedTeams) { create.EmbeddedTeams.Add(x.Id); }
                foreach (var x in operatingSystemTypes) { create.OperatingSystemTypes.Add(x.Id); }
                foreach (var x in securityTools) { create.SecurityTools.Add(x.Id); }
                foreach (var x in services) { create.Services.Add(x.Id); }
                foreach (var x in simulators) { create.Simulators.Add(x.Id); }
                foreach (var x in supports) { create.Supports.Add(x.Id); }
                foreach (var x in terrains) { create.Terrains.Add(x.Id); }
                foreach (var x in threats) { create.Threats.Add(x.Id); }

                var order = await os.Add(create);

                Assert.Equal(assessmentTypes.Count(), order.AssessmentTypes.Count());
                Assert.Equal(embeddedTeams.Count(), order.EmbeddedTeams.Count());
                Assert.Equal(operatingSystemTypes.Count(), order.OperatingSystemTypes.Count());
                Assert.Equal(securityTools.Count(), order.SecurityTools.Count());
                Assert.Equal(services.Count(), order.Services.Count());
                Assert.Equal(simulators.Count(), order.Simulators.Count());
                Assert.Equal(supports.Count(), order.Supports.Count());
                Assert.Equal(terrains.Count(), order.Terrains.Count());
                Assert.Equal(threats.Count(), order.Threats.Count());
            }
        }

        [Fact]
        public async Task Update_ModifiesOrderWithBasicChangesSuccessufully()
        {
            using (var context = new TestContext())
            {
                var os = context.CreateOrderService();
                var description = Guid.NewGuid().ToString();

                var model = new OrderEdit();

                var order = await os.Add(model);

                model.Id = order.Id;
                model.Description = description;
                await os.Update(order.Id, model);

                var updated = await os.GetById(order.Id);

                Assert.Equal(description, updated.Description);
            }
        }

        [Fact]
        public async Task Update_ModifiesOrderWithRelationChangesSuccessufully()
        {
            using (var context = new TestContext())
            {
                var os = context.CreateOrderService();
                var db = context.DbContext;

                var edit = await Stub(context.DbContext);

                var order = await os.Add(edit);

                var audience = await db.Audiences.Include(a => a.AudienceItems).FirstOrDefaultAsync();
                var branch = await db.Branches.FirstOrDefaultAsync();
                var rank = await db.Ranks.Where(r => r.BranchId == branch.Id).LastOrDefaultAsync();
                var classification = await db.Classifications.FirstOrDefaultAsync();
                var contentType = await db.ContentTypes.FirstOrDefaultAsync();
                var eventType = await db.EventTypes.FirstOrDefaultAsync();

                Assert.Equal(audience.Id, order.AudienceId);
                Assert.Equal(branch.Id, order.BranchId);
                Assert.Equal(rank.Id, order.RankId);
                Assert.Equal(classification.Id, order.ClassificationId);
                Assert.Equal(contentType.Id, order.ContentTypeId);
                Assert.Equal(eventType.Id, order.EventTypeId);

                var update = AutoMapper.Mapper.Map<OrderEdit>(await os.OrderRepository.GetById(order.Id));

                audience = await context.DbContext.Audiences.Include(a => a.AudienceItems).LastOrDefaultAsync();
                branch = await context.DbContext.Branches.Skip(1).Take(1).FirstOrDefaultAsync();
                rank = await context.DbContext.Ranks.Where(r => r.BranchId == branch.Id).LastOrDefaultAsync();
                classification = await context.DbContext.Classifications.Skip(1).Take(1).FirstOrDefaultAsync();
                contentType = await context.DbContext.ContentTypes.Skip(1).Take(1).FirstOrDefaultAsync();
                eventType = await context.DbContext.EventTypes.Skip(1).Take(1).FirstOrDefaultAsync();

                update.AudienceId = audience.Id;
                update.BranchId = branch.Id;
                update.RankId = rank.Id;
                update.ClassificationId = classification.Id;
                update.ContentTypeId = contentType.Id;
                update.EventTypeId = eventType.Id;

                order = await os.Update(order.Id, update);

                Assert.Equal(audience.Id, order.AudienceId);
                Assert.Equal(branch.Id, order.BranchId);
                Assert.Equal(rank.Id, order.RankId);
                Assert.Equal(classification.Id, order.ClassificationId);
                Assert.Equal(contentType.Id, order.ContentTypeId);
                Assert.Equal(eventType.Id, order.EventTypeId);
            }
        }

        [Fact]
        public async Task SetStatus_ChangesStatusSuccessfully()
        {
            using (var context = new TestContext(true, true))
            {
                var os = context.CreateOrderService();
                var description = Guid.NewGuid().ToString();

                var model = await Stub(context.DbContext);
                var order = await os.Add(model);

                Assert.Equal(OrderStatus.Draft, order.Status);

                await os.SetStatus(order.Id, OrderStatus.InProgress);

                var updated = await os.GetById(order.Id);

                Assert.Equal(OrderStatus.InProgress, updated.Status);
            }
        }
    }
}
