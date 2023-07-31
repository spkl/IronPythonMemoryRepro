using System.Reflection;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;

namespace IronPythonMemoryRepro
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("How many script executions?");
                int n = int.Parse(Console.ReadLine()!);
                if (n == 0)
                {
                    return;
                }

                Program.CallScript(n);
            }
        }

        private static void CallScript(int n)
        {
            for (int i = 0; i < n; i++)
            {
                Program.CallScript();
                Console.WriteLine(i + 1);
            }
        }

        public static void CallScript()
        {
            ScriptEngine engine = IronPython.Hosting.Python.CreateEngine(
                // Without these options, the object retention graph looks different,
                // but the problem still occurs.
                new Dictionary<string, object>
                    {
                        ["LightweightScopes"] = ScriptingRuntimeHelpers.True,
                        ["FullFrames"] = ScriptingRuntimeHelpers.True
                    });
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            engine.SetSearchPaths(new[] { assemblyDir, Path.Combine(assemblyDir, "lib") });

            ScriptScope scope = engine.CreateScope();
            ScriptSource source = engine.CreateScriptSourceFromFile("MyScript.py");
            source.Execute(scope);

            Func<string, string> myMethod = scope.GetVariable<Func<string, string>>("MyMethod");
            myMethod("dummy");
            scope.Engine.Runtime.Shutdown();
        }
    }
}