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
using Microsoft.Extensions.Logging;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Extensions;
using Foundry.Portal.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Services
{
    public class DiscussionService : Service<Discussion>
    {
        IDiscussionRepository _discussionRepository;

        public DiscussionService(IDiscussionRepository discussionRepository, CoreOptions options, IStackIdentityResolver userResolver, ILoggerFactory loggerFactory, IMapper mapper)
            : base(options, userResolver, loggerFactory, mapper)
        {
            _discussionRepository = discussionRepository ?? throw new ArgumentNullException("discussionRepository");
        }

        public async Task<DiscussionDetail> GetById(int id)
        {
            Discussion model = await _discussionRepository.GetById(id);

            if (model == null)
                throw new EntityNotFoundException("Discussion was not found.");

            if (!CanComment(model))
                throw new EntityPermissionException("Getting content requires elevated permissions.");

            return Map<DiscussionDetail>(model);
        }

        public async Task<PagedResult<Discussion, DiscussionSummary>> GetAll(DiscussionDataFilter search)
        {
            return await PagedResult<Discussion, DiscussionSummary>(_discussionRepository.GetAll(), search);
        }

        public async Task<DiscussionDetail> Add(DiscussionCreate model)
        {
            if (!await CanManage(model.ContentId.Value, model.Type))
                throw new EntityPermissionException("Creating content requires elevated permissions.");

            var discussion = new Discussion
            {
                Name = model.Name,
                Description = model.Description,
                ContentId = model.ContentId,
                Type = model.Type,
                GlobalId = model.GlobalId,
                Status = model.Status
            };

            await _discussionRepository.Add(discussion);

            return await GetById(discussion.Id);
        }

        public async Task<DiscussionDetail> Update(DiscussionUpdate model)
        {
            if (!await CanManage(model.ContentId.Value, model.Type))
                throw new EntityPermissionException("Content edit requires elevated permissions.");

            var discussion = await _discussionRepository.GetById(model.Id);

            discussion.Name = model.Name;
            discussion.Description = model.Description;
            discussion.ContentId = model.ContentId;
            discussion.Type = model.Type;
            discussion.GlobalId = model.GlobalId;
            discussion.Status = model.Status;

            await _discussionRepository.Update(discussion);

            return await GetById(discussion.Id);
        }
        public async Task<List<DiscussionDetail>> GetByContentIdAndDiscussionType(DiscussionType type, int id)
        {
            var discussions = await _discussionRepository.GetAllByContentIdAndDiscussionType(id, type).ToListAsync();

            return discussions.Select(d => Map<DiscussionDetail>(d)).ToList();
        }

        public async Task<PagedResult<Comment, DiscussionDetailComment>> GetAllCommentsByDiscussionId(int discussionId, CommentDataFilter search)
        {
            Discussion discussion = await _discussionRepository.GetById(discussionId);

            if (discussion == null)
                throw new EntityNotFoundException("Discussion was not found.");

            if (!(CanComment(discussion)))
                throw new EntityPermissionException("Getting all comments for discussion requires elevated permissions.");

            var query = _discussionRepository.GetAllCommentsByDiscussionId(discussion.Id);

            return await PagedResult<Comment, DiscussionDetailComment>(query, search);
        }

        public async Task<PagedResult<Comment, DiscussionDetailComment>> GetAllCommentsByContentId(int contentId, CommentDataFilter search)
        {
            var discussions = _discussionRepository.GetAllByContentIdAndDiscussionType(contentId, DiscussionType.ContentReview);

            var discussion = await discussions.FirstOrDefaultAsync();

            if (discussion == null)
                throw new EntityNotFoundException("Discussion was not found.");

            if (!CanComment(discussion))
                throw new EntityPermissionException("Getting all comments for content requires elevated permissions.");

            var query = _discussionRepository.GetAllCommentsByDiscussionId(discussion.Id);

            var result = await PagedResult<Comment, DiscussionDetailComment>(query, search);

            return result;
        }

        private async Task<bool> CanComment(int discussionId)
        {
            return CanComment(await _discussionRepository.DbContext.Discussions.FindAsync(discussionId));
        }

        private bool CanComment(Discussion discussion)
        {
            return (
                discussion != null
                && discussion.Status == DiscussionStatus.Open
                && discussion.Type == DiscussionType.ContentReview
            );
            //return (await _discussionRepository.PermissionForContent(discussion.ContentId.Value, ProfileId)).CanAccess(Identity);
        }

        private async Task<bool> CanManage(int contentId, DiscussionType type)
        {
            //if (type == DiscussionType.ContentReview)
            //    return (await _discussionRepository.PermissionForContent(contentId, ProfileId)).CanManage(Identity);

            if (type == DiscussionType.ContentReview)
            {
                var content = await _discussionRepository.DbContext.Contents.Where(c => c.Id == contentId).FirstOrDefaultAsync();

                if (content != null)
                {
                    if (Identity.Permissions.Contains(SystemPermissions.PowerUser))
                        return true;

                    if (content.CreatedBy == Identity.Id)
                        return true;
                }
            }

            return false;
        }

        public async Task<DiscussionDetailComment> AddContentComment(int id, DiscussionType type, string text)
        {
            var content = await _discussionRepository.DbContext.Contents
                .Include(c => c.Discussions)
                .SingleOrDefaultAsync(c => c.Id == id);

            return await AddComment(content, type, text);
        }

        public async Task<DiscussionDetailComment> AddComment(IDiscussions entity, DiscussionType type, string text)
        {
            if (entity is Content content)
            {
                var discussion = await _discussionRepository.DbContext.Discussions
                        .SingleOrDefaultAsync(m => m.ContentId == content.Id && m.Type == type);

                if (discussion == null)
                {
                    AddDiscussionToContent(content);
                    discussion = content.Discussions.First();
                }

                await _discussionRepository.DbContext.SaveChangesAsync();

                return await AddComment(discussion.Id, new CommentCreate { Text = text });
            }

            throw new InvalidModelException("Content was not found.");
        }

        public async Task DeleteComment(int id)
        {
            if (!(await CanEditComment(id)))
                throw new EntityPermissionException("Comment delete requires elevated permissions.");

            await _discussionRepository.DeleteComment(id);
        }

        async Task<bool> CanEditComment(int id)
        {
            return Identity.Permissions.Contains(SystemPermissions.PowerUser) ||
                await _discussionRepository.IsCommentOwner(id, Identity.GetId());
        }

        public async Task<DiscussionDetailComment> AddComment(int discussionId, CommentCreate comment)
        {
            if (!(await CanComment(discussionId)))
                throw new EntityPermissionException("Commenting requires elevated permissions.");

            if (!string.IsNullOrWhiteSpace(comment.Text) && comment.Text.Length > 512)
            {
                comment.Text = comment.Text.Substring(0, 512);
            }

            Comment result = await _discussionRepository.AddComment(discussionId, Identity.GetId(), comment.Text);

            return Map<DiscussionDetailComment>(result);
        }

        public void AddDiscussionToContent(Content content)
        {
            if (!content.Discussions.Any(d => d.Type == DiscussionType.ContentReview))
            {
                content.Discussions.Add(new Discussion()
                {
                    Name = content.Name,
                    Type = DiscussionType.ContentReview,
                    Status = DiscussionStatus.Open
                });
            }
        }

        public async Task<DiscussionDetailComment> UpdateComment(CommentUpdate comment)
        {
            Comment target = await _discussionRepository.GetComment(comment.Id);

            if (!(await CanComment(target.DiscussionId)))
                throw new EntityPermissionException("Commenting requires elevated permissions.");

            if (!(await CanEditComment(comment.Id)))
                throw new EntityPermissionException("Comment editing requires elevated permissions.");

            var result = await _discussionRepository.UpdateComment(comment.Id, comment.Text);

            return Map<DiscussionDetailComment>(result);
        }

        public async Task<DiscussionDetailComment> UpVote(int commentId)
        {
            return await Vote(commentId, 1);
        }

        public async Task<DiscussionDetailComment> DownVote(int commentId)
        {
            return await Vote(commentId, -1);
        }

        /// <summary>
        /// [REVIEW] Should users be able to remove their vote? Seems like once they vote they are locked in
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        async Task<DiscussionDetailComment> Vote(int commentId, int value)
        {
            Comment comment = await _discussionRepository.GetComment(commentId);

            if (comment == null)
                throw new EntityNotFoundException("Comment was not found.");

            if (!CanComment(comment.Discussion))
                throw new EntityPermissionException("Voting on comment requires elevated permissions.");

            var commentVote = await _discussionRepository.GetCommentVote(commentId, Identity.GetId());

            value = value > 0 ? 1 : -1;

            if (commentVote != null)
            {
                if (commentVote.Value == value)
                {
                    await _discussionRepository.DeleteCommentVote(commentVote);
                }
                else
                {
                    await _discussionRepository.UpdateCommentVote(commentVote, value);
                }
            }
            else
            {
                await _discussionRepository.AddCommentVote(comment.Id, Identity.GetId(), value);
            }

            return Map<DiscussionDetailComment>(comment);
        }
    }
}
