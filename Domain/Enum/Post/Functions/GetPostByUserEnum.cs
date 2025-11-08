using System;

namespace Domain.Enum.Post.Functions
{
    public enum GetPostsByUserEnum
    {
        Success,
        NoPostsFound,
        Failed
    }

    public static class GetPostsByUserEnumMessage
    {
        public static string GetMessage(this GetPostsByUserEnum status)
        {
            return status switch
            {
                GetPostsByUserEnum.Success => "User posts retrieved successfully.",
                GetPostsByUserEnum.NoPostsFound => "No posts found for this user.",
                GetPostsByUserEnum.Failed => "Failed to retrieve user posts.",
                _ => "Unknown error."
            };
        }
    }
}
