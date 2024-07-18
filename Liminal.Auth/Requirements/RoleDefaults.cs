namespace Liminal.Auth.Requirements;

public static class RoleDefaults
{
    public static readonly string NotConfirmed = "not_confirmed";
    public static readonly string Basic = "basic";
    public static readonly string Trial = "trial";
    public static readonly string Medium = "medium";
    public static readonly string Premium = "premium";
    public static readonly string Admin = "admin";
    public static readonly string SuperAdmin = "super_admin";

    public static bool IsAdministrative(string roleName)
    {
        if (roleName == Admin || roleName == SuperAdmin)
        {
            return true;
        }

        return false;
    }
}