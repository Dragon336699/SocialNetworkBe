using DataAccess.DbContext;
using Domain.Entities;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class PostImageRepository : GenericRepository<PostImage>, IPostImageRepository
    {
        public PostImageRepository(SocialNetworkDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PostImage>?> GetImagesByPostIdAsync(Guid postId)
        {
            var images = await _context.PostImage
                .Where(pi => pi.PostId == postId)
                .ToListAsync();

            return images.Any() ? images : null;
        }
    }
}