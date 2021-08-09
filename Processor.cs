

using Serilog;

namespace GBEmulator
{

    // Simple representation of a Gameboy Z80 CPU
    public class Processor
    {
        public Clock Clock { get; private set; }
        public Register Register { get; private set; }
        public Memory Memory { get; }
        public bool Stopped { get; internal set; }

        public Instructions instructions;

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
            Memory = memory ?? throw new System.ArgumentNullException(nameof(memory));
            instructions = new Instructions();
        }

        private void ExecuteInstruction(OpCode opCode)
        {
            Register.R = (short)((Register.R + (short)1) & (short)127);
            var operation = instructions.GetOperation(opCode);
            operation.Invoke(this, Memory);
            Clock.Add(Register.LastInstructionClock);
        }

        public void ExecuteNextInstruction()
        {
            string pc = Register.ProgramCounter.ToString("X8");
            var opCode = GetInstruction();
            Log.Logger.Warning("Addr: " + pc + $" - Executing {opCode} ({((byte)opCode).ToString("X2")})");
            ExecuteInstruction(opCode);
            Register.Print();
        }

        private OpCode GetInstruction()
        {
            var OpCode = Memory.GetInstruction(Register.ProgramCounter);
            Register.IncrementProgramCounter();
            return OpCode;
        }

        public byte GetNextByte()
        {
            byte b = Memory.ReadByte(Register.ProgramCounter);
            Log.Logger.Information($"Read byte {((byte)b).ToString("X8")} from {Register.ProgramCounter.ToString("X8")}");
            Register.IncrementProgramCounter();
            return b;
        }

        public sbyte GetNextSByte()
        {
            sbyte b = Memory.ReadSByte(Register.ProgramCounter);
            Log.Logger.Information($"Read signed byte {((byte)b).ToString("X8")} from {Register.ProgramCounter.ToString("X8")}");
            Register.IncrementProgramCounter();
            return b;
        }

        internal void DoVBlank()
        {

        }
    }
}