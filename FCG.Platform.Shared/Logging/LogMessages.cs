namespace FCG.Platform.Shared.Logging
{
    public static class LogMessages
    {
        #region User Validation

        public static string InvalidUserInputs() => "Invalid user data.";

        #endregion

        #region User Not Found

        public static string CannotPerformActionOnUser(string action, int userId) => $"Cannot {action} user. User with id {userId} was not found.";

        #endregion

        #region User CRUD

        public static string AddingUserError(Exception ex) => $"Error adding user. Details: {ex.Message}";
        public static string AddingUserSuccess() => "User added successfully.";

        public static string UpdatingErrorUser(Exception ex) => $"Error updating user. Details: {ex.Message}";
        public static string UpdatingSuccessUser() => "User updated successfully.";

        public static string DeleteUserError(Exception ex) => $"Error deleting user. Details: {ex.Message}";
        public static string DeleteUserSuccess() => "User deleted successfully.";

        public static string GetAllUserError(Exception ex) => $"Error retrieving users list. Details: {ex.Message}";
        public static string GetAllUserSuccess() => "Users retrieved successfully.";

        public static string GetByUserIdError(Exception ex) => $"Error retrieving user by id. Details: {ex.Message}";
        public static string GetByUserIdSuccess() => "User retrieved successfully.";

        #endregion

        #region Password

        public static string PasswordInvalid() => "Incorrect current password.";
        public static string UpdatingSuccessPassword() => "Password updated successfully.";

        #endregion
    }
}