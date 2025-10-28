using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStruct.Shared
{
    public class PhysicalFile
    {
        public string FileNameWithoutDotAndExtension { get; set; }
        public string Extension { get; set; }
        public byte[] FileContent { get; set; }
    }
}
