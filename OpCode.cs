public enum OpCode : byte
{
  ANDA = 0xA7,
  ANDB = 0xA0,
  ANDC = 0xA1,
  ANDD = 0xA2,
  ANDE = 0xA3,
  ANDH = 0xA4,
  ANDL = 0xA5,
  ANDHL = 0xA6,
  DECA = 0x3D,
  DECB = 0x05,
  DECC = 0x0D,
  DECD = 0x15,
  DECE = 0x1D,
  DECH = 0x25,
  DECL = 0x2D,
  DECHL = 0x35,
  PUSHAF = 0xF5,
  PUSHBC = 0xC5,
  NOP = 0x00,
  LDDHLA = 0x32,
  LDAA = 0x7F,
  LDAB = 0x78,
  LDAC = 0x79,
  LDAE = 0x7B,
  LDAH = 0x7C,
  LDAL = 0x7D,
  LDAHL = 0x7E,
  LDADE = 0x1A,
  LDBA = 0x47,
  LDBB = 0x40,
  LDBC = 0x41,
  LDBD = 0x42,
  LDBE = 0x43,
  LDBH = 0x44,
  LDBL = 0x45,
  LDBHL = 0x46,
  LDCA = 0x4F,
  LDCB = 0x48,
  LDCC = 0x49,
  LDCD = 0x4A,
  LDCE = 0x4B,
  LDCN = 0x0E,
  LDCH = 0x4C,
  LDCL = 0x4D,
  LDCHL = 0x4E,
  LDDA = 0x57,
  LDDB = 0x50,
  LDDC = 0x51,
  LDDD = 0x52,
  LDDE = 0x53,
  LDDH = 0x54,
  LDDL = 0x55,
  LDDHL = 0x56,
  LDEA = 0x5F,
  LDEB = 0x58,
  LDEC = 0x59,
  LDED = 0x5A,
  LDEH = 0x5C,
  LDEL = 0x5D,
  LDEHL = 0x5E,
  LDBCA = 0x02,
  LDDEA = 0x12,
  LDHLA = 0x77,
  LD_NN_A = 0xEA,
  LD_BC_NN = 0x01,
  LD_DE_NN = 0x11,
  LD_HL_NN = 0x21,
  LD_SP_NN = 0x31,
  LDHA = 0x67,
  LDHB = 0x60,
  LDHC = 0x61,
  LDHD = 0x62,
  LDHE = 0x63,
  LDHH = 0x64,
  LDHL = 0x65,
  LDHHL = 0x66,
  LDLB = 0x68,
  LDLC = 0x69,
  LDLD = 0x6A,
  LDLE = 0x6B,
  LDLH = 0x6C,
  LDLL = 0x6D,
  LDLHL = 0x6E,
  LDHLB = 0x70,
  LDHLC = 0x71,
  LDHLD = 0x72,
  LDHLE = 0x73,
  LDHLH = 0x74,
  LDHLL = 0x75,
  LDHL_N = 0x36,
  STOP = 0x10,
  LDIHL = 0x22,
  LDIAHL = 0x2A,
  JP_nn = 0xC3,
  JP_NZ_nn = 0xCB,
  LDH_A_n = 0xF0,
  LDH_n_A = 0xE0,
  CP_n = 0xFE,
  RET = 0xC9,
  RET_NZ = 0xC0,
  RET_z = 0xC8,
  RET_NC = 0xD0,
  RET_C = 0xD8,
  EI = 0xFB,
  CALL = 0xCD,
  CALLCNN = 0xDC,
  JRNZ = 0x20,
  JRZ = 0x28,
  JRNC = 0x30,
  JRC = 0x38,
  JR = 0x18,
  INCA = 0x3C,
  INCB = 0x04,
  INCBC = 0x03,
  INCC = 0x0C,
  INCDE = 0x13,
  INCD = 0x14,
  INCE = 0x1C,
  INCH = 0x24,
  INCL = 0x2C,
  INCHL = 0x34,
  RST_38H = 0xFF,
  RST_18H = 0xDF,

  XORA = 0xAF,
  XORB = 0xA8,
  XORC = 0xA9,
  XORD = 0xAA,
  XORE = 0xAB,
  XORH = 0xAC,
  XORL = 0xAD,
  XORHL = 0xAE,
  XOR = 0xEE,

  RRA = 0x1F,

  ADCAA = 0x8F,
  ADCAB = 0x88,
  ADCAC = 0x89,
  ADCAD = 0x8A,
  ADCAE = 0x8B,
  ADCAH = 0x8C,
  ADCAL = 0x8D,
  ADCAHL = 0x8E
}