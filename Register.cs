using System;
using System.Collections;

namespace GBEmulator
{
  public class Register
  {
    public Clock LastInstructionClock { get; private set; }

    // 8-bit registers, represented as a byte
    public byte A;
    public byte B;
    public byte C;
    public byte D;
    public byte E;
    public byte H;
    public byte L;

    // 16-bit registers, represented as Short
    public short AF
    {
      get
      {
        var lowerByte = Utilities.BitArrayToByte(new BitArray(new bool[] { ZeroFlag, SubstractFlag, HalfCarryFlag, CarryFlag, false, false, false, false }));
        return new TwoByteShort(A, lowerByte).Short;
      }
      set
      {
        var tbs = new TwoByteShort(value);
        A = tbs.HigherByte;
        var bitArray = new BitArray(new byte[] { tbs.LowerByte });
        ZeroFlag = bitArray[7];
        SubstractFlag = bitArray[6];
        HalfCarryFlag = bitArray[5];
        CarryFlag = bitArray[4];
      }
    }

    public short BC
    {
      get
      {
        return new TwoByteShort(B, C).Short;
      }
      set
      {
        var tbs = new TwoByteShort(value);
        B = tbs.HigherByte;
        C = tbs.LowerByte;
      }
    }

    public short DE
    {
      get
      {
        return new TwoByteShort(D, E).Short;
      }
      set
      {
        var tbs = new TwoByteShort(value);
        D = tbs.HigherByte;
        E = tbs.LowerByte;
      }
    }

    public short HL
    {
      get
      {
        return new TwoByteShort(H, L).Short;
      }
      set
      {
        var tbs = new TwoByteShort(value);
        H = tbs.HigherByte;
        L = tbs.LowerByte;
      }
    }

    private byte IME = 1;

    // 16-bit (execution) registers, represented as a int
    public short ProgramCounter;
    public int StackPointer { get; private set; }

    public short I { get; private set; } //Interrupt
    public short R { get; set; }

    /*
     On the gameboy this is represented as a byte like this:
     Z N H C 0 0 0 0
     Zero Flag (Z)
     Substract Flag (N)
     Half Carry Flag (H)
     Carry Flag (C)
     But have booleans :D
    */
    public bool ZeroFlag { get; set; }
    public bool SubstractFlag { get; set; }
    public bool HalfCarryFlag { get; set; }
    public bool CarryFlag { get; set; }

    public void Print()
    {
      System.Console.WriteLine($"A: {A.ToString("X2")}, B: {B.ToString("X2")}, C: {C.ToString("X2")}, D: {D.ToString("X2")}, E: {E.ToString("X2")}, H: {H.ToString("X2")}, L: {L.ToString("X2")}");

      System.Console.WriteLine("Z: " + ZeroFlag + " N: " + SubstractFlag + " H: " + HalfCarryFlag + " C: " + CarryFlag);
    }

    public Register(Clock lastInstruction) {
      ProgramCounter = 0x100; // Program counter starts at 0x100
      StackPointer = 0xFFFE; // Stackpointer starts at 0xFFFE

      AF = 0x01B0; // GB/SGB identifier
      BC = 0x0013;
      DE = 0x00D8;
      HL = 0x014D;

      LastInstructionClock = new Clock();
    }

    public void DecreaseStackPointer()
    {
      StackPointer--;
    }

    public void IncreaseStackPointer()
    {
      StackPointer++;
    }

    public void ReportClockElapsed(int MTimes)
    {
      LastInstructionClock.SetValue(MTimes);
    }

    internal void IncrementProgramCounter()
    {
      ProgramCounter++;
    }
  }
}