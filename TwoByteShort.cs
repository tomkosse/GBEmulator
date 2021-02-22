namespace GBEmulator
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    struct TwoByteShort {
        [FieldOffset(0)]
        public byte LowerByte;
        [FieldOffset(1)]
        public byte HigherByte;
        [FieldOffset(0)]
        public short Short;

    public TwoByteShort(short value) : this()
    {
        Short = value;
    }

    public TwoByteShort(byte a, byte f) : this()
    {
        HigherByte = a;
        LowerByte = f;
    }
  }

}