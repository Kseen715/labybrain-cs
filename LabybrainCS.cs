using System;
using Python.Runtime;


namespace LabybrainCS
{
    public static class LabybrainCS
    {
        enum log_mode
        {
            QUIET = 0,
            INFO = 1,
            WARNING = 2,
            ERROR = 3,
            DEBUG = 4,
        }

        public static int PyInitialize(string pathToPythonDll)
        {
            // set the path to the python dll
            // TODO: make this path relative
            try
            {
                Runtime.PythonDLL = pathToPythonDll;
                PythonEngine.Initialize();
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                return 0;
            }
        }

        public static dynamic PyImport(string filePath)
        {
            /// <summary>
            /// Imports a python module from a given file path
            /// </summary>
            /// <param name="filePath">The path to the python file</param>
            /// <returns>The imported module</returns>
            /// <example>
            /// <code>
            /// dynamic module = PyImport("labybrain/labybrain.py");
            /// </code>
            /// Than you can use the module like in Python.
            /// <code>
            /// module.main();
            /// </code>
            /// </example>
            try
            {
                dynamic module;
                using (Py.GIL())
                {
                    // simple imports for glueing python's and c#'s system 
                    // together
                    dynamic os = Py.Import("os");
                    dynamic sys = Py.Import("sys");
                    sys.path.append(
                        os.path.dirname(
                            os.path.expanduser(filePath)));
                    module = Py.Import(
                        Path.GetFileNameWithoutExtension(filePath));
                }
                return module;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                return 0;
            }
        }

        public static int PyChDir(string dir)
        {
            /// <summary>
            /// Changes the current working directory to the given directory
            /// </summary>
            /// <param name="dir">The directory to change to</param>
            /// <returns>1 if successful, 0 if not</returns>
            try
            {
                using (Py.GIL())
                {
                    dynamic os = Py.Import("os");
                    os.chdir(dir + "/");
                }
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                return 0;
            }
        }
        static void Main(string[] args)
        {
            PyInitialize("python311.dll");
            PyChDir("labybrain");
            var lb = PyImport("labybrain.py");
            List<int> res = new();
            try
            {
                using (Py.GIL())
                {
                    lb.load_config();
                    lb.load_model_callback("0004", 1);
                    for (int i = 0; i < 50; i++)
                    {
                        dynamic result = lb.predict_callback(0);
                        // FIXME: this looks awful AF + it's very unsafe
                        // but works for now
                        string action = result.ToString();
                        int action_int = int.Parse(action);
                        res.Add(action_int);
                    }
                    Console.WriteLine();
                    // lb.EXIT();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            PythonEngine.Shutdown();
            PrintList(res);

        }
        public static void PrintList<T>(this IEnumerable<T> list)
        {
            System.Console.Write("[");
            for (int i = 0; i < list.Count(); i++)
            {
                System.Console.Write(list.ElementAt(i));
                if (i < list.Count() - 1)
                {
                    System.Console.Write(", ");
                }
            }
            System.Console.Write("]");
        }
    }

}