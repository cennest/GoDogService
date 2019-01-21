using System;
using System.Collections.Generic;
using System.Text;

namespace GoDogNUCServer.Models
{
    public class Facility
    {
        public Facility()
        {
            Fields = new List<Field>();
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public List<Field> Fields { get; set; }
    }
}
