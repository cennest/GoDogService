using GoDogServer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoDogServer.Helpers
{
    public sealed class DataHelper
    {
        private static DataHelper dataHelper;
        private static readonly object locker = new object();

        DataHelper()
        {

        }

        public static DataHelper GetDataHelper()
        {
            lock (locker)
            {
                if (dataHelper == null)
                {
                    dataHelper = new DataHelper();
                }
                return dataHelper;
            }
        }

        public Facility GetFacility()
        {
            return new Facility();
        }
    }
}
