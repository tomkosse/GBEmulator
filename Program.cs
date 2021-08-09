using System;
using Serilog;

namespace GBEmulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Error)
                .WriteTo.File(new Serilog.Formatting.Json.JsonFormatter(), "log.log")
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
