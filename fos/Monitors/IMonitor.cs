﻿using System.Drawing;

namespace fos.Monitors;

public interface IMonitor
{
    public string Name { get; }
    public string DeviceName { get; }
    public string DeviceId { get; }
    public Size Resolution { get; }
    public Point Position { get; }
    public uint Brightness { get; set; }
    public void SetBrightnessSlow(uint brightness);
}