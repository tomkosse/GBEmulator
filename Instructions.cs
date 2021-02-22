using System;
using System.Collections.Generic;

namespace GBEmulator
{
  public class Instructions
  {
    private IDictionary<OpCode, Action<Processor, Memory>> _instructionSet = new Dictionary<OpCode, Action<Processor, Memory>>();

    public Instructions()
    {
        _instructionSet.Add(OpCode.PUSHBC, PUSHBC);
        _instructionSet.Add(OpCode.NOP, NOP);
    }

    public Action<Processor, Memory> GetOperation(OpCode opCode)
    {
        return _instructionSet[opCode];
    }

    public void PUSHBC(Processor processor, Memory memory)
    {
        processor.Register.DecreaseStackPointer();
        memory.WriteByte(processor.Register.StackPointer, processor.Register.B);
        processor.Register.DecreaseStackPointer();
        memory.WriteByte(processor.Register.StackPointer, processor.Register.C);
    }

    public void NOP(Processor processor, Memory memory)
    {

    }
  }
}