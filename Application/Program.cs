using ServerLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerAsync server = new ServerAsync(IPAddress.Parse("127.0.0.1"), 3000);
            server.Start();
        }
    }
}
