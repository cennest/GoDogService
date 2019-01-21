using System;
using System.IO;

namespace GoDogCommon
{
    public class Configuration
    {
        private Logger logger;
        private string ConfigFile = AppDomain.CurrentDomain.BaseDirectory + "\\configuration.json";

        public Configuration()
        {
            this.logger = Logger.GetLogger();
        }

        private static string Identifier(string wmiClass, string wmiProperty)
        {
            string result = string.Empty;

            System.Management.ManagementClass mclass = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mclass.GetInstances();

            foreach (System.Management.ManagementObject mo in moc)
            {
                if (string.IsNullOrEmpty(result))
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                    }
                    catch { }
                }
            }

            return result;
        }

        private static string Identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = string.Empty;

            System.Management.ManagementClass mclass = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mclass.GetInstances();

            foreach (System.Management.ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                        }
                        catch { }
                    }
                }
            }

            return result;
        }

        public static string BiosID()
        {
            return Identifier("Win32_BIOS", "SerialNumber");
        }

        public static string MacID()
        {
            return Identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
        }
    }
}
