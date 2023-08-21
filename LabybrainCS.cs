// =============================================================================
// 
// Kseen715, 2023
// 
// This module wraps around the python module labybrain.py and provides a
// simple interface to use it in C#.
//
// Links:
//     labybrain - https://github.com/Kseen715/labybrain
//
// =============================================================================



// =============================================================================
//
// Define one of this things to enable the corresponding log level:
//     LBCS_DEBUG   - Enables debug logs and all logs below
//     LBCS_INFO    - Enables info logs and all logs below
//     LBCS_WARNING - Enables warning logs and all logs below
//     LBCS_ERROR   - Enables error logs only
//     ~nothing~    - No logs
//
#if !LBCS_DEBUG
#define LBCS_DEBUG
#endif // !LBCS_DEBUG
//
#if !LBCS_INFO
#define LBCS_INFO
#endif // !LBCS_INFO
//
#if !LBCS_WARNING
#define LBCS_WARNING
#endif // !LBCS_WARNING
//
#if !LBCS_ERROR
#define LBCS_ERROR
#endif // !LBCS_ERROR
//
// =============================================================================

using System;
using Python.Runtime;
using System.Text.Json;

namespace LabybrainCS
{
    public static class LabybrainCS
    {
        class LogMode
        {
            public readonly static int DEBUG = 0;
            public readonly static int INFO = 1;
            public readonly static int WARNING = 2;
            public readonly static int ERROR = 3;
            public readonly static int QUIET = 4;
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

#if LBCS_DEBUG || LBCS_INFO
                Logger.LogInfo("Initialized python engine");
#endif // LBCS_DEBUG || LBCS_INFO

                return 1;
            }
            catch (Exception e)
            {

#if LBSC_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR
                Logger.LogError(e.ToString());
#endif // LBSC_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR

                return 0;
            }
        }

        public static int PyShutdown()
        {
            /// <summary>
            /// Shuts down the python engine
            /// </summary>
            /// <returns>1 if successful, 0 if not</returns>
            try
            {
                PythonEngine.Shutdown();

#if LBCS_DEBUG || LBCS_INFO
                Logger.LogInfo("Shut down python engine");
#endif // LBCS_DEBUG || LBCS_INFO

                return 1;
            }
            catch (Exception e)
            {

#if LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR
                Logger.LogError(e.ToString());
#endif // LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR

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

#if LBCS_DEBUG || LBCS_INFO
                Logger.LogInfo("Imported module \"" + filePath + "\"");
#endif // LBCS_DEBUG || LBCS_INFO

                return module;
            }
            catch (Exception e)
            {

#if LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR
                Logger.LogError(e.ToString());
#endif // LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR

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

#if LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR
                Logger.LogError(e.ToString());
#endif // LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR

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

#if LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR
                Logger.LogError(e.ToString());
#endif // LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR

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

#if LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR
                Logger.LogError(e.ToString());
#endif // LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR

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
                System.Console.Write(
                    new string(',', Convert.ToInt32(i < list.Count() - 1)) +
                    new string(' ', Convert.ToInt32(i < list.Count() - 1))
                    );
            }
            System.Console.Write("]\n");
        }


        public static Dictionary<string, float> LoadConfig()
        {
            /// <summary>
            /// Loads the config from the config file
            /// </summary>
            /// <returns>The config as a dictionary</returns>
            Dictionary<string, float> res = new();
            try
            {
                string json = File.ReadAllText("labybrain/config.json");
                res = JsonSerializer
                    .Deserialize<Dictionary<string, float>>(json)
                    ?? new Dictionary<string, float>();

#if LBCS_DEBUG || LBCS_INFO
                Logger.LogInfo("Loaded config");
#endif // LBCS_DEBUG || LBCS_INFO

            }
            catch (Exception e)
            {

#if LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR
                Logger.LogError(e.ToString());
#endif // LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR

            }
            return res;
        }


        public static void PrintDictionary<TKey, TValue>(
            Dictionary<TKey, TValue> dict)
            where TKey : notnull
            where TValue : notnull
        {
            /// <summary>
            /// Prints a dictionary to the console
            /// </summary>
            /// <param name="dict">The dictionary to print</param>
            /// <returns>void</returns>
            System.Console.WriteLine("{");
            foreach (var pair in dict)
            {
                System.Console.WriteLine("\"{0}\": {1}",
                    pair.Key,
                    pair.Value);
            }
            System.Console.WriteLine("}");
        }

        public static void SaveConfig(Dictionary<string, float> dict)
        {
            /// <summary>
            /// Saves the config from a dictionary
            /// </summary>
            /// <param name="dict">The dictionary to set the config from</param>
            /// <returns>void</returns>
            try
            {
                string json = JsonSerializer.Serialize(dict);
                File.WriteAllText("labybrain/config.json", json);

#if LBCS_DEBUG || LBCS_INFO
                Logger.LogInfo("Saved config");
#endif // LBCS_DEBUG || LBCS_INFO

            }
            catch (Exception e)
            {

#if LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR
                Logger.LogError(e.ToString());
#endif // LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR

            }
        }

        public static void SmollMain()
        {
            Dictionary<string, float> res = LoadConfig();
            // PrintDictionary(res);
            res["randomness"] = 0.3f;
            // res["dir_1"] = 0.434f;
            // res["dir_2"] = 0.434f;
            // res["dir_3"] = 0.549f;
            // res["dir_4"] = 0.441f;
            // res["dir_5"] = 0.427f;
            // res["dir_6"] = 0.727f;

            res["dir_1"] = 0.5f;
            res["dir_2"] = 0.5f;
            res["dir_3"] = 0.5f;
            res["dir_4"] = 0.5f;
            res["dir_5"] = 0.5f;
            res["dir_6"] = 0.5f;

            SaveConfig(res);

            System.Console.WriteLine("Config: ");
            PrintDictionary(res);

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
                    lb.load_model_callback("0004");
                    py_res = lb.predict_callback_mult(100);
                }

                PrintList(PyCastList<int>(py_res));
            }
            catch (Exception e)
            {

#if LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR
                Logger.LogError(e.ToString());
#endif // LBCS_DEBUG || LBCS_INFO || LBCS_WARNING || LBCS_ERROR

            }

            // Shutdown the python engine
            PyShutdown();
        }
        static void Main(string[] args)
        {
            SmollMain();
        }
    }
}