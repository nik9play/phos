﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace fos
{
    public interface IMonitor
    {
        public string Name { get; set; }
        public string DeviceName { get; }
        public Size Resolution { get; set; }
        public Point Position { get; set; }
        public uint Brightness { get; set; }

        public void SetBrightness(uint brightness);
    }
}
