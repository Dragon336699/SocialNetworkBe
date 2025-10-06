namespace Domain.Enum.User.Functions
{
    public enum ResetPasswordEnum
    {
        UserNotFound,
        ResetPasswordFail,
        ResetPasswordSuccess,
    }

    public static class ResetPasswordMessage
    {
        public static string GetMessage(this ResetPasswordEnum status)
        {
            return status switch
            {
                ResetPasswordEnum.UserNotFound => "User not found!",
                ResetPasswordEnum.ResetPasswordFail => "Reset password fail. Please try again!",
                ResetPasswordEnum.ResetPasswordSuccess => "Reset password successfully!",
                _ => "An unknown error occurred"
            };
        }
    }
}
