namespace GBEmulator
{
  public class Register
  {
    // 8-bit registers, represented as a byte
    private byte A;
    public byte B { get; private set; }
    public byte C { get; private set; }
    private byte D;
    private byte E;
    private byte H;
    private byte L;
    private byte F;

    // 16-bit registers, represented as Short
    private short AF
    {
      get
      {
        return new TwoByteShort(A, F).Short;
      }
      set
      {
        var tbs = new TwoByteShort(value);
        A = tbs.HigherByte;
        F = tbs.LowerByte;
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
    private short ProgramCounter;
    public short StackPointer { get; private set; }
    public short I { get; private set; }
    public short R { get; set; }

    public Clock LastInstructionClock { get; private set; }

    public Register(Clock lastInstruction) => LastInstructionClock = lastInstruction ?? throw new System.ArgumentNullException(nameof(lastInstruction));

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
      // ?
    }
  }
}