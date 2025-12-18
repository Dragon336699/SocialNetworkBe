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
                var comments = await _context.Comment
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(c => c.PostId == postId && c.RepliedCommentId == null)
                    .Include(c => c.User)
                    .Include(c => c.CommentImage)
                    .Include(c => c.CommentReactionUsers)
                    .ToListAsync();

                // Load replies đệ quy
                foreach (var comment in comments)
                {
                    await LoadRepliesRecursive(comment);
                }          
                return comments.Skip(skip).Take(take).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task LoadRepliesRecursive(Comment comment)
        {
            // Lấy các replies của comment hiện tại 
            comment.Replies = await _context.Comment
                .AsNoTracking()
                .AsSplitQuery()
                .Where(c => c.RepliedCommentId == comment.Id)
                .Include(c => c.User)
                .Include(c => c.CommentImage)
                .Include(c => c.CommentReactionUsers)
                .ToListAsync();

            // Đệ quy để load replies của replies
            foreach (var reply in comment.Replies)
            {
                await LoadRepliesRecursive(reply);
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
