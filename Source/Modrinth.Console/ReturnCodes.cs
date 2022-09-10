using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Modrinth.Console
{
    public static class ReturnCodes
    {
        public const int GenericError = 0x01;
        public const int ArgumentError = 0x02;
        public const int NetworkError = 0x03;
    }
}