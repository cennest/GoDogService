﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GoDogServer.Models
{
    public class Field
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public NUCDevice NUC { get; set; }
    }
}