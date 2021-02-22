using static SDL2.SDL;

namespace GBEmulator
{
  public class Gameboy
  {
    private bool IsRunning = true;

    public Processor Processor { get; private set; }
    public Memory Memory {get; private set;}
    public Screen Screen {get; private set;}

    public Gameboy()
    {
      Memory = new Memory();

      Processor = Processor.CreateNew(Memory);
      Screen = new Screen();
    }

    public void Start(byte[] romFile)
    {
      // Init
      Memory.LoadRom(romFile);
      Screen.Initialize();

      while(IsRunning){
        while(SDL2.SDL.SDL_PollEvent(out SDL_Event ev) > 0){
          HandleEvent(ev);
        }
        Update();
        Render();
      }

      Exit();
    }

    public void Init(){
      
    }

    public void HandleEvent(SDL2.SDL.SDL_Event ev)
    {
      System.Console.WriteLine("Handling event" + ev.type.ToString());

      switch (ev.type)
      {
          case SDL_EventType.SDL_QUIT:
              IsRunning = false;
              break;
          case SDL_EventType.SDL_KEYUP:
              if(ev.key.keysym.scancode == SDL_Scancode.SDL_SCANCODE_ESCAPE) {
                IsRunning = false;
              }
              break;

      }
      // Do Something
    }

    public void Update()
    {
      System.Console.WriteLine("Updating");
    }

    public void Render()
    {
      System.Console.WriteLine("Rendering");
      Screen.Render();
    }

    public void Exit()
    {
      System.Console.WriteLine("Exiting..");
    }
  }
}