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
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Repositories
{
    public class DiscussionRepository : Repository<Discussion>, IDiscussionRepository
    {
        public DiscussionRepository(SketchDbContext dbContext)
            : base(dbContext) { }

        int CleanVote(int value)
        {
            if (value > 1) value = 1;
            if (value < -1) value = -1;
            return value;
        }

        public async Task AddCommentVote(int commentId, int profileId, int value)
        {
            DbContext.CommentVotes.Add(new CommentVote
            {
                CommentId = commentId,
                ProfileId = profileId,
                Value = CleanVote(value)
            });

            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateCommentVote(CommentVote cv, int value)
        {
            cv.Value = CleanVote(value);

            await DbContext.SaveChangesAsync();
        }

        public async Task DeleteCommentVote(CommentVote cv)
        {
            var vote = await DbContext.CommentVotes
                .SingleOrDefaultAsync(v => v.CommentId == cv.CommentId && v.ProfileId == cv.ProfileId);

            if (vote == null)
                return;

            DbContext.CommentVotes.Remove(vote);
            await DbContext.SaveChangesAsync();
        }

        public async override Task<Discussion> GetById(int id)
        {
            return await DbContext.Discussions
                .Include(d => d.Comments)
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        public override IQueryable<Discussion> GetAll()
        {
            return DbContext.Discussions.Include(d => d.Comments);
        }

        public async Task<Comment> GetComment(int commentId)
        {
            return await DbContext.Comments
                .Include(c => c.Discussion)
                .Include(c => c.Profile)
                .Include(c => c.Votes)
                .SingleOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<CommentVote> GetCommentVote(int commentId, int profileId)
        {
            return await DbContext.CommentVotes
                .SingleOrDefaultAsync(o => o.CommentId == commentId && o.ProfileId == profileId);
        }

        public async Task<Comment> UpdateComment(int id, string text)
        {
            var comment = await GetComment(id);
            comment.Text = text;
            await DbContext.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> AddComment(int discussionId, int profileId, string text)
        {
            var comment = new Comment
            {
                DiscussionId = discussionId,
                ProfileId = profileId,
                Text = text
            };

            comment.Votes.Add(new CommentVote { ProfileId = profileId });

            DbContext.Comments.Add(comment);
            await DbContext.SaveChangesAsync();

            return comment;
        }

        public async Task DeleteComment(int id)
        {
            var comment = await DbContext.Comments.SingleOrDefaultAsync(c => c.Id == id);

            if (comment == null)
                return;

            DbContext.Comments.Remove(comment);

            await DbContext.SaveChangesAsync();
        }

        public async Task<bool> IsCommentOwner(int id, int profileId)
        {
            return await DbContext.Comments.AnyAsync(c => c.Id == id && c.ProfileId == profileId);
        }

        public IQueryable<Discussion> GetAllByContentIdAndDiscussionType(int id, DiscussionType type)
        {
            return DbContext.Discussions
                .Include(d => d.Comments)
                .ThenInclude(c => c.Profile)
                .Where(d => d.ContentId == id && d.Type == type);
        }

        public IQueryable<Comment> GetAllCommentsByDiscussionId(int id)
        {
            return DbContext.Comments
                .Include(c => c.Profile)
                .Include(c => c.Votes)
                .Where(o => o.DiscussionId == id);
        }
    }
}
