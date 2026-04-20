using System.ComponentModel;

namespace FCG.Platform.Shared.DomainErrors
{
    public enum UserErrors
    {
        [Description("'Email' can not be null or empty!")]
        User_Error_EmailCanNotBeNullOrEmpty,

        [Description("'Email' invalid format!")]
        User_Error_InvalidEmailFormat,

        [Description("'Password' can not be null or empty!")]
        User_Error_PasswordCanNotBeNullOrEmpty,

        [Description("'Password' must be at least 8 characters long!")]
        User_Error_PasswordLengthLessEight,

        [Description("'Password' must contain at least 1 letter, 1 number and 1 special character!")]
        User_Error_PasswordInvalid,

        [Description("'Name' can not be null or empty!")]
        User_Error_NameCanNotBeNullOrEmpty,

        [Description("'Name' must be at least 8 characters long!")]
        User_Error_NameLengthLessEight
    }
}