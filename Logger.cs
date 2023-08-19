namespace LabybrainCS
{
    class Logger
    {
        class AnsiSys
        {
            public readonly static string CSI = "\x1b[";
            public readonly static string OSC = "\x1b]";
            public readonly static string BEL = "\a";
        }

        class AnsiFore
        {
            public readonly static int BLACK = 30;
            public readonly static int RED = 31;
            public readonly static int GREEN = 32;
            public readonly static int YELLOW = 33;
            public readonly static int BLUE = 34;
            public readonly static int MAGENTA = 35;
            public readonly static int CYAN = 36;
            public readonly static int WHITE = 37;
            public readonly static int RESET = 39;

            // These are fairly well supported, but not part of the standard.
            public readonly static int LIGHTBLACK_EX = 90;
            public readonly static int LIGHTRED_EX = 91;
            public readonly static int LIGHTGREEN_EX = 92;
            public readonly static int LIGHTYELLOW_EX = 93;
            public readonly static int LIGHTBLUE_EX = 94;
            public readonly static int LIGHTMAGENTA_EX = 95;
            public readonly static int LIGHTCYAN_EX = 96;
            public readonly static int LIGHTWHITE_EX = 97;
        }

        class AnsiBack
        {
            public readonly static int BLACK = 40;
            public readonly static int RED = 41;
            public readonly static int GREEN = 42;
            public readonly static int YELLOW = 43;
            public readonly static int BLUE = 44;
            public readonly static int MAGENTA = 45;
            public readonly static int CYAN = 46;
            public readonly static int WHITE = 47;
            public readonly static int RESET = 49;

            // These are fairly well supported, but not part of the standard.
            public readonly static int LIGHTBLACK_EX = 100;
            public readonly static int LIGHTRED_EX = 101;
            public readonly static int LIGHTGREEN_EX = 102;
            public readonly static int LIGHTYELLOW_EX = 103;
            public readonly static int LIGHTBLUE_EX = 104;
            public readonly static int LIGHTMAGENTA_EX = 105;
            public readonly static int LIGHTCYAN_EX = 106;
            public readonly static int LIGHTWHITE_EX = 107;
        }

        class AnsiStyle
        {
            public readonly static int BRIGHT = 1;
            public readonly static int DIM = 2;
            public readonly static int NORMAL = 22;
            public readonly static int RESET_ALL = 0;
        }

        public static void Log(string message)
        {
            Console.WriteLine("[LABYBRAIN-CS] "
                + AnsiSys.CSI + AnsiFore.GREEN + "m" + "TRACE: "
                + message + AnsiSys.CSI + AnsiStyle.RESET_ALL + "m");
        }
        public static void LogDebug(string message)
        {
            Console.WriteLine("[LABYBRAIN-CS] "
                + AnsiSys.CSI + AnsiFore.BLUE + "m" + "DEBUG: "
                + message + AnsiSys.CSI + AnsiStyle.RESET_ALL + "m");
        }
        public static void LogInfo(string message)
        {
            Console.WriteLine("[LABYBRAIN-CS] "
                + AnsiSys.CSI + AnsiStyle.RESET_ALL + "m" + "INFO: "
                + message + AnsiSys.CSI + AnsiStyle.RESET_ALL + "m");
        }
        public static void LogWarning(string message)
        {
            Console.WriteLine("[LABYBRAIN-CS] "
                + AnsiSys.CSI + AnsiFore.YELLOW + "m" + "WARNING: "
                + message + AnsiSys.CSI + AnsiStyle.RESET_ALL + "m");
        }

        public static void LogError(string message)
        {
            Console.WriteLine("[LABYBRAIN-CS] "
                + AnsiSys.CSI + AnsiFore.RED + "m" + "ERROR: "
                + message + AnsiSys.CSI + AnsiStyle.RESET_ALL + "m");
        }
        public static void LogCritical(string message)
        {
            Console.WriteLine("[LABYBRAIN-CS] "
                + AnsiSys.CSI + AnsiBack.RED + ";"
                + AnsiFore.WHITE + "m" + "CRITICAL: "
                + message + AnsiSys.CSI + AnsiStyle.RESET_ALL + "m");
        }
    }
}