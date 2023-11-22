using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerSell
{
    public static class ConnectionHelper
    {
        public static string ConnectionString
        {
            get
            {
                string db = Path.Combine(Path.GetFullPath(@"..\..\"), "Computerdb.mdf");
                return  $@"Data Source=(localdb)\mssqllocaldb;AttachDbFilename={db};Initial Catalog=Computerdb;Trusted_Connection=True";
            }
        }
    }
}
