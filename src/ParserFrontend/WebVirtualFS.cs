﻿using PdfTextReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend
{
    class WebVirtualFS : IVirtualFS
    {
        const string FILEFOLDERWWW = "wwwroot/files";

        public Stream OpenReader(string virtualfile)
        {
            string filename = GetLocalFilename(virtualfile);
            
            return new FileStream(filename, FileMode.Open, FileAccess.Read);
        }

        public Stream OpenWriter(string virtualfile)
        {
            string filename = GetLocalFilename(virtualfile);
            
            string folderName = Path.GetDirectoryName(filename);
            if (!Directory.Exists(folderName))
            {
                if (Path.IsPathRooted(folderName))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    DirectoryInfo directory = new DirectoryInfo(".");
                    directory.CreateSubdirectory(folderName);
                }
            }

            return new FileStream(filename, FileMode.Create);
        }

        string GetLocalFilename(string virtualfile)
        {
            if (virtualfile.Contains(".."))
                throw new FileNotFoundException("Invalid path");

            return $"{FILEFOLDERWWW}/{virtualfile}";
        }
    }
}
