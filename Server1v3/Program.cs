using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace KursovoyProektserver1
{
    class Program
    {
        public static Mutex m = new Mutex(false, "kursCommon");
        public static Mutex m1 = new Mutex(true, "kurs1");
        static string FilePath;

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        private static void Main(string[] args)
        {
            m1.WaitOne();
            if (args.Length > 0)
            {
                FilePath = args[0];
            }
            if (m.WaitOne(50000))
            {
                POINT lpPoint;
                GetCursorPos(out lpPoint);
                POINT cursorPos = (POINT)lpPoint;

                string mouse = $@"Mouse Coorditats: {cursorPos.X} {cursorPos.Y}";
                string error = $@"Last Error {Error()}";
                //Console.WriteLine(mouse);
                //Console.WriteLine(error);
                WriteToFile(mouse);
                WriteToFile(error);
                m.ReleaseMutex();
            }
            else
            {
              //Console.WriteLine("Файл {0} используется другим процессом, запись невозможна", FilePath);
                m1.ReleaseMutex();
            }
            m1.ReleaseMutex();
            Console.ReadKey();
        }
        private static int Error() => new EventLog("System").Entries.Cast<EventLogEntry>().FirstOrDefault().EventID;
        public struct POINT
        {
            public int X;
            public int Y;
        }
        static void WriteToFile(string text)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FilePath, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(text);
                    //Console.WriteLine("Запись в файл успешно выполнена Сервером 1");
                }
            }
            catch
            {
                Console.WriteLine("Ошибка записи в файл");
            }
        }
    }
}