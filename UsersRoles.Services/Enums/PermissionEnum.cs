namespace UsersRoles.Services.Enums
{
    public enum PermissionEnum
    {
        UsersRead = 1,
        UsersCreate = 2,
        UsersUpdate = 3,
        UsersDelete = 4,
        UsersChangePassword = 5,

        RolesRead = 6,
        RolesCreate = 7,
        RolesUpdate = 8,
        RolesDelete = 9,
        RolesAssign = 10,

        PermissionsRead = 11,
        PermissionsAssign = 12,

        AdminPanel = 13,

        ModelsAlgorithmsRead = 14,
        ModelsAlgorithmsCreate = 15,
        ModelsAlgorithmsUpdate = 16,
        ModelsAlgorithmsDelete = 17,

        CompExperimentsRead = 18,
        CompExperimentsCreate = 19
    }

    public static class PermissionEnumExtensions
    {
        public static string GetName(this PermissionEnum permission)
        {
            return permission switch
            {
                PermissionEnum.UsersRead => "Users.Read",
                PermissionEnum.UsersCreate => "Users.Create",
                PermissionEnum.UsersUpdate => "Users.Update",
                PermissionEnum.UsersDelete => "Users.Delete",
                PermissionEnum.UsersChangePassword => "Users.ChangePassword",

                PermissionEnum.RolesRead => "Roles.Read",
                PermissionEnum.RolesCreate => "Roles.Create",
                PermissionEnum.RolesUpdate => "Roles.Update",
                PermissionEnum.RolesDelete => "Roles.Delete",
                PermissionEnum.RolesAssign => "Roles.Assign",

                PermissionEnum.PermissionsRead => "Permissions.Read",
                PermissionEnum.PermissionsAssign => "Permissions.Assign",

                PermissionEnum.AdminPanel => "Admin.Panel",

                PermissionEnum.ModelsAlgorithmsRead => "ModelsAlgorithms.Read",
                PermissionEnum.ModelsAlgorithmsCreate => "ModelsAlgorithms.Create",
                PermissionEnum.ModelsAlgorithmsUpdate => "ModelsAlgorithms.Update",
                PermissionEnum.ModelsAlgorithmsDelete => "ModelsAlgorithms.Delete",

                PermissionEnum.CompExperimentsRead => "CompExperiments.Read",
                PermissionEnum.CompExperimentsCreate => "CompExperiments.Create",

                _ => throw new ArgumentOutOfRangeException(nameof(permission), permission, null)
            };
        }
    }
}
