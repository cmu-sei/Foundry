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
using Foundry.Orders.Data.Repositories;
using Stack.Patterns.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Orders.Repositories
{
    public class OrderRepository : Repository<Data.OrdersDbContext, Order>, IOrderRepository
    {
        public OrderRepository(Data.OrdersDbContext db)
            : base(db) { }

        public override IQueryable<Order> GetAll()
        {
            var query = base.GetAll()
                .Include(x => x.CreatedBy)
                .Include(x => x.Audience)
                .Include(x => x.Branch)
                .Include(x => x.Classification)
                .Include(x => x.Comments)
                .Include(x => x.EventType)
                .Include(x => x.ContentType)
                .Include(x => x.Facility)
                .Include(x => x.Files)
                .Include(x => x.Producer)
                .Include(x => x.Rank)
                .Include(x => x.OrderAssessmentTypes)
                .Include(x => x.OrderAudienceItems)
                .Include(x => x.OrderEmbeddedTeams)
                .Include(x => x.OrderOperatingSystemTypes)
                .Include(x => x.OrderSecurityTools)
                .Include(x => x.OrderServices)
                .Include(x => x.OrderSimulators)
                .Include(x => x.OrderSupports)
                .Include(x => x.OrderTerrains)
                .Include(x => x.OrderThreats);

            return query;
        }

        public async override Task<Order> GetById(int id)
        {
            return await base.GetAll()
                .Include(x => x.Audience)
                .Include(x => x.CreatedBy)
                .Include(x => x.Branch)
                .Include(x => x.Classification)
                .Include(x => x.Comments)
                .Include(x => x.EventType)
                .Include(x => x.ContentType)
                .Include(x => x.Facility)
                .Include(x => x.Files)
                .Include(x => x.Producer)
                .Include(x => x.Rank)
                .Include(x => x.OrderAssessmentTypes)
                .Include(x => x.OrderAudienceItems)
                .Include(x => x.OrderEmbeddedTeams)
                .Include(x => x.OrderOperatingSystemTypes)
                .Include(x => x.OrderSecurityTools)
                .Include(x => x.OrderServices)
                .Include(x => x.OrderSimulators)
                .Include(x => x.OrderSupports)
                .Include(x => x.OrderTerrains)
                .Include(x => x.OrderThreats)
                .Include("OrderAssessmentTypes.AssessmentType")
                .Include("OrderAudienceItems.AudienceItem")
                .Include("OrderEmbeddedTeams.EmbeddedTeam")
                .Include("OrderOperatingSystemTypes.OperatingSystemType")
                .Include("OrderSecurityTools.SecurityTool")
                .Include("OrderServices.Service")
                .Include("OrderSimulators.Simulator")
                .Include("OrderSupports.Support")
                .Include("OrderTerrains.Terrain")
                .Include("OrderThreats.Threat")
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
