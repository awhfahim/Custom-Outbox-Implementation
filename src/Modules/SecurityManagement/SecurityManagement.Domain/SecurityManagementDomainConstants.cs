namespace SecurityManagement.Domain;

public static class SecurityManagementDomainConstants
{
    public static class AuthorizableRoleEntity
    {
        public const string DbTableName = "AuthorizableRoles";
        public const int LabelMaxLength = 500;
    }

    public static class RolePermissionEntity
    {
        public const string DbTableName = "RolePermissions";
    }

    public static class UserRoleEntity
    {
        public const string DbTableName = "UserRoles";
    }

    public static class AuthorizablePermissionEntity
    {
        public const string DbTableName = "AuthorizablePermissions";
        public const int LabelMaxLength = 1000;
    }

    public static class AuthorizablePermissionGroupEntity
    {
        public const string DbTableName = "AuthorizablePermissionGroups";
        public const int LabelMaxLength = 1000;
    }

    public static class UserEntity
    {
        public const string DbTableName = "Users";
        public const int FullNameMaxLength = 100;
        public const int EmailMaxLength = 256;
        public const int UserNameMaxLength = 256;
        public const int PhoneNumberMaxLength = 255;
        public const int AddressMaxLength = 1000;
        public const int ProfilePictureUriMaxLength = 2048;
    }
}
