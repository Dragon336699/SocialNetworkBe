
using Domain.Contracts.Responses.Notification;
using Domain.Entities;
using Domain.Enum.User.Types;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DataAccess.DbContext
{
    public class SocialNetworkDbContext : IdentityDbContext<User, Role, Guid>
    {
        public SocialNetworkDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Comment> Comment { get; set; }
        public DbSet<CommentImage> CommentImage { get; set; }
        public DbSet<CommentReactionUser> CommentReactionUser { get; set; }
        public DbSet<Conversation> Conversation { get; set; }
        public DbSet<ConversationUser> ConversationUser { get; set; }
        public DbSet<FriendRequest> FriendRequest { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<GroupUser> GroupUser { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<MessageAttachment> MessageAttachment { get; set; }
        public DbSet<MessageReactionUser> MessageReactionUser { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<PostImage> PostImage { get; set; }
        public DbSet<PostReactionUser> PostReactionUser { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<SearchingHistory> SearchingHistory { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserRelation> UserRelation { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(Guid) && property.IsPrimaryKey())
                    {
                        property.SetDefaultValueSql("NEWID()");
                    }
                }
            }

            // Config composite key

            builder.Entity<CommentReactionUser>()
               .HasKey(e => new { e.UserId, e.CommentId });

            builder.Entity<ConversationUser>()
                .HasKey(e => new { e.UserId, e.ConversationId });

            builder.Entity<GroupUser>()
                .HasKey(e => new { e.UserId, e.GroupId });

            builder.Entity<MessageReactionUser>()
               .HasKey(e => new { e.UserId, e.MessageId });

            builder.Entity<PostReactionUser>()
               .HasKey(e => new { e.UserId, e.PostId });

            builder.Entity<UserRelation>()
               .HasKey(e => new { e.UserId, e.RelatedUserId });

            builder.Entity<FriendRequest>()
               .HasKey(e => new { e.SenderId, e.ReceiverId });

            // Config enum to string
            builder.Entity<User>(entity =>
            {
                entity.Property(u => u.Status)
                    .HasConversion<string>();
                entity.Property(u => u.Gender)
                    .HasConversion<string>();
            });

            builder.Entity<Notification>(entity =>
            {
                entity.Property(n => n.NoficationType)
                    .HasConversion<string>();
                entity.Property(n => n.Data)
                    .HasConversion(
                        v => JsonSerializer.Serialize<NotificationData>(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<NotificationData>(v, (JsonSerializerOptions?)null)!);
                entity.HasIndex(n => n.MergeKey)
                    .IsUnique();
            });

            builder.Entity<Message>(entity =>
            {
                entity.Property(m => m.Status)
                    .HasConversion<string>();
            });

            builder.Entity<MessageAttachment>(entity =>
            {
                entity.Property(ma => ma.FileType)
                    .HasConversion<string>();
            });

            builder.Entity<GroupUser>(entity =>
            {
                entity.Property(gu => gu.RoleName)
                    .HasConversion<string>();
            });

            builder.Entity<Conversation>(entity =>
            {
                entity.Property(c => c.Type)
                    .HasConversion<string>();
            });

            builder.Entity<ConversationUser>(entity =>
            {
                entity.Property(cu => cu.RoleName)
                    .HasConversion<string>();
            });

            builder.Entity<Post>(entity =>
            {
                entity.Property(p => p.PostPrivacy)
                    .HasConversion<string>();
            });

            builder.Entity<UserRelation>(entity =>
            {
                entity.Property(ur => ur.RelationType)
                    .HasConversion<string>();
            });

            //  Config self relationship
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FriendRequest>()
                .HasOne(fe => fe.Sender)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(fe => fe.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FriendRequest>()
                .HasOne(fe => fe.Receiver)
                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(fe => fe.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SearchingHistory>()
                .HasOne(sh => sh.SearchedUser)
                .WithMany(u => u.Searched)
                .HasForeignKey(sh => sh.SearchedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SearchingHistory>()
               .HasOne(sh => sh.User)
               .WithMany(u => u.SearchingHistories)
               .HasForeignKey(sh => sh.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserRelation>()
               .HasOne(ur => ur.User)
               .WithMany(u => u.Relations)
               .HasForeignKey(ur => ur.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserRelation>()
               .HasOne(ur => ur.RelatedUser)
               .WithMany(u => u.RelatedTo)
               .HasForeignKey(ur => ur.RelatedUserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
              .HasOne(m => m.Sender)
              .WithMany(u => u.MessageSent)
              .HasForeignKey(m => m.SenderId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Post>()
              .HasOne(p => p.User)
              .WithMany(u => u.Posts)
              .HasForeignKey(p => p.UserId)
              .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}
