

namespace GBEmulator
{

  // Simple representation of a Gameboy Z80 CPU
  public class Processor
  {
    public Clock Clock {get; private set;}
    public Register Register { get; private set; }

    private Memory _memory;

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
      Clock = clock ?? throw new System.ArgumentNullException(nameof(clock));
      Register = register ?? throw new System.ArgumentNullException(nameof(register));
      _memory = memory ?? throw new System.ArgumentNullException(nameof(memory));
    }

    public void Reset()
    {
      Clock = new Clock();
      Register = new Register(Clock);
      _stop = 0;
      _halt = 0;
    }

    public void ExecuteInstruction(OpCode opCode)
    {
      Register.R = (short)((Register.R + (short)1) & (short)127);
      var operation = instructions.GetOperation(opCode);
      operation.Invoke(this, _memory);
      Register.IncrementProgramCounter();
      Clock.Add(Register.LastInstructionClock);
    }
  }
}