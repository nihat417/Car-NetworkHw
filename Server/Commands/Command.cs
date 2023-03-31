using Server.Enums;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Commands
{
    public class Command
    {
        public HttpMethods Method { get; set; }
        public Car? Car { get; set; }
    }
}
