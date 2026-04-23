using FCG.Platform.Domain.Entities.Entity;

namespace FCG.Platform.Shared.Logging
{
    public static class LogMessages
    {
        #region User Validation

            public static string InvalidUserInputs() => "Invalid user data.";

        #endregion

        #region User Not Found

            public static string CannotPerformActionOnUser(string action, string userId) => $"Cannot {action} user. User with id {userId} was not found.";

        #endregion

        #region User CRUD

            public static string AddingUserError(Exception ex) => $"Error adding user. Details: {ex.Message}";
            public static string AddingUserSuccess(UserEntity userEntity) => $"User name:{userEntity.Name} - id:{userEntity.Id} added successfully.";

            public static string UpdatingErrorUser(Exception ex) => $"Error updating user. Details: {ex.Message}";
            public static string UpdatingSuccessUser(UserEntity userEntity) => $"User name:{userEntity.Name} - id:{userEntity.Id} updated successfully.";

            public static string DeleteUserError(Exception ex) => $"Error deleting user. Details: {ex.Message}";
            public static string DeleteUserSuccess(UserEntity userEntity) => $"User name:{userEntity.Name} - id:{userEntity.Id} deleted successfully.";

            public static string GetAllUserError(Exception ex) => $"Error retrieving users list. Details: {ex.Message}";
            public static string GetAllUserSuccess() => "Users retrieved successfully.";

            public static string GetByUserIdError(Exception ex) => $"Error retrieving user by id. Details: {ex.Message}";
            public static string GetByUserIdSuccess(UserEntity userEntity) => $"User name:{userEntity.Name} - id:{userEntity.Id} retrieved successfully.";

        #endregion

        #region Password

            public static string PasswordInvalid() => "Incorrect current password.";
            public static string UpdatingSuccessPassword() => "Password updated successfully.";

        #endregion

        #region Game Validation

            public static string InvalidGameInputs() => "Invalid game data.";

        #endregion

        #region Game Not Found

            public static string CannotPerformActionOnGame(string action, int gameId) => $"Cannot {action} Game. Game with id {gameId} was not found.";

        #endregion

        #region Game CRUD

            public static string AddingGameError(Exception ex) => $"Error adding Game. Details: {ex.Message}";
            public static string AddingGameSuccess() => "Game added successfully.";

            public static string UpdatingErrorGame(Exception ex) => $"Error updating Game. Details: {ex.Message}";
            public static string UpdatingSuccessGame() => "Game updated successfully.";

            public static string DeleteGameError(Exception ex) => $"Error deleting Game. Details: {ex.Message}";
            public static string DeleteGameSuccess() => "Game deleted successfully.";

            public static string GetAllGameError(Exception ex) => $"Error retrieving Games list. Details: {ex.Message}";
            public static string GetAllGameSuccess() => "Games retrieved successfully.";

            public static string GetByGameIdError(Exception ex) => $"Error retrieving Game by id. Details: {ex.Message}";
            public static string GetByGameIdSuccess() => "Game retrieved successfully.";

        #endregion
    }
}