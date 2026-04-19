namespace FCG.Platform.Shared.Logging
{
    public static class LogMessages
    {
        //User
        public static string InvalidUserInputs() => "Message: Invalid inputs to User.";
        public static string NullOrEmptyUserEmail() => "Message: The Email field is null, empty, or whitespace.";
        public static string UpdatingErrorUser(Exception ex) => $"Message: Error updating User: {ex.Message}";
        public static string UpdatingSuccessUser() => "Message: Successfully updated User.";
        public static string UserNotFound(string action) => $"Message: User not found for {action} action.";
        public static string AddingUserError(Exception ex) => $"Message: Error adding a new User: {ex.Message}";
        public static string AddingUserSuccess() => "Message: Successfully added a new User.";
        public static string DeleteUserError(Exception ex) => $"Message: Error to delete a User: {ex.Message}";
        public static string DeleteUserSuccess() => "Message: Delete with success User.";
        public static string GetAllUserError(Exception ex) => $"Message: Error to loading the list User: {ex.Message}";
        public static string GetAllUserSuccess() => "Message: GetAll with success User.";
        public static string UserNotFound() => "Message: User not found.";
        public static string PasswordInvalid() => "Message: Incorrect current password.";
        public static string UpdatingSuccessPassword() => "Message: Password updated successfully.";
    }
}