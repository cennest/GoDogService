using System;
using System.Collections.Generic;
using System.Text;

namespace GoDogServer.Models
{
    public class Camera
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public CameraType CameraType { get; set; }
        public string IPAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CameraURL { get; set; }
        public string StreamingURL { get; set; }
        public long ArchivalPeriod { get; set; }
    }
}
