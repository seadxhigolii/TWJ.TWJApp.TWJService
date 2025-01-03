﻿namespace TWJ.TWJApp.TWJService.Common.Constants
{
    public class ValidatorMessages
    {

        public static string OneNotFound() => $"One or more data on list cannot be found";
        public static string NotEmpty(string msg) => $"{msg} must not be empty";
        public static string NotFound(string msg) => $"{msg} can not be found!";
        public static string MinLength(string msg, int length = ValidatorConditions.PasswordMinLength) => $"{msg} must be at last {length} characters";
        public static string FormatNotMatch(string msg) => $"Invalid {msg} format!";
        public static string AlreadyExists(string msg) => $"{msg} is already registered";
        public static string OnDeleteRestricted(string msg) => $"You cannot delete {msg} because has children!";
        public static string BoolValidation(string msg) => $"{msg} can be only true or false!";
        public static string PropertyNotNull(string msg) => $"{msg} cannot be null!";
        public static string DefaultExists(string msg) => $"You can not add {msg} because there is a default value for this category already";

        public const string IncorrectPassword = "Password Incorrect!";

        public const string CurrentPassowrdIncorrect = "Current password is incorrect!";

        public const string NoPermission = "You have no permission for this action!";

        public const string ExpiredToken = "Expired token!";
    }
}