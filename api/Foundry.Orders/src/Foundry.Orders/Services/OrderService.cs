/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Foundry.Orders.Data.Entities;
using Foundry.Orders.Data.Repositories;
using Foundry.Orders.Notifications;
using Foundry.Orders.ViewModels;
using Stack.DomainEvents;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundry.Orders.Identity;

namespace Foundry.Orders.Services
{
    /// <summary>
    /// order service
    /// </summary>
    public class OrderService : DispatchService<Order>
    {
        public IOrderRepository OrderRepository { get; }

        ProfileIdentity ProfileIdentity
        {
            get
            {
                return Identity as ProfileIdentity;
            }
        }

        /// <summary>
        /// create an instance of the order service
        /// </summary>
        /// <param name="domainEventDispatcher"></param>
        /// <param name="identityResolver"></param>
        /// <param name="mapper"></param>
        /// <param name="orderRepository"></param>
        public OrderService(IDomainEventDispatcher domainEventDispatcher, IStackIdentityResolver identityResolver, IMapper mapper, IOrderRepository orderRepository)
            : base(domainEventDispatcher, identityResolver, mapper)
        {
            OrderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        /// <summary>
        /// get all orders
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<PagedResult<Order, OrderSummary>> GetAll(OrderDataFilter filter = null)
        {
            var query = OrderRepository.GetAll();

            if (!IsAdministrator)
            {
                query = query.Where(o => o.Status == OrderStatus.Draft && o.CreatedById == ProfileIdentity.Profile.Id || o.Status != OrderStatus.Draft);
            }

            return await PagedResult<Order, OrderSummary>(query, filter);
        }

        /// <summary>
        /// get order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OrderDetail> GetById(int id)
        {
            if (!(await Exists(id)))
                throw new EntityNotFoundException("record with id '" + id + "' was not found");

            var entity = await OrderRepository.GetById(id);

            return Mapper.Map<OrderDetail>(entity);
        }

        /// <summary>
        /// set order status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<OrderDetail> SetStatus(int id, OrderStatus status)
        {
            if (!(await Exists(id)))
                throw new EntityNotFoundException("record with id '" + id + "' was not found");

            if (!IsAdministrator)
                throw new EntityPermissionException("User does not have permission to perform this action");

            var entity = await OrderRepository.GetById(id);

            if (entity == null)
                return null;

            entity.Status = status;
            await OrderRepository.DbContext.SaveChangesAsync();

            Dispatch(new DomainEvent(entity, entity.Id.ToString(), entity.Id.ToString(), "orderstatus"));

            return await GetById(id);
        }

        /// <summary>
        /// add order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OrderDetail> Add(OrderEdit model)
        {
            if (model == null)
                throw new InvalidModelException("model is null");

            var entity = SetProperties(new Order(), model);

            entity.Created = DateTime.UtcNow;
            entity.CreatedById = ProfileIdentity.Profile.Id;
            entity.Status = OrderStatus.Draft;

            var saved = await OrderRepository.Add(entity);

            return await GetById(saved.Id);
        }

        /// <summary>
        /// update order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OrderDetail> Update(int id, OrderEdit model)
        {
            if (model == null)
                throw new InvalidModelException("model is null");

            if (model.Id == 0)
                throw new InvalidIdentityException("model is invalid");

            if (id != model.Id)
                throw new InvalidIdentityException("id mismatch");

            if (!(await Exists(id)))
                throw new EntityNotFoundException("record with id '" + id + "' was not found");

            var entity = await OrderRepository.GetById(id);

            if (!IsAdministrator && entity.CreatedById != ProfileIdentity.Profile.Id)
                throw new EntityPermissionException("User does not have permission to perform this action");

            var sendNotification = false;

            if (model.Status == OrderStatus.Draft || model.Status == OrderStatus.Submitted)
            {
                // send notification when order status changed to submitted

                sendNotification = entity.Status != OrderStatus.Submitted && model.Status == OrderStatus.Submitted;

                entity.Status = model.Status;
            };

            SetProperties(entity, model);

            var saved = await OrderRepository.Update(entity);

            if (sendNotification)
            {
                var notification = new OrderAddModel
                {
                    Order = saved,
                    EventDesigners = OrderRepository.DbContext.Profiles.Where(p => p.IsAdministrator).Select(p => p.GlobalId).ToArray()
                };

                Dispatch(new DomainEvent(notification, saved.Id.ToString(), saved.Id.ToString(), "orderadd"));
            }

            return await GetById(id);
        }

        /// <summary>
        /// delete order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Delete(int id)
        {
            var entity = await OrderRepository.GetById(id);

             if (!IsAdministrator && entity.CreatedById != ProfileIdentity.Profile.Id)
                throw new EntityPermissionException("User does not have permission to perform this action");

            await OrderRepository.Delete(entity);
        }

        /// <summary>
        /// check if order exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Exists(int id)
        {
            return await OrderRepository.Exists(id);
        }

        /// <summary>
        /// set order properties
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Order SetProperties(Order entity, OrderEdit model)
        {
            entity.AdversaryDetails = model.AdversaryDetails;
            entity.AudienceNumber = model.AudienceNumber;
            entity.CyberThreats = model.CyberThreats;
            entity.IsEmbeddedTeam = model.IsEmbeddedTeam;
            entity.MissionProcedures = model.MissionProcedures;
            entity.Storyline = model.Storyline;
            entity.Theater = model.Theater;
            entity.ContentDescription = model.ContentDescription;
            entity.Description = model.Description;
            entity.Due = model.Due;
            entity.Duration = model.Duration;
            entity.DurationType = model.DurationType;
            entity.Email = model.Email;
            entity.EventEnd = model.EventEnd;
            entity.EnvironmentDetails = model.EnvironmentDetails;
            entity.EventParticipants = model.EventParticipants;
            entity.IsConfigurationNeeded = model.IsConfigurationNeeded;
            entity.IsPrivate = model.IsPrivate;
            entity.Objectives = model.Objectives;
            entity.Onboarding = model.Onboarding;
            entity.Phone = model.Phone;
            entity.NetworkDiagramUrl = model.NetworkDiagramUrl;
            entity.Prerequisites = model.Prerequisites;
            entity.Requestor = model.Requestor;
            entity.ScenarioAudienceDetails = model.ScenarioAudienceDetails;
            entity.ScenarioAudienceProcedures = model.ScenarioAudienceProcedures;
            entity.ScenarioCommunications = model.ScenarioCommunications;
            entity.EventStart = model.EventStart;
            entity.SuccessIndicators = model.SuccessIndicators;
            entity.ToolPreparation = model.ToolPreparation;
            entity.TrainingDescription = model.TrainingDescription;
            entity.Unit = model.Unit;
            entity.RoleCrewPosition = model.RoleCrewPosition;

            return MapRelationships(entity, model);
        }

        /// <summary>
        /// map order properties to edit model
        /// TODO: move to mapping profile
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Order MapRelationships(Order entity, OrderEdit model)
        {
            var db = OrderRepository.DbContext;

            entity.AudienceId = model.AudienceId;
            entity.EventTypeId = model.EventTypeId;
            entity.BranchId = model.BranchId;
            entity.BranchOther = model.BranchOther;
            entity.ClassificationId = model.ClassificationId;
            entity.ClassificationOther = model.ClassificationOther;
            entity.FacilityId = model.FacilityId;
            entity.ProducerId = model.ProducerId;
            entity.RankId = model.RankId;
            entity.ContentTypeId = model.ContentTypeId;

            entity.AssessmentTypeOther = model.AssessmentTypeOther;
            entity.AudienceItemOther = model.AudienceItemOther;
            entity.OperatingSystemOther = model.OperatingSystemOther;
            entity.ThreatOther = model.ThreatOther;
            entity.SecurityToolOther = model.SecurityToolOther;
            entity.ServiceOther = model.ServiceOther;
            entity.EmbeddedTeamOther = model.EmbeddedTeamOther;
            entity.SimulatorOther = model.SimulatorOther;
            entity.SupportOther = model.SupportOther;
            entity.TerrainOther = model.TerrainOther;

            // map collections
            entity.Files = MapFiles(entity.Files, model.Files);

            entity.OrderAssessmentTypes = Map(entity.OrderAssessmentTypes, model.AssessmentTypes, db.OrderAssessmentTypes,
                (id) => { return id.AssessmentTypeId; }, (id) => {
                    return new OrderAssessmentType
                    {
                        AssessmentType = db.AssessmentTypes.Single(ai => ai.Id == id)
                    };
                });

            entity.OrderAudienceItems = Map(entity.OrderAudienceItems, model.AudienceItems, db.OrderAudienceItems,
               (e) => { return e.AudienceItemId; }, (m) => {
                   return new OrderAudienceItem { AudienceItem = db.AudienceItems.Single(ai => ai.Id == m) };
               });

            entity.OrderOperatingSystemTypes = Map(entity.OrderOperatingSystemTypes, model.OperatingSystemTypes, db.OrderOperatingSystemTypes,
                (id) => { return id.OperatingSystemTypeId; }, (id) => {
                    return new OrderOperatingSystemType { OperatingSystemType = db.OperatingSystemTypes.Single(ai => ai.Id == id) };
                });

            entity.OrderThreats = Map(entity.OrderThreats, model.Threats, db.OrderThreats,
                (id) => { return id.ThreatId; }, (id) => {
                    return new OrderThreat { Threat = db.Threats.Single(ai => ai.Id == id) };
                });

            entity.OrderSecurityTools = Map(entity.OrderSecurityTools, model.SecurityTools, db.OrderSecurityTools,
                (id) => { return id.SecurityToolId; }, (id) => {
                    return new OrderSecurityTool { SecurityTool = db.SecurityTools.Single(ai => ai.Id == id) };
                });

            entity.OrderServices = Map(entity.OrderServices, model.Services, db.OrderServices,
                (id) => { return id.ServiceId; }, (id) => {
                    return new Data.Entities.OrderService { Service = db.Services.Single(ai => ai.Id == id) };
                });

            entity.OrderEmbeddedTeams = Map(entity.OrderEmbeddedTeams, model.EmbeddedTeams, db.OrderEmbeddedTeams,
                (id) => { return id.EmbeddedTeamId; }, (id) => {
                    return new OrderEmbeddedTeam { EmbeddedTeam = db.EmbeddedTeams.Single(ai => ai.Id == id) };
                });

            entity.OrderSimulators = Map(entity.OrderSimulators, model.Simulators, db.OrderSimulators,
                (id) => { return id.SimulatorId; }, (id) => {
                    return new OrderSimulator { Simulator = db.Simulators.Single(ai => ai.Id == id) };
                });

            entity.OrderSupports = Map(entity.OrderSupports, model.Supports, db.OrderSupports,
                (id) => { return id.SupportId; }, (id) => {
                    return new OrderSupport { Support = db.Supports.Single(ai => ai.Id == id) };
                });

            entity.OrderTerrains = Map(entity.OrderTerrains, model.Terrains, db.OrderTerrains,
                (id) => { return id.TerrainId; }, (id) =>
                {
                    return new OrderTerrain { Terrain = db.Terrains.Single(ai => ai.Id == id) };
                });

            return entity;
        }

        /// <summary>
        /// map collection relationship
        /// </summary>
        /// <typeparam name="TJoinEntity"></typeparam>
        /// <param name="current"></param>
        /// <param name="update"></param>
        /// <param name="set"></param>
        /// <param name="selectId"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        ICollection<TJoinEntity> Map<TJoinEntity>(
            ICollection<TJoinEntity> current,
            IEnumerable<int> update,
            DbSet<TJoinEntity> set,
            Func<TJoinEntity, int> selectId,
            Func<int, TJoinEntity> select)
            where TJoinEntity : class, IJoinEntity
        {
            //TODO: preserve previous

            current.Clear();

            var newSet = update.Distinct();

            foreach (var model in newSet)
            {
                current.Add(select(model));
            }

            return current;
        }

        /// <summary>
        /// map file relationship
        /// </summary>
        /// <param name="current"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        ICollection<File> MapFiles(ICollection<File> current, IEnumerable<OrderEditFile> update)
        {
            //TODO: preserve previous

            current.Clear();

            var newSet = update.Distinct();

            foreach (var file in newSet)
            {
                current.Add(new File { Name = file.Name ?? file.Url, Url = file.Url, Created = DateTime.UtcNow, Type = file.Type, CreatedById = ProfileIdentity.Profile.Id });
            }

            return current;
        }
    }
}
