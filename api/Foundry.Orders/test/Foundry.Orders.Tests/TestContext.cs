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
using Foundry.Orders.Data.Entities;
using Foundry.Orders.Repositories;
using Stack.Http;
using System;

namespace Foundry.Orders.Tests
{
    public class TestContext : IDisposable
    {
        public TestIdentityResolver IdentityResolver { get; set; }
        public TestDomainEventDispatcher DomainEventDispatcher { get; set; }
        Data.Entities.Profile _profile;
        public AutoMapper.IMapper Mapper { get; set; }
        public Data.OrdersDbContext DbContext { get; private set; }

        public TestContext(bool seed = true, bool useAdmin = false)
        {
            var dbContextOptions = new DbContextOptionsBuilder<Data.OrdersDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            DbContext = new Data.OrdersDbContext(dbContextOptions);

            if (seed)
            {
                Data.Seed.CyberSeedData.Seed(DbContext).Wait();
            }

            var configuration = new AutoMapper.MapperConfiguration(cfg => {
                (typeof(AutoMapper.Profile)).ProcessTypeOf("Foundry.Orders", (t) =>
                {
                    cfg.AddProfile(t);
                });
            });

            Mapper = new AutoMapper.Mapper(configuration);

            var user = new Profile() { GlobalId = Guid.NewGuid().ToString(), Name = "user" };
            var admin = new Profile() { GlobalId = Guid.NewGuid().ToString(), Name = "admin", IsAdministrator = true };

            Profile = useAdmin ? admin : user;
            DomainEventDispatcher = new TestDomainEventDispatcher();
        }

        public Profile Profile
        {
            get { return _profile; }
            set
            {
                _profile = value;
                IdentityResolver = new TestIdentityResolver(_profile);
            }
        }

        public Services.OrderService CreateOrderService()
        {
            return new Services.OrderService(DomainEventDispatcher, IdentityResolver, Mapper, new OrderRepository(DbContext));
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}

