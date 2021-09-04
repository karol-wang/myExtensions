using System;
using System.IO;

namespace MyExtension
{
    public class Logs
    {
        private readonly string LogPath = $@"{System.Environment.CurrentDirectory}\Log\";
        private string LogFile = "";
        public Logs()
        {
            LogFile = $@"{LogPath}log{DateTime.Today:yyyy-MM-dd}.txt";
        }

        private void CheckFile()
        {
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
            if (!File.Exists(LogFile))
            {
                File.Create(LogFile).Close();
            }
        }

        public void Write(LogMessagesType type, string logMessage)
        {
            CheckFile();

            StreamWriter sw = File.AppendText(LogFile);
            sw.WriteLine($"{DateTime.Now:yyyy-MM-dd  hh:mm:ss} | {type} | {logMessage.Replace('\r', ' ')}");
            sw.Close();
        }

    }
    public enum LogMessagesType
    {
        info,
        error,
    }
}
