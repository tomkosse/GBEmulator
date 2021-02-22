using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace GBEmulator
{
  public class Memory
  {
    private const int ROM_BANK = 0x0000;
    private const int VIDEO_RAM = 0x8000;
    
    public bool InBios { get; private set; }

    System.Memory<byte> Heap = new System.Memory<byte>(new byte[0xFFFF]);

    internal void LoadRom(byte[] romFile)
    {
      if(romFile.Length > VIDEO_RAM) { throw new InvalidOperationException("ROM is larger than 32k! "); }
      WriteBytes(ROM_BANK, romFile);
    }

    
    public byte ReadByte(int address)
    {
      using(var handle = Heap.Pin())
      {
        unsafe
        {
          var addr = (byte*)handle.Pointer + address;
          var value = *addr;
          return Convert.ToByte(value);
        }
      }
    }

    public char ReadWord(int address)
    {
      using(var handle = Heap.Pin())
      {
        unsafe
        {
          var addr = (char*)handle.Pointer + address;
          var value = *addr;
          return Convert.ToChar(value);
        }
      }
    }

    public void WriteByte(int address, byte value)
    {
      using(var handle = Heap.Pin())
      {
        unsafe
        {
          var addr = (byte*)handle.Pointer + address;
          *addr = value;
        }
      }
    }

    public void WriteBytes(int address, byte[] value)
    {
      using(var handle = Heap.Pin())
      {
        unsafe
        {
          var addr = (byte*)handle.Pointer + address;
          foreach(byte b in value){
            *addr = b;
            addr++;
          }
        }
      }
    }

    public void WriteWord(int address, char value)
    {
      using(var handle = Heap.Pin())
      {
        unsafe
        {
          var addr = (char*)handle.Pointer + address;
          *addr = value;
        }
      }
    }
  }
}