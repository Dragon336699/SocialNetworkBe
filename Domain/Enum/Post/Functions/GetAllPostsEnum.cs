using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Post.Functions
{
    public enum GetAllPostsEnum
    {
        Success,
        NoPostsFound,
        Failed
    }

    public static class GetAllPostsEnumMessage
    {
        public static string GetMessage(this GetAllPostsEnum status)
        {
            return status switch
            {
                GetAllPostsEnum.Success => "Posts retrieved successfully.",
                GetAllPostsEnum.NoPostsFound => "No posts found.",
                GetAllPostsEnum.Failed => "Failed to retrieve posts.",
                _ => "Unknown error."
            };
        }
    }
}
