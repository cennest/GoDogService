using System;
using System.ServiceProcess;

namespace GoDogService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new GoDogService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
