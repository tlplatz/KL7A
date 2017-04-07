using System;
using System.IO;
using KL7A.Configuration;

namespace KL7A.Utility
{
    public static class IoUtil
    {
        private static string MonthlySettingFileName(MonthlySettings monSet, string ext)
        {
            return string.Format("Settings_{0}_{1:MMMyyyy}{2}", monSet.Name, monSet.Date, ext);
        }
        private static void SaveFile(string data, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(data);
                }
            }
        }
        private static void SaveMonthlyFiles(MonthlySettings settings, string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string xmlPath = Path.Combine(folder, MonthlySettingFileName(settings, ".xml"));
            settings.Save(xmlPath);

            string txtPath = Path.Combine(folder, MonthlySettingFileName(settings, ".txt"));
            SaveFile(settings.ToString(), txtPath);
        }

        public static void SaveYearlyFiles(string name, string classification, int year, string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            for (int m = 1; m <= 12; m++)
            {
                DateTime dt = new DateTime(year, m, 1);
                string folderName = string.Format("{0} {1:MMM yyyy}", name, dt);
                string monthFolderName = Path.Combine(folder, folderName);
                MonthlySettings monSet = MonthlySettings.Random(name, classification, dt);
                SaveMonthlyFiles(monSet, monthFolderName);
            }
        }
        public static void SaveYearlyFiles(YearlySettings ys, string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            foreach (MonthlySettings monSet in ys.MonthlySettings)
            {
                string folderName = string.Format("{0} {1:MMM yyyy}", monSet.Name, monSet.Date);
                string monthFolderName = Path.Combine(folder, folderName);
                SaveMonthlyFiles(monSet, monthFolderName);
            }
        }
    }
}
