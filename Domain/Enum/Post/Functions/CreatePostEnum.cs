using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Post.Functions
{
    public enum CreatePostEnum
    {
        UserNotFound,      
        InvalidContent,    
        CreatePostSuccess, 
        CreatePostFailed   
    }

    public static class CreatePostEnumMessage
    {
        public static string GetMessage(this CreatePostEnum status)
        {
            return status switch
            {
                CreatePostEnum.UserNotFound => "User not found.",
                CreatePostEnum.InvalidContent => "Invalid post content.",
                CreatePostEnum.CreatePostSuccess => "Post created successfully.",
                CreatePostEnum.CreatePostFailed => "Failed to create post.",
                _ => "Unknown error."

            };
        }
    }
}
