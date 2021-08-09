using System.Threading;
using Serilog;
using static SDL2.SDL;

namespace GBEmulator
{
  public class Gameboy
  {
    private bool IsRunning = true;
    private bool DebugStep = true;
    private bool Debugging = true;

    public Processor Processor { get; private set; }
    public Memory Memory { get; private set; }
    public Screen Screen { get; private set; }

    public Gameboy()
    {
      Memory = new Memory();

      Processor = Processor.CreateNew(Memory);
      Screen = new Screen(Memory);
    }

    public void Start(byte[] romFile)
    {
      // Init
      Init(romFile);

      while (IsRunning)
      {
        while (SDL2.SDL.SDL_PollEvent(out SDL_Event ev) > 0)
        {
          HandleEvent(ev);
        }
        if(!Processor.Stopped)
        {
          Update();
          Render();
        }
      }

      Exit();
    }

    public void Init(byte[] romFile)
    {
      Memory.LoadRom(romFile);
      Memory.SetDefaults();
      Screen.Initialize();
    }

    public void HandleEvent(SDL2.SDL.SDL_Event ev)
    {
      switch (ev.type)
      {
        case SDL_EventType.SDL_QUIT:
          IsRunning = false;
          break;
        case SDL_EventType.SDL_KEYUP:
          if (ev.key.keysym.scancode == SDL_Scancode.SDL_SCANCODE_ESCAPE)
          {
            IsRunning = false;
          }
          if (ev.key.keysym.scancode == SDL_Scancode.SDL_SCANCODE_RETURN)
          {
            DebugStep = true;
          }
          if (ev.key.keysym.scancode == SDL_Scancode.SDL_SCANCODE_R)
          {
            Debugging = false;
          }
          break;

      }
      // Do Something
    }

    public void Update()
    {
      if (Debugging && DebugStep)
      {
        Processor.ExecuteNextInstruction();
        DebugStep = false;
      }
      else if (!Debugging)
      {
        Processor.ExecuteNextInstruction();
      }
    }

    public void Render()
    {
      Processor.DoVBlank();
      Screen.Render();
    }

    public void Exit()
    {
      Log.Logger.Verbose("Exiting..");
    }
  }
}