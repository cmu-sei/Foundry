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
using Foundry.Portal.Events;
using Foundry.Portal.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Services
{
    public class PostService : DispatchService<Post>
    {
        IPostRepository _postRepository;
        public PostService(
            IPostRepository postRepository,
            IDomainEventDispatcher domainEventDispatcher,
            CoreOptions options,
            IStackIdentityResolver userResolver,
            ILoggerFactory loggerFactory,
            IMapper mapper)
            : base(domainEventDispatcher, options, userResolver, loggerFactory, mapper)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException("postRepository");
        }

        public async Task<PagedResult<Post, PostDetail>> GetAll(PostDataFilter search)
        {
            var query = _postRepository.GetAll();
            return await PagedResult<Post, PostDetail>(query, search);
        }

        public async Task<PostDetail> GetById(int id)
        {
            var post = await _postRepository.GetById(id);

            return Mapper.Map<PostDetail>(post);
        }

        public async Task<PostDetail> Add(PostCreate model)
        {
            Data.Entities.Profile profile = null;

            if (model.ParentId.HasValue)
            {
                var parent = await _postRepository.GetById(model.ParentId.Value);

                if (parent == null)
                    throw new EntityNotFoundException("Parent post '" + model.ParentId + "' was not found");

                if (parent.ParentId.HasValue)
                    throw new InvalidModelException("Cannot reply to child posts.");

                profile = _postRepository.DbContext.Profiles.Single(p => p.Id == parent.ProfileId);
            }

            var post = new Post
            {
                ProfileId = Identity.GetId(),
                Text = model.Text,
                ParentId = model.ParentId
            };

            foreach (var a in model.Attachments)
            {
                post.Attachments.Add(new PostAttachment { Url = a });
            }

            var saved = await _postRepository.Add(post);

            if (profile != null)
            {
                Dispatch(new DomainEvent(profile.GlobalId, profile.Name, DomainEventType.ProfilePost));
            }

            return Mapper.Map<PostDetail>(saved);
        }

        public async Task<PostDetail> Update(PostUpdate model)
        {
            var post = await _postRepository.GetById(model.Id);

            post.Text = model.Text;

            post.Attachments.Clear();

            foreach (var a in model.Attachments)
            {
                post.Attachments.Add(new PostAttachment { Url = a });
            }

            var saved = await _postRepository.Add(post);

            return Mapper.Map<PostDetail>(saved);
        }

        public async Task<bool> Delete(int id)
        {
            var post = await _postRepository.GetById(id);

            if (post.ProfileId != Identity.GetId())
                throw new EntityPermissionException("Action requires elevated permissions.");

            await _postRepository.Delete(post);

            return true;
        }

        public async Task<PostVoteMetric> UpVote(int postId)
        {
            return await Vote(postId, 1);
        }

        public async Task<PostVoteMetric> DownVote(int postId)
        {
            return await Vote(postId, -1);
        }

        async Task<PostVoteMetric> Vote(int postId, int value)
        {
            var post = await _postRepository.GetById(postId);

            if (post == null)
                throw new EntityNotFoundException("Comment was not found.");

            var db = _postRepository.DbContext;

            var id = Identity.GetId();

            var postVote = await db.PostVotes
                .FirstOrDefaultAsync(v => v.PostId == postId && v.ProfileId == id);

            value = value > 0 ? 1 : -1;

            var userVote = value;

            if (postVote == null)
            {
                // add vote
                await db.PostVotes.AddAsync(new PostVote { PostId = postId, Value = value, ProfileId = id });
            }
            else
            {
                if (postVote.Value == value)
                {
                    // toggle vote
                    userVote = 0;
                    db.PostVotes.Remove(postVote);
                }
                else
                {
                    // update vote
                    postVote.Value = value;
                }
            }

            await db.SaveChangesAsync();

            return await CalculatePostVotes(postId, userVote);
        }

        public async Task<PostVoteMetric> CalculatePostVotes(int postId, int userVote)
        {
            var db = _postRepository.DbContext;
            var post = await _postRepository.GetById(postId);

            var up = await db.PostVotes
                .Where(v => v.PostId == postId && v.Value > 0)
                .CountAsync();

            var down = await db.PostVotes
                .Where(v => v.PostId == postId && v.Value < 0)
                .CountAsync();

            post.Value = up - down;
            await db.SaveChangesAsync();

            return new PostVoteMetric { Up = up, Down = down, UserVote = userVote };
        }
    }
}
