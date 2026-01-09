using DataAccess.DbContext;
using Domain.Entities;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(SocialNetworkDbContext context) : base(context)
        {
        }
        public async Task<List<Comment>?> GetCommentsByPostIdAsync(Guid postId, int skip = 0, int take = 10)
        {
            try
            {              
                var allComments = await _context.Comment
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(c => c.PostId == postId)
                    .Include(c => c.User)
                    .Include(c => c.CommentImage)
                    .Include(c => c.CommentReactionUsers)
                    .ToListAsync();
               
                if (!allComments.Any())
                    return new List<Comment>();
             
                var commentDict = allComments.ToDictionary(c => c.Id);

                var rootComments = new List<Comment>();

                foreach (var comment in allComments)
                {
                    if (comment.RepliedCommentId == null)
                    {                      
                        rootComments.Add(comment);
                    }
                    else
                    {                       
                        if (commentDict.TryGetValue(comment.RepliedCommentId.Value, out var parentComment))
                        {                          
                            if (parentComment.Replies == null)
                                parentComment.Replies = new List<Comment>();                          
                            parentComment.Replies.Add(comment);
                        }
                    }
                }         
                SortRepliesRecursive(rootComments);
             
                return rootComments
                    .OrderBy(c => c.CreatedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
            }
            catch (Exception ex)
            {            
                return null;
            }
        }     
        private void SortRepliesRecursive(List<Comment> comments)
        {
            if (comments == null || !comments.Any())
                return;

            foreach (var comment in comments)
            {
                if (comment.Replies != null && comment.Replies.Any())
                {                   
                    var sortedReplies = comment.Replies
                        .OrderBy(r => r.CreatedAt)
                        .ToList();
                  
                    comment.Replies = sortedReplies;                    
                    SortRepliesRecursive(sortedReplies);
                }
            }
        }

        public async Task<List<Comment>?> GetRepliesByCommentIdAsync(Guid commentId, int skip = 0, int take = 10)
        {
            try
            {
                return await _context.Comment
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(c => c.RepliedCommentId == commentId)
                    .Include(c => c.User)
                    .Include(c => c.CommentImage)
                    .Include(c => c.CommentReactionUsers)
                    .OrderBy(c => c.CreatedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Comment?> GetCommentByIdWithTrackingAsync(Guid commentId)
        {
            return await _context.Comment
                .Include(c => c.User)
                .Include(c => c.CommentImage)
                .Include(c => c.CommentReactionUsers)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<Comment?> GetCommentNewestByPostId(Guid postId)
        {
            return await _context.Comment
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
