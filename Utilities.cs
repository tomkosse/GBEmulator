using System.Collections;

namespace GBEmulator
{
    public class Utilities
    {        
        public static byte BitArrayToByte(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret[0];
        }
    }
}