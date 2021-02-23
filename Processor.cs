

namespace GBEmulator
{

  // Simple representation of a Gameboy Z80 CPU
  public class Processor
  {
    public Clock Clock {get; private set;}
    public Register Register { get; private set; }
    public Memory Memory { get; }
    public bool Stopped { get; internal set; }

    public Instructions instructions;

    private int _halt;
    private int _stop;

    public static Processor CreateNew(Memory memory)
    {
      var clock = new Clock();
      var register = new Register(clock);

      return new Processor(clock, register, memory);
    }

    private Processor(Clock clock, Register register, Memory memory)
    {
      _stop = 0;
      _halt = 0;

      Clock = clock ?? throw new System.ArgumentNullException(nameof(clock));
      Register = register ?? throw new System.ArgumentNullException(nameof(register));
      Memory = memory ?? throw new System.ArgumentNullException(nameof(memory));
      instructions = new Instructions();
    }

    private void ExecuteInstruction(OpCode opCode, byte parameter1, byte parameter2)
    {
      Register.R = (short)((Register.R + (short)1) & (short)127);
      var operation = instructions.GetOperation(opCode);
      operation.Invoke(this, Memory, parameter1, parameter2);
      Clock.Add(Register.LastInstructionClock);
    }

    public void ExecuteNextInstruction()
    {
      (var opCode, var firstParam, var secondParam) = Memory.GetInstruction(Register.ProgramCounter);
      System.Console.WriteLine("Addr: " + Register.ProgramCounter.ToString("X8") + $" - Executing {opCode} ({((byte)opCode).ToString("X2")}) with params {firstParam.ToString("X2")}, {secondParam.ToString("X2")}");
      ExecuteInstruction(opCode, firstParam, secondParam);
      Register.Print();
    }
  }
}