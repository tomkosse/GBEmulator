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

    internal void SetDefaults()
    {
       WriteByte(0xFF05, 0x00); // TIMA
       WriteByte(0xFF06, 0x00); // TMA
       WriteByte(0xFF07, 0x00); // TAC
       WriteByte(0xFF10, 0x80); // NR10
       WriteByte(0xFF11, 0xBF); // NR11
       WriteByte(0xFF12, 0xF3); // NR12
       WriteByte(0xFF14, 0xBF); // NR14
       WriteByte(0xFF16, 0x3F); // NR21
       WriteByte(0xFF17, 0x00); // NR22
       WriteByte(0xFF19, 0xBF); // NR24
       WriteByte(0xFF1A, 0x7F); // NR30
       WriteByte(0xFF1B, 0xFF); // NR31
       WriteByte(0xFF1C, 0x9F); // NR32
       WriteByte(0xFF1E, 0xBF); // NR33
       WriteByte(0xFF20, 0xFF); // NR41
       WriteByte(0xFF21, 0x00); // NR42
       WriteByte(0xFF22, 0x00); // NR43
       WriteByte(0xFF23, 0xBF); // NR30
       WriteByte(0xFF24, 0x77); // NR50
       WriteByte(0xFF25, 0xF3); // NR51
       WriteByte(0xFF26, 0xF1); // NR52
       WriteByte(0xFF40, 0x91); // LCDC
       WriteByte(0xFF42, 0x00); // SCY
       WriteByte(0xFF43, 0x00); // SCX
       WriteByte(0xFF45, 0x00); // LYC
       WriteByte(0xFF47, 0xFC); // BGP
       WriteByte(0xFF48, 0xFF); // OBP0
       WriteByte(0xFF49, 0xFF); // OBP1
       WriteByte(0xFF4A, 0x00); // WY
       WriteByte(0xFF4B, 0x00); // WX
       WriteByte(0xFFFF, 0x00); // IE
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
      if (address == 0xFF02 && value == 0x81) {
        Console.WriteLine("Written to datapin: " + ReadByte(0xFF01));
      }
      using(var handle = Heap.Pin())
      {
        unsafe
        {
          var addr = (byte*)handle.Pointer + address;
          *addr = value;
        }
      }
    }

    public void WriteShort(int address, short value)
    {
      if (address == 0xFF02 && value == 0x81) {
        Console.WriteLine("Written to datapin: " + ReadByte(0xFF01));
      }
      using(var handle = Heap.Pin())
      {
        unsafe
        {
          var addr = (short*)handle.Pointer + address;
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

    public (OpCode, byte param1, byte param2) GetInstruction(int programCounter)
    {
      var firstByte = ReadByte(programCounter);
      var secondByte = ReadByte(programCounter + 1);
      var thirdByte = ReadByte(programCounter + 2);
      return ((OpCode)firstByte, secondByte, thirdByte);
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