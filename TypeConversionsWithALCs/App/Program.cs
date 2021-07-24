using static System.Console;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System;

namespace App
{
    static class Program
    {
        static void Main(string[] args)
        {
            AssemblyLoadContext defaultContext = AssemblyLoadContext.Default;
            AssemblyLoadContext redContext = new AssemblyLoadContext(redContextName);
            AssemblyLoadContext blueContext = new AssemblyLoadContext(blueContextName);

            Assembly blueLib = blueContext.LoadFromAssemblyPath(LibPath);
            Assembly redLib = redContext.LoadFromAssemblyPath(LibPath);

            defaultContext.FindLib();
            blueContext.FindLib();
            redContext.FindLib();

            object? blueTime = blueLib.GetTimeNow();
            object? redTime = redLib.GetTimeNow();
            WriteLine(blueTime?.ToString() ?? string.Empty);

            try
            {
                // The static cast throws an InvalidCastException.
                Lib.Time? blueTimeRef = (Lib.Time?)blueTime;
            }
            catch (System.InvalidCastException e)
            {
                WriteLine(e);
                WriteLine("Note that the types of blueTime and redTime {0}.",
                    blueTime?.GetType().Equals(redTime) ?? false ? "equal" : "differ");
            }

            // Reflection in the extension method below.
            blueTime?.ToUnixTimeMilliseconds()?.WriteUnixTime();

            // Dynamic binding.
            dynamic? dynamicBlueTime = blueTime;
            WriteUnixTime(dynamicBlueTime?.ToUnixTimeMilliseconds());

            // Using common types loaded to the default load context
            // such as a delegate
            AppLib.UnixTimeMilliseconds? func = dynamicBlueTime?.UnixTimeMilliseconds;
            func?.Invoke().WriteUnixTime();
            // or an interface.
            AppLib.Time? blueTimeAgain = (AppLib.Time?)blueTime;
            blueTimeAgain?.ToUnixTimeMilliseconds().WriteUnixTime();

            // Or marshal to the Lib.Time in the default ALC.
            Lib.Time? copiedBlueTime = func is not null ? Lib.Time.Of(func()) : null;
            copiedBlueTime?.ToUnixTimeMilliseconds().WriteUnixTime();
        }

        const string blueContextName = "blue";

        const string redContextName = "red";

        static void WriteUnixTime(this long time)
        => WriteLine($"UNIX time is {time}ms.");

        static long? ToUnixTimeMilliseconds(this object? libTime)
        => (long?)libTime?.GetType()?.GetRuntimeMethod("ToUnixTimeMilliseconds", new Type[0])?.Invoke(libTime, null);

        static string LibPath
        => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Lib.dll");

        static void FindLib(this AssemblyLoadContext context)
        => WriteLine($"The {context.Name} ALC has {context.GetLib()?.GetName().ToString() ?? "no Lib assembly"}.");

        static Assembly? GetLib(this AssemblyLoadContext defaultContext)
        => defaultContext.Assemblies.Where(a => a.FullName?.StartsWith("Lib,") ?? false).FirstOrDefault();

        static object? GetTimeNow(this Assembly lib)
        => lib.GetType("Lib.Time")?.GetRuntimeProperty("Now")?.GetValue(null);
    }
}