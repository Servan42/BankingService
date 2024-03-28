﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.FileSystem.API.Interfaces
{
    public interface IFileSystemService
    {
        public List<string> ReadAllLines(string filePath);
        public void WriteAllLinesOverride(string filePath, List<string> lines);
    }
}