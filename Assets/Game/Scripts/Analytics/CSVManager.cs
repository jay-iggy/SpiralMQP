using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Scripts.Analytics {
    public static class CSVManager  {
        private const char SEPARATOR = ',';
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
                string finalString = "";
                
                foreach (string header in reportHeaders) {
                    if (finalString != "") {
                        finalString += SEPARATOR;
                    }
                    finalString += header;
                }

                finalString += SEPARATOR + timeStampHeader;
                sw.WriteLine(finalString);
            }
        }

        public static void AppendToReport(List<string> strings) {
            VerifyDirectory();
            VerifyFile();
            using (StreamWriter sw = File.AppendText(GetFilePath())) {
                string finalString = "";
                foreach (string str in strings) {
                    if (finalString != "") {
                        finalString += SEPARATOR;
                    }

                    finalString += str;
                }

                finalString += SEPARATOR + GetTimestamp();
                sw.WriteLine(finalString);
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