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
using Stack.DomainEvents;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Foundry.Orders.Data.Entities;
using Foundry.Orders.Data.Repositories;
using Foundry.Orders.Identity;
using Foundry.Orders.ViewModels;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Orders.Services
{
    /// <summary>
    /// comment service
    /// </summary>
    public class CommentService : DispatchService<Comment>
    {
        ICommentRepository CommentRepository { get; }
        IValidationHandler ValidationHandler { get; }

        ProfileIdentity ProfileIdentity
        {
            get { return Identity as ProfileIdentity; }
        }

        /// <summary>
        /// create an instance of the order service
        /// </summary>
        /// <param name="domainEventDispatcher"></param>
        /// <param name="identityResolver"></param>
        /// <param name="mapper"></param>
        /// <param name="commentRepository"></param>
        /// <param name="validationHandler"></param>
        public CommentService(IDomainEventDispatcher domainEventDispatcher, IStackIdentityResolver identityResolver, IMapper mapper, ICommentRepository commentRepository, IValidationHandler validationHandler)
            : base(domainEventDispatcher, identityResolver, mapper)
        {
            CommentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            ValidationHandler = validationHandler ?? throw new ArgumentNullException(nameof(validationHandler));
        }

        /// <summary>
        /// get all comments for order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        public async Task<PagedResult<Comment, CommentDetail>> GetAll(int orderId, CommentDataFilter dataFilter = null)
        {
            var query = CommentRepository.GetAll().Where(x => x.OrderId == orderId);
            return await PagedResult<Comment, CommentDetail>(query, dataFilter);
        }

        /// <summary>
        /// add comment to order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<CommentDetail> Add(int orderId, CommentEdit model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var entity = new Comment
            {
                OrderId = orderId,
                Title = model.Title,
                Message = model.Message,
                Created = DateTime.UtcNow,
                CreatedById = ProfileIdentity.Profile.Id
            };

            var result = await CommentRepository.Add(entity);

            var order = await CommentRepository.DbContext.Orders
                .Include(o => o.CreatedBy)
                .SingleOrDefaultAsync(o => o.Id == orderId);

            Dispatch(new DomainEvent(order, order.Id.ToString(), order.Id.ToString(), "ordercomment"));

            return Mapper.Map<CommentDetail>(result);
        }

        /// <summary>
        /// update comment
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<CommentDetail> Update(int orderId, CommentEdit model)
        {
            if (orderId != model.Id)
                throw new InvalidModelException("ID mismatch.");

            var entity = await CommentRepository.GetById(orderId);

            if (entity == null)
                throw new EntityNotFoundException("Comment was not found.");

            await ValidationHandler.ValidateRulesFor(model);

            entity.Title = model.Title;
            entity.Created = DateTime.UtcNow;
            entity.Message = model.Message;

            var result = await CommentRepository.Update(entity);

            var order = await CommentRepository.DbContext.Orders
                .Include(o => o.CreatedBy)
                .SingleOrDefaultAsync(o => o.Id == orderId);

            Dispatch(new DomainEvent(order, order.Id.ToString(), order.Id.ToString(), "ordercomment"));

            return Mapper.Map<CommentDetail>(result);
        }
    }
}
