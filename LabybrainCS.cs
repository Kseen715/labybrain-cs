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
            /// <summary>
            /// Initializes the python engine
            /// </summary>
            /// <param name="pathToPythonDll">The path to the python dll</param>
            /// <returns>1 if successful, 0 if not</returns>
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

        public static T PyCast<T>(dynamic py_obj)
        {
            /// <summary>
            /// Casts a python object to an T
            /// </summary>
            /// <param name="py_obj_int">The python object to cast</param>
            /// <returns>The casted T</returns>
            try
            {
                T res;
                using (Py.GIL())
                {
                    res = (T)py_obj;
                }
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                throw;
            }
        }

        public static List<T> PyCastList<T>(dynamic py_obj)
        {
            /// <summary>
            /// Casts a python object to an List< T>
            /// </summary>
            /// <param name="py_obj_int">The python object to cast</param>
            /// <returns>The casted List< T></returns>
            try
            {
                List<T> res = new();
                using (Py.GIL())
                {
                    foreach (var item in py_obj)
                    {
                        res.Add((T)item);
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                throw;
            }
        }
        public static void PrintList<T>(this IEnumerable<T> list)
        {
            /// <summary>
            /// Prints a list to the console
            /// </summary>
            /// <param name="list">The list to print</param>
            /// <returns>void</returns>
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

        static void Main(string[] args)
        {
            // Initialize the python engine
            PyInitialize("python311.dll");
            // Change the current working directory to the labybrain folder
            PyChDir("labybrain");

            // Import the labybrain module
            var lb = PyImport("labybrain.py");

            dynamic py_res;
            try
            {
                // This part will be executed in python
                using (Py.GIL())
                {
                    lb.load_config();
                    lb.load_model_callback("0004", 1);
                    py_res = lb.predict_callback_mult(1000, 0);
                }

                PrintList(PyCastList<int>(py_res));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }

            // Shutdown the python engine
            PythonEngine.Shutdown();
        }
    }
}