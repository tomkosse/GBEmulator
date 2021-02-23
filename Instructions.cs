using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace GBEmulator
{
  public class Instructions
  {
    private IDictionary<OpCode, Action<Processor, Memory, byte, byte>> _instructionSet = new Dictionary<OpCode, Action<Processor, Memory, byte, byte>>();

    public Instructions()
    {
      var methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (var opCode in Enum.GetValues<OpCode>())
      {
        var action = methods
            .FirstOrDefault(
              m => Enum.TryParse(m.Name, true, out OpCode parsedOpCode) && parsedOpCode == opCode
              )?.CreateDelegate<Action<Processor, Memory, byte, byte>>(this);

        if (action == null)
        {
          System.Console.WriteLine("Missing implementation of: " + opCode);
        }
        else
        {
          _instructionSet.Add(opCode, action);
        }
      }
    }

    public Action<Processor, Memory, byte, byte> GetOperation(OpCode opCode)
    {
      if (_instructionSet.ContainsKey(opCode))
      {
        return _instructionSet[opCode];
      }
      else
      {
        throw new NotImplementedException($"OpCode not implemented! " + ((byte)opCode).ToString("X2"));
      }
    }

    private void ANDB(Processor processor, Memory memory, byte parameter1, byte parameter2) 
    {
      processor.Register.A = (byte)(processor.Register.A & processor.Register.B);

      processor.Register.IncrementProgramCounter();

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = true;
      processor.Register.CarryFlag = false;
    }

    private void ANDC(Processor processor, Memory memory, byte parameter1, byte parameter2) 
    {
      processor.Register.A = (byte)(processor.Register.A & processor.Register.C);

      processor.Register.IncrementProgramCounter();

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = true;
      processor.Register.CarryFlag = false;
    }

    private void ANDD(Processor processor, Memory memory, byte parameter1, byte parameter2) 
    {
      processor.Register.A = (byte)(processor.Register.A & processor.Register.D);

      processor.Register.IncrementProgramCounter();

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = true;
      processor.Register.CarryFlag = false;
    }

    private void ANDE(Processor processor, Memory memory, byte parameter1, byte parameter2) 
    {
      processor.Register.A = (byte)(processor.Register.A & processor.Register.E);

      processor.Register.IncrementProgramCounter();

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = true;
      processor.Register.CarryFlag = false;
    }

    private void PUSHBC(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      PushOntoStack(processor, memory, processor.Register.BC);

      processor.Register.IncrementProgramCounter();
      processor.Register.IncrementProgramCounter();
      processor.Register.IncrementProgramCounter();
    }

    private void PUSHAF(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      PushOntoStack(processor, memory, processor.Register.AF);

      processor.Register.IncrementProgramCounter();
      processor.Register.IncrementProgramCounter();
      processor.Register.IncrementProgramCounter();
    }

    private void NOP(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.IncrementProgramCounter();
    }

    private void STOP(Processor processor, Memory memory, byte parameter1, byte parameter2) {
      processor.Stopped = true;
      processor.Register.IncrementProgramCounter();
    }

    private void JP_nn(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.ProgramCounter = new TwoByteShort(parameter2, parameter1).Short;
    }

    private void JP_NZ_nn(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      if(!processor.Register.ZeroFlag) {
        processor.Register.ProgramCounter = new TwoByteShort(parameter2, parameter1).Short;
      }
    }

    private void LDH_A_n(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.A = (byte)(0xFF00 + parameter1);
      processor.Register.IncrementProgramCounter();
      processor.Register.IncrementProgramCounter();
    }

    private void LDH_n_A(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      memory.WriteByte(0xFF00 + parameter1, processor.Register.A);
      processor.Register.IncrementProgramCounter();
      processor.Register.IncrementProgramCounter();
    }

    private void CP_n(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      var registerA = processor.Register.A;
      var result = registerA - parameter1;

      processor.Register.ZeroFlag = (result == 0);
      processor.Register.SubstractFlag = true;
      processor.Register.HalfCarryFlag = (registerA & 0xF) - (parameter1 & 0xF) < 0;
      processor.Register.CarryFlag = registerA < parameter1;

      processor.Register.IncrementProgramCounter();
      processor.Register.IncrementProgramCounter();
    }

    private void RET_z(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      if (processor.Register.ZeroFlag)
      {
        RET(processor, memory, parameter1, parameter2);
      }
      else
      {
        processor.Register.IncrementProgramCounter();
      }
    }

    private void RET_NZ(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      if (!processor.Register.ZeroFlag)
      {
        RET(processor, memory, parameter1, parameter2);
      }
      else
      {
        processor.Register.IncrementProgramCounter();
      }
    }

    private void RET(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      var firstByte = PopFromStack(processor, memory);
      var secondByte = PopFromStack(processor, memory);
      TwoByteShort tbs = new TwoByteShort(firstByte, secondByte);
      processor.Register.ProgramCounter = tbs.Short;
    }

    private void RST_38H(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      Reset(processor, memory, 0x38);
    }

    private void RST_18H(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      Reset(processor, memory, 0x18);
    }

    private void Reset(Processor processor, Memory memory, int offset)
    {

      PushOntoStack(processor, memory, processor.Register.ProgramCounter);
      processor.Register.ProgramCounter = (short)(0 + offset);
    }

    private void EI(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      // TODO: Enable interrupts (what does this do?)

      processor.Register.IncrementProgramCounter();
    }

    private void CALL(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      PushOntoStack(processor, memory, (short)(processor.Register.ProgramCounter + 2));

      TwoByteShort value = new TwoByteShort(parameter2, parameter1);
      processor.Register.ProgramCounter = value.Short;
    }

    private void CALLCNN(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      if (processor.Register.CarryFlag)
      {
        PushOntoStack(processor, memory, (short)(processor.Register.ProgramCounter + 2));

        TwoByteShort value = new TwoByteShort(parameter2, parameter1);
        processor.Register.ProgramCounter = value.Short;
      }
      else
      {
        processor.Register.IncrementProgramCounter();
      }
    }

    private void SBC_A_B(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      var registerA = processor.Register.A;
      var result = processor.Register.A - (processor.Register.B + (processor.Register.CarryFlag ? 1 : 0));

      processor.Register.ZeroFlag = (result == 0);
      processor.Register.SubstractFlag = true;
      processor.Register.HalfCarryFlag = (registerA & 0xF) - (parameter1 & 0xF) < 0;
      processor.Register.CarryFlag = registerA < parameter1;
    }

    
    private void LDBA(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.B = processor.Register.A;
      processor.Register.IncrementProgramCounter();
    }

    private void LDBB(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      NOP(processor, memory, parameter1, parameter2);
    }

    private void LD_BC_NN(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.BC = new TwoByteShort(parameter1, parameter2).Short;

      processor.Register.IncrementProgramCounter();
    }

    private void DECB(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.B = (byte)(processor.Register.B - 1);
      processor.Register.IncrementProgramCounter();

      processor.Register.ZeroFlag = processor.Register.B == 0;
      processor.Register.SubstractFlag = true;
      processor.Register.HalfCarryFlag = (processor.Register.B & 0xF) < 0;
    }

    private void LDDHLA(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.HL = processor.Register.A;
      processor.Register.HL = (short)(processor.Register.HL - 1);
      processor.Register.IncrementProgramCounter();

      processor.Register.ZeroFlag = processor.Register.HL == 0;
      processor.Register.SubstractFlag = true;
      processor.Register.HalfCarryFlag = (processor.Register.HL & 0xF) < 0;
    }

    private void JR(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.ProgramCounter += parameter1;
    }

    private void JRNZ(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      if (processor.Register.ZeroFlag == false)
      {
        processor.Register.ProgramCounter += parameter1;
      }
      else
      {
        processor.Register.IncrementProgramCounter();
        processor.Register.IncrementProgramCounter();
      }
    }

    private void JRZ(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      if (processor.Register.ZeroFlag == true)
      {
        processor.Register.ProgramCounter += parameter1;
      }
      else
      {
        processor.Register.IncrementProgramCounter();
        processor.Register.IncrementProgramCounter();
      }
    }

    private void JRNC(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      if (processor.Register.CarryFlag == false)
      {
        processor.Register.ProgramCounter += parameter1;
      }
      else
      {
        processor.Register.IncrementProgramCounter();
        processor.Register.IncrementProgramCounter();
      }
    }

    private void JRC(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      if (processor.Register.CarryFlag == true)
      {
        processor.Register.ProgramCounter += parameter1;
      }
      else
      {
        processor.Register.IncrementProgramCounter();
        processor.Register.IncrementProgramCounter();
      }
    }

    private void RRA(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      BitArray bitArray = new BitArray(new byte[] { processor.Register.A });
      var toBeCarryBit = bitArray[bitArray.Length - 1];
      BitArray rotatedBitArray = bitArray.RightShift(1);
      rotatedBitArray[0] = processor.Register.CarryFlag;
      processor.Register.CarryFlag = toBeCarryBit;
      processor.Register.A = Utilities.BitArrayToByte(rotatedBitArray);

      processor.Register.IncrementProgramCounter();
    }

    private void LD_DE_NN(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.DE = new TwoByteShort(parameter1, parameter2).Short;
      processor.Register.IncrementProgramCounter();
    }

    private void LDDEA(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.DE = processor.Register.A;
      processor.Register.IncrementProgramCounter();
    }

    private void INCA(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.A++;
      processor.Register.IncrementProgramCounter();
      processor.Register.ZeroFlag = processor.Register.A == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.A & 0xF) < 0;
    }

    private void INCB(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.B++;
      processor.Register.IncrementProgramCounter();
      processor.Register.ZeroFlag = processor.Register.B == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.B & 0xF) < 0;
    }
    private void INCC(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.C++;
      processor.Register.IncrementProgramCounter();
      processor.Register.ZeroFlag = processor.Register.C == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.C & 0xF) < 0;
    }
    private void INCD(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.D++;
      processor.Register.IncrementProgramCounter();
      processor.Register.ZeroFlag = processor.Register.D == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.D & 0xF) < 0;
    }

    private void INCDE(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.DE++;
      processor.Register.IncrementProgramCounter();
      processor.Register.ZeroFlag = processor.Register.DE == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.DE & 0xF) < 0;
    }

    private void INCE(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.E++;
      processor.Register.IncrementProgramCounter();
      processor.Register.ZeroFlag = processor.Register.E == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.E & 0xF) < 0;
    }
    private void INCH(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.H++;
      processor.Register.IncrementProgramCounter();
      processor.Register.ZeroFlag = processor.Register.H == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.H & 0xF) < 0;
    }
    private void INCL(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.L++;
      processor.Register.IncrementProgramCounter();
      processor.Register.ZeroFlag = processor.Register.L == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.L & 0xF) < 0;
    }

    private void INCHL(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.HL++;
      processor.Register.IncrementProgramCounter();
      processor.Register.ZeroFlag = processor.Register.HL == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.HL & 0xF) < 0;
    }

    private void XORA(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      var result = processor.Register.A ^ processor.Register.A;
      processor.Register.A = (byte)result;

      processor.Register.IncrementProgramCounter();

      processor.Register.ZeroFlag = result == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = false;
      processor.Register.CarryFlag = false;
    }

    private void LD_HL_NN(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.HL = new TwoByteShort(parameter1, parameter2).Short;

      processor.Register.IncrementProgramCounter();
      processor.Register.IncrementProgramCounter();
    }

    private void LDADE(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.A = new TwoByteShort(processor.Register.DE).LowerByte;

      processor.Register.IncrementProgramCounter();
    }

    private void LDIHL(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.HL = new TwoByteShort(0, processor.Register.A).Short;
      processor.Register.HL++;

      processor.Register.IncrementProgramCounter();
    }
    
    private void LDIAHL(Processor processor, Memory memory, byte parameter1, byte parameter2) 
    {
      processor.Register.A = memory.ReadByte(processor.Register.HL);
      processor.Register.HL++;

      processor.Register.IncrementProgramCounter();
    }

    private void ADCAB(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.A += (byte)(processor.Register.B + (processor.Register.CarryFlag ? 1 : 0));

      processor.Register.IncrementProgramCounter();
    }

    private void ADCAC(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.A += (byte)(processor.Register.C + (processor.Register.CarryFlag ? 1 : 0));

      processor.Register.IncrementProgramCounter();
    }

    private void LDCN(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      processor.Register.C = parameter1;

      processor.Register.IncrementProgramCounter();
      processor.Register.IncrementProgramCounter();
    }

    #region Helpers

    private byte PopFromStack(Processor processor, Memory memory)
    {
      processor.Register.IncreaseStackPointer();
      byte value = memory.ReadByte(processor.Register.StackPointer);

      System.Console.WriteLine($"Read from stack: {processor.Register.StackPointer.ToString("X8")} - {value.ToString("X8")}");
      return value;
    }

    private void PushOntoStack(Processor processor, Memory memory, byte value)
    {
      processor.Register.DecreaseStackPointer();
      memory.WriteByte(processor.Register.StackPointer, value);

      System.Console.WriteLine($"Written to stack: {processor.Register.StackPointer.ToString("X8")} - {value.ToString("X8")}");
    }

    private void PushOntoStack(Processor processor, Memory memory, short value)
    {
      processor.Register.DecreaseStackPointer();
      memory.WriteShort(processor.Register.StackPointer, value);
      processor.Register.DecreaseStackPointer();

      System.Console.WriteLine($"Written to stack: {processor.Register.StackPointer.ToString("X8")} - {value.ToString("X16")}");
    }


    #endregion
  }
}