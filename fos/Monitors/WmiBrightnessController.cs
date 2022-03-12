using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace fos
{
    class WmiBrightnessController
    {
        //private uint minBrightness = 0;
        private uint maxBrightness;

        private ManagementScope scope = new ManagementScope("root\\WMI");
        private ObjectQuery query = new ObjectQuery("SELECT * FROM WmiMonitorBrightness");
        private SelectQuery queryMethods = new SelectQuery("WmiMonitorBrightnessMethods");

        public WmiBrightnessController()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                maxBrightness = (uint)m["Levels"] - 1;
            }
        }

        public uint GetBrightness()
        {
            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            using ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                return (uint)(Convert.ToSingle(m["CurrentBrightness"]) / maxBrightness * 100);
            }

            return 0;
        }

        public void SetBrightness(uint brightness)
        {
            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, queryMethods);
            using ManagementObjectCollection objectCollection = searcher.Get();

            foreach (ManagementObject m in objectCollection)
            {
                m.InvokeMethod("WmiSetBrightness",
                    new object[] { uint.MaxValue, (uint)(brightness / 100.0f * maxBrightness) });
                break;
            }
        }
    }
}
