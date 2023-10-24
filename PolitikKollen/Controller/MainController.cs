using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Controls;

namespace Controller
{
    public class MainController
    {

        DAL.DbCommunication dal = new();

        public void Test()
        {
            dal.test();

        }
    }
}
