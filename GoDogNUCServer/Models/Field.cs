using System;
using System.Collections.Generic;
using System.Text;

namespace GoDogNUCServer.Models
{
    public class Field
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public NUCDevice NUC { get; set; }
    }
}
