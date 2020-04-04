using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MMCApi.Model
{
    public class FileDirectory
    {
        /// <summary>
        /// Create Directory 
        /// </summary>
        /// <param name="dirPath">directory path</param>
        public void CreateDirectory(string dirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
                dir.Create();
        }
        /// <summary>
        /// Delete file if exists
        /// </summary>
        /// <param name="fileName">file path</param>
        public void DeleteFile(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists)
                fileInfo.Delete();


        }

    }
}
