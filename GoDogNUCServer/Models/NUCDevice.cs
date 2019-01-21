using System;
using System.Collections.Generic;
using System.Text;

namespace GoDogNUCServer.Models
{
    public class NUCDevice
    {
        public NUCDevice()
        {
            Cameras = new List<Camera>();
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string ServerURL { get; set; }

        public List<Camera> Cameras { get; set; }
    }
}
