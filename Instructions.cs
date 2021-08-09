using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Serilog;

namespace GBEmulator
{
  public class Instructions
  {
    private IDictionary<OpCode, Action<Processor, Memory>> _instructionSet = new Dictionary<OpCode, Action<Processor, Memory>>();

    public Instructions()
    {
      var methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (var opCode in Enum.GetValues<OpCode>())
      {
        var action = methods
            .FirstOrDefault(
              m => Enum.TryParse(m.Name, true, out OpCode parsedOpCode) && parsedOpCode == opCode
              )?.CreateDelegate<Action<Processor, Memory>>(this);

        if (action == null)
        {
          Log.Logger.Verbose("Missing implementation of: " + opCode);
        }
        else
        {
          _instructionSet.Add(opCode, action);
        }
      }
    }

    public Action<Processor, Memory> GetOperation(OpCode opCode)
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

    private void ADDAE(Processor processor, Memory memory)
    {
      processor.Register.A = (byte)(processor.Register.A + processor.Register.E);

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = ((((processor.Register.A & 0xF) + (processor.Register.E & 0xF)) & 0x10) == 0x10);
      processor.Register.CarryFlag = (processor.Register.A & 0xFFFF) < 0;
    }

    private void ANDA(Processor processor, Memory memory)
    {
      processor.Register.A = (byte)(processor.Register.A & processor.Register.A);

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = true;
      processor.Register.CarryFlag = false;
    }
    private void ANDB(Processor processor, Memory memory)
    {
      processor.Register.A = (byte)(processor.Register.A & processor.Register.B);

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = true;
      processor.Register.CarryFlag = false;
    }

    private void ANDC(Processor processor, Memory memory)
    {
      processor.Register.A = (byte)(processor.Register.A & processor.Register.C);

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = true;
      processor.Register.CarryFlag = false;
    }

    private void ANDD(Processor processor, Memory memory)
    {
      processor.Register.A = (byte)(processor.Register.A & processor.Register.D);

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = true;
      processor.Register.CarryFlag = false;
    }

    private void ANDE(Processor processor, Memory memory)
    {
      processor.Register.A = (byte)(processor.Register.A & processor.Register.E);

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = true;
      processor.Register.CarryFlag = false;
    }

    private void ANDN(Processor processor, Memory memory)
    {
      processor.Register.A = (byte)(processor.Register.A & processor.GetNextByte());

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = true;
      processor.Register.CarryFlag = false;
    }

    private void PUSHBC(Processor processor, Memory memory)
    {
      PushOntoStack(processor, memory, processor.Register.BC);
    }

    private void PUSHAF(Processor processor, Memory memory)
    {
      PushOntoStack(processor, memory, processor.Register.AF);
    }

    private void NOP(Processor processor, Memory memory)
    {

    }

    private void STOP(Processor processor, Memory memory)
    {
      processor.Stopped = true;
    }

    private void JP_nn(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var parameter2 = processor.GetNextByte();
      processor.Register.ProgramCounter = new TwoByteShort(parameter2, parameter1).Short;
    }

    private void JP_NZ_nn(Processor processor, Memory memory)
    {
      if (!processor.Register.ZeroFlag)
      {
        var parameter1 = processor.GetNextByte();
        var parameter2 = processor.GetNextByte();
        processor.Register.ProgramCounter = new TwoByteShort(parameter2, parameter1).Short;
      }
    }

    private void LDH_A_n(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      processor.Register.A = (byte)(0xFF00 + parameter1);
    }

    private void LDH_n_A(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      memory.WriteByte(0xFF00 + parameter1, processor.Register.A);
    }

    private void CP_n(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var registerA = processor.Register.A;
      var result = registerA - parameter1;

      processor.Register.ZeroFlag = (result == 0);
      processor.Register.SubstractFlag = true;
      processor.Register.HalfCarryFlag = (registerA & 0xF) - (parameter1 & 0xF) < 0;
      processor.Register.CarryFlag = registerA < parameter1;
    }

    private void RET_z(Processor processor, Memory memory)
    {
      if (processor.Register.ZeroFlag)
      {
        RET(processor, memory);
      }
      else
      {

      }
    }

    private void RET_NZ(Processor processor, Memory memory)
    {
      if (!processor.Register.ZeroFlag)
      {
        RET(processor, memory);
      }
      else
      {

      }
    }

    private void RET(Processor processor, Memory memory)
    {
      var firstByte = PopFromStack(processor, memory);
      var secondByte = PopFromStack(processor, memory);
      TwoByteShort tbs = new TwoByteShort(firstByte, secondByte);
      processor.Register.ProgramCounter = tbs.Short;
    }

    private void RST_38H(Processor processor, Memory memory)
    {
      Reset(processor, memory, 0x38);
    }

    private void RST_18H(Processor processor, Memory memory)
    {
      Reset(processor, memory, 0x18);
    }

    private void RST_08H(Processor processor, Memory memory)
    {
      Reset(processor, memory, 0x08);
    }

    private void Reset(Processor processor, Memory memory, int offset)
    {

      PushOntoStack(processor, memory, processor.Register.ProgramCounter);
      processor.Register.ProgramCounter = (ushort)(0 + offset);
    }

    private void EI(Processor processor, Memory memory)
    {
      processor.Register.IME = 1;
    }

    private void DI(Processor processor, Memory memory)
    {
      processor.Register.IME = 0;
    }

    private void CALL(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var parameter2 = processor.GetNextByte();
      CALL(processor, memory, parameter1, parameter2);
    }
    private void CALL(Processor processor, Memory memory, byte parameter1, byte parameter2)
    {
      PushOntoStack(processor, memory, (ushort)(processor.Register.ProgramCounter + 2));

      TwoByteShort value = new TwoByteShort(parameter2, parameter1);
      processor.Register.ProgramCounter = value.Short;
    }

    private void CALLCNN(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var parameter2 = processor.GetNextByte();
      if (processor.Register.CarryFlag)
      {
        CALL(processor, memory, parameter1, parameter2);
      }
    }

    private void CALLNCNN(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var parameter2 = processor.GetNextByte();
      if (!processor.Register.CarryFlag)
      {
        CALL(processor, memory, parameter1, parameter2);
      }
    }


    private void CALLNZNN(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var parameter2 = processor.GetNextByte();
      if (!processor.Register.ZeroFlag)
      {
        CALL(processor, memory, parameter1, parameter2);
      }
    }
    private void CALLZNN(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var parameter2 = processor.GetNextByte();
      if (processor.Register.ZeroFlag)
      {
        CALL(processor, memory, parameter1, parameter2);
      }
    }

    private void SBC_A_B(Processor processor, Memory memory)
    {
      var registerA = processor.Register.A;
      var result = processor.Register.A - (processor.Register.B + (processor.Register.CarryFlag ? 1 : 0));

      var parameter1 = processor.GetNextByte();
      processor.Register.ZeroFlag = (result == 0);
      processor.Register.SubstractFlag = true;
      processor.Register.HalfCarryFlag = (registerA & 0xF) - (parameter1 & 0xF) < 0;
      processor.Register.CarryFlag = registerA < parameter1;
    }

    private void LDLHL(Processor processor, Memory memory)
    {
      processor.Register.L = memory.ReadByte(processor.Register.HL);
    }

    private void LDAHL(Processor processor, Memory memory)
    {
      processor.Register.A = memory.ReadByte(processor.Register.HL);
    }

    private void LDAN(Processor processor, Memory memory)
    {
      processor.Register.A = processor.GetNextByte();
    }

    private void LDBA(Processor processor, Memory memory)
    {
      processor.Register.B = processor.Register.A;
    }

    private void LDBB(Processor processor, Memory memory)
    {
      NOP(processor, memory);
    }

    private void LD_SP_NN(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var parameter2 = processor.GetNextByte();
      processor.Register.StackPointer = new TwoByteShort(parameter1, parameter2).Short;
    }

    private void LD_BC_NN(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var parameter2 = processor.GetNextByte();
      processor.Register.BC = new TwoByteShort(parameter1, parameter2).Short;
    }

    private void LD_NN_A(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var parameter2 = processor.GetNextByte();
      memory.WriteByte(new TwoByteShort(parameter2, parameter1).Short, processor.Register.A);
    }

    private void LD_NN_SP(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var parameter2 = processor.GetNextByte();
      memory.WriteShort(new TwoByteShort(parameter2, parameter1).Short, processor.Register.StackPointer);
    }

    private void DECC(Processor processor, Memory memory)
    {
      processor.Register.C--;

      processor.Register.ZeroFlag = processor.Register.C == 0;
      processor.Register.SubstractFlag = true;
      processor.Register.HalfCarryFlag = (processor.Register.B & 0xF) < 0;
    }

    private void DECB(Processor processor, Memory memory)
    {
      processor.Register.B--;

      processor.Register.ZeroFlag = processor.Register.B == 0;
      processor.Register.SubstractFlag = true;
      processor.Register.HalfCarryFlag = (processor.Register.B & 0xF) < 0;
    }

    private void DECBC(Processor processor, Memory memory)
    {
      processor.Register.BC--;

      processor.Register.ZeroFlag = processor.Register.BC == 0;
      processor.Register.SubstractFlag = true;
      processor.Register.HalfCarryFlag = (processor.Register.BC & 0xF) < 0;
    }

    private void DECHL(Processor processor, Memory memory)
    {
      processor.Register.HL--;

      processor.Register.ZeroFlag = processor.Register.HL == 0;
      processor.Register.SubstractFlag = true;
      processor.Register.HalfCarryFlag = (processor.Register.HL & 0xF) < 0;
    }

    private void DECHL_Parentheses(Processor processor, Memory memory)
    {
      memory.WriteByte(processor.Register.HL, (byte)(memory.ReadByte(processor.Register.HL) - 1));

      processor.Register.ZeroFlag = processor.Register.HL == 0;
      processor.Register.SubstractFlag = true;
      processor.Register.HalfCarryFlag = (processor.Register.HL & 0xF) < 0;
    }

    private void LDHLE(Processor processor, Memory memory)
    {
      memory.WriteByte(processor.Register.HL, processor.GetNextByte());
    }

    private void BIT_7_H(Processor processor, Memory memory)
    {
      int bitvalue = (processor.Register.H >> 7) & 1;

      processor.Register.ZeroFlag = bitvalue == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = true;
    }

    private void LDDHLA(Processor processor, Memory memory)
    {
      memory.WriteByte(processor.Register.HL, processor.Register.A);
      processor.Register.HL = (ushort)(processor.Register.HL - 1);

      processor.Register.IncrementProgramCounter();

      processor.Register.ZeroFlag = processor.Register.HL == 0;
      processor.Register.SubstractFlag = true;
      processor.Register.HalfCarryFlag = (processor.Register.HL & 0xF) < 0;
    }

    private void JR(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      processor.Register.ProgramCounter += parameter1;
    }

    private void JRNZ(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextSByte(); // Interpreted as signed byte; first bit indicates positive or negative
      if (processor.Register.ZeroFlag == false)
      {
        processor.Register.ProgramCounter = (ushort)(processor.Register.ProgramCounter + parameter1);
      }
    }

    private void JRZ(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      if (processor.Register.ZeroFlag == true)
      {
        processor.Register.ProgramCounter += parameter1;
      }
    }

    private void JRNC(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      if (processor.Register.CarryFlag == false)
      {
        processor.Register.ProgramCounter += parameter1;
      }
    }

    private void JRC(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      if (processor.Register.CarryFlag == true)
      {
        processor.Register.ProgramCounter += parameter1;
      }
    }

    private void RRA(Processor processor, Memory memory)
    {
      BitArray bitArray = new BitArray(new byte[] { processor.Register.A });
      var toBeCarryBit = bitArray[bitArray.Length - 1];
      BitArray rotatedBitArray = bitArray.RightShift(1);
      rotatedBitArray[0] = processor.Register.CarryFlag;
      processor.Register.CarryFlag = toBeCarryBit;
      processor.Register.A = Utilities.BitArrayToByte(rotatedBitArray);
    }

    private void LD_DE_NN(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var parameter2 = processor.GetNextByte();
      processor.Register.DE = new TwoByteShort(parameter1, parameter2).Short;
    }

    private void LDDEA(Processor processor, Memory memory)
    {
      processor.Register.DE = processor.Register.A;
    }

    private void INCA(Processor processor, Memory memory)
    {
      processor.Register.A++;
      processor.Register.ZeroFlag = processor.Register.A == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.A & 0xF) < 0;
    }

    private void INCB(Processor processor, Memory memory)
    {
      processor.Register.B++;

      processor.Register.ZeroFlag = processor.Register.B == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.B & 0xF) < 0;
    }
    private void INCC(Processor processor, Memory memory)
    {
      processor.Register.C++;

      processor.Register.ZeroFlag = processor.Register.C == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.C & 0xF) < 0;
    }
    private void INCD(Processor processor, Memory memory)
    {
      processor.Register.D++;

      processor.Register.ZeroFlag = processor.Register.D == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.D & 0xF) < 0;
    }

    private void INCDE(Processor processor, Memory memory)
    {
      processor.Register.DE++;

      processor.Register.ZeroFlag = processor.Register.DE == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.DE & 0xF) < 0;
    }

    private void INCE(Processor processor, Memory memory)
    {
      processor.Register.E++;

      processor.Register.ZeroFlag = processor.Register.E == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.E & 0xF) < 0;
    }
    private void INCH(Processor processor, Memory memory)
    {
      processor.Register.H++;

      processor.Register.ZeroFlag = processor.Register.H == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.H & 0xF) < 0;
    }
    private void INCL(Processor processor, Memory memory)
    {
      processor.Register.L++;

      processor.Register.ZeroFlag = processor.Register.L == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.L & 0xF) < 0;
    }

    private void INCHL(Processor processor, Memory memory)
    {
      processor.Register.HL++;

      processor.Register.ZeroFlag = processor.Register.HL == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.HL & 0xF) < 0;
    }

    private void INCBC(Processor processor, Memory memory)
    {
      processor.Register.BC++;

      processor.Register.ZeroFlag = processor.Register.BC == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.BC & 0xF) < 0;
    }

    private void XORA(Processor processor, Memory memory)
    {
      var result = processor.Register.A ^ processor.Register.A;
      processor.Register.A = (byte)result;



      processor.Register.ZeroFlag = result == 0;
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = false;
      processor.Register.CarryFlag = false;
    }

    private void LD_HL_NN(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      var parameter2 = processor.GetNextByte();
      processor.Register.HL = new TwoByteShort(parameter2, parameter1).Short;
    }

    private void LDADE(Processor processor, Memory memory)
    {
      processor.Register.A = new TwoByteShort(processor.Register.DE).LowerByte;
    }

    private void LDIHL(Processor processor, Memory memory)
    {
      processor.Register.HL = new TwoByteShort(0, processor.Register.A).Short;
      processor.Register.HL++;
    }

    private void LDIAHL(Processor processor, Memory memory)
    {
      processor.Register.A = memory.ReadByte(processor.Register.HL);
      processor.Register.HL++;
    }

    private void ADCAB(Processor processor, Memory memory)
    {
      processor.Register.A += (byte)(processor.Register.B + (processor.Register.CarryFlag ? 1 : 0));

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.A & 0x10) == 0x10;
      processor.Register.CarryFlag = (processor.Register.A & 0xFFFF) < 0;
    }

    private void ADCAC(Processor processor, Memory memory)
    {
      processor.Register.A += (byte)(processor.Register.C + (processor.Register.CarryFlag ? 1 : 0));

      processor.Register.ZeroFlag = (processor.Register.A == 0);
      processor.Register.SubstractFlag = false;
      processor.Register.HalfCarryFlag = (processor.Register.A & 0x10) == 0x10;
      processor.Register.CarryFlag = (processor.Register.A & 0xFFFF) < 0;
    }

    private void LDCN(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      processor.Register.C = parameter1;
    }

    private void LDBN(Processor processor, Memory memory)
    {
      var parameter1 = processor.GetNextByte();
      processor.Register.B = parameter1;
    }

    #region Helpers

    private byte PopFromStack(Processor processor, Memory memory)
    {
      processor.Register.IncreaseStackPointer();
      byte value = memory.ReadByte(processor.Register.StackPointer);

      Log.Logger.Verbose($"Read from stack: {processor.Register.StackPointer.ToString("X8")} - {value.ToString("X8")}");
      return value;
    }

    private void PushOntoStack(Processor processor, Memory memory, byte value)
    {
      processor.Register.DecreaseStackPointer();
      memory.WriteByte(processor.Register.StackPointer, value);

      Log.Logger.Verbose($"Written to stack: {processor.Register.StackPointer.ToString("X8")} - {value.ToString("X8")}");
    }

    private void PushOntoStack(Processor processor, Memory memory, ushort value)
    {
      processor.Register.DecreaseStackPointer();
      memory.WriteShort(processor.Register.StackPointer, value);
      processor.Register.DecreaseStackPointer();

      Log.Logger.Verbose($"Written to stack: {processor.Register.StackPointer.ToString("X8")} - {value.ToString("X16")}");
    }


    #endregion
  }
}