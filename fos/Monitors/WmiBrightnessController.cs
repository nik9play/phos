using System;
using System.Management;

namespace fos
{
    internal class WmiBrightnessController
    {
        //private uint minBrightness = 0;
        private readonly uint _maxBrightness;
        private readonly ObjectQuery _query = new("SELECT * FROM WmiMonitorBrightness");
        private readonly SelectQuery _queryMethods = new("WmiMonitorBrightnessMethods");

        private readonly ManagementScope _scope = new("root\\WMI");

        public WmiBrightnessController()
        {
            var searcher = new ManagementObjectSearcher(_scope, _query);

            var queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection) _maxBrightness = (uint)m["Levels"] - 1;
        }

        public uint GetBrightness()
        {
            using var searcher = new ManagementObjectSearcher(_scope, _query);
            using var queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
                return (uint)(Convert.ToSingle(m["CurrentBrightness"]) / _maxBrightness * 100);

            return 0;
        }

        public void SetBrightness(uint brightness)
        {
            using var searcher = new ManagementObjectSearcher(_scope, _queryMethods);
            using var objectCollection = searcher.Get();

            foreach (ManagementObject m in objectCollection)
            {
                m.InvokeMethod("WmiSetBrightness",
                    new object[] { uint.MaxValue, (uint)(brightness / 100.0f * _maxBrightness) });
                break;
            }
        }
    }
}