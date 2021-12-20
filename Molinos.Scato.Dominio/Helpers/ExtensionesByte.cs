using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Helpers
{
    public static class ExtensionesByte
    {
        public static bool BitAt(this byte aByte, int bitNumber)
        {
            //0 -> 7
            return (aByte & (1 << (7 - bitNumber))) != 0;
        }
    }
}
