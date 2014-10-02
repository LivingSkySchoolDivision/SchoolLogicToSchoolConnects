using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLDataLib
{
    public static class FileHelpers
    {
        public static bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public static void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }

        public static void SaveFile(MemoryStream fileContent, string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            if (fileName.Length <= 1) return;

            using (FileStream fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
            {
                fileContent.WriteTo(fileStream);
                fileStream.Flush();
            }
        }
    }
}
