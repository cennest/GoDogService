using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskConsoleApp
{
    class ActionModel
    {

        public ActionModel(int streamNumber,CancellationToken token)
        {
            this.streamNumber = streamNumber;
            this.token = token;
        }

        public int streamNumber { get; set; }
        public CancellationToken token { get; set; }
    }
}
