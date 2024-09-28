using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Scripts.Analytics {
    public static class CSVManager  {
        private const string SEPARATOR = ",";
        private static string reportFileName = "spiral-analytics.csv";
        private static string reportDirectoryName = "Analytics";

        private static string[] reportHeaders = {
            "Version",
            "Player #",
            "Player Won"
        };

        private static string timeStampHeader = "Time";
        
#region Interactions
        public static void CreateReport() {
            VerifyDirectory();
            using (StreamWriter sw = File.CreateText(GetFilePath())) {
                string headerLine = string.Join(SEPARATOR, reportHeaders);
                headerLine += SEPARATOR + timeStampHeader;
                sw.WriteLine(headerLine);
            }
        }

        public static void AppendToReport(List<string> strings) {
            VerifyDirectory();
            VerifyFile();
            using (StreamWriter sw = File.AppendText(GetFilePath())) {
                string data = string.Join(SEPARATOR, strings);
                data += SEPARATOR + GetTimestamp();
                sw.WriteLine(data);
            }
        }
#endregion

#region Operations
        static void VerifyDirectory() {
            string dir = GetDirectoryPath();
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
        }

        static void VerifyFile() {
            string file = GetFilePath();
            if (!File.Exists(file)) {
                CreateReport();
            }
        }
#endregion
        
#region Queries
        static string GetDirectoryPath() {
            return $"{Application.dataPath}/{reportDirectoryName}";
        }

        static string GetFilePath() {
            return $"{GetDirectoryPath()}/{reportFileName}";
        }
        static string GetTimestamp() {
            return System.DateTime.Now.ToString();
        }
#endregion
    }
}