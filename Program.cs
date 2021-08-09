﻿using System;
using Serilog;

namespace GBEmulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
                
            var filepath = args[0];

            var romFile = System.IO.File.ReadAllBytes(filepath);

            Gameboy g = new Gameboy();
            g.Start(romFile);
        }
        
        // static void Main(string[] args)
        // {
        //     SDL2Test.SDL2Test.RunTest();
        // }
    }
}
