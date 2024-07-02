using System.Reflection;

namespace Liminal.Auth.EntityFrameworkCore;

public static class AssemblyMarker
{
    public static Assembly Current => typeof(AssemblyMarker).Assembly;
}