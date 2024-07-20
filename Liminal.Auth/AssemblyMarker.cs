using System.Reflection;

namespace Liminal.Auth;

public class AssemblyMarker
{
    public static Assembly Assembly => typeof(AssemblyMarker).Assembly;
}