using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace AuthService.Models
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Teacher = "Teacher";
        public const string Assistant = "Assistant";
        public const string Student = "Student";

        public static readonly string[] AllRoles =
        {
            Admin,
            Manager,
            Student,
            Teacher,
            Assistant
        };
    }
}
