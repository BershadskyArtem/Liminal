namespace Liminal.Auth.Requirements;

public class PolicyDefaults
{
    public static readonly List<string> Admin = [RoleDefaults.Admin, RoleDefaults.SuperAdmin];
    public static readonly string AdminName = "admin";

    public static readonly List<string> PaidAndAdmin =
    [
        RoleDefaults.Trial,
        RoleDefaults.Medium,
        RoleDefaults.Premium,
        RoleDefaults.Admin,
        RoleDefaults.SuperAdmin
    ];
    
    public static readonly string PaidAndAdminName = "paidandadmin";

    public static readonly List<string> SuperAdmin = [RoleDefaults.SuperAdmin];
    public static readonly string SuperAdminName = "superadmin";


    public static string ConfirmedAccount { get; set; } = "confirmedaccount";
}