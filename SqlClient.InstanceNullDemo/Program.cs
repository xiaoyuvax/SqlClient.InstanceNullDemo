// See https://aka.ms/new-console-template for more information
using DemoPlugin;
using System.Reflection;

internal class Program
{
    /// <summary>
    /// 获取指定接口的所有插件类型
    /// </summary>
    /// <typeparam name="T">必须是接口</typeparam>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetPlugInTypes(IEnumerable<Assembly> assemblies) => assemblies.SelectMany(i => i.GetTypes()).Where(i => i.IsAssignableTo(typeof(IPlugin)));

    /// <summary>
    /// 加载插件程序集
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Assembly> LoadPlugInAssemblies(string rootDir) =>
       Directory.GetFiles(rootDir, "Plugin.*.dll", SearchOption.AllDirectories).Select(i =>
       {
           try { return Assembly.LoadFile(i); }
           catch (Exception ex) { Console.WriteLine(ex.Message); }
           return null;
       }).Where(i => i != null);

    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, asm) =>
        {
            Assembly assembly = null;

            var dirs = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory).ToList();
            dirs.Insert(0, AppDomain.CurrentDomain.BaseDirectory);

            foreach (var p in dirs.Select(i => Path.Combine(i, new AssemblyName(asm.Name).Name + ".dll")).Where(i => File.Exists(i)))
                try
                {
                    assembly = Assembly.LoadFrom(p);
                    if (asm.Name.StartsWith("Microsoft.Data.SqlClient"))
                    {
                        var type = assembly.GetType(new AssemblyName(asm.Name).Name + ".SqlClientFactory");
                        var ci = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
                        if (ci.Length == 0) Console.WriteLine("Instance property not found!");
                    }
                    break;
                }
                catch (Exception ex) { Console.WriteLine(ex.Message, ex); }

            return assembly;
        };

        var t0 = GetPlugInTypes(LoadPlugInAssemblies(Environment.CurrentDirectory)).FirstOrDefault();
        if (t0 != null)
            try
            {
                var t = (IPlugin)Activator.CreateInstance(t0, args);
                t.Start();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
    }
}