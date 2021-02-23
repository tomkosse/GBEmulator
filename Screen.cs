using System;
using SDL2;

namespace GBEmulator
{
  public class Screen
  {
    public IntPtr Window { get; private set; }
    public IntPtr PrimarySurface { get; private set; }
    public IntPtr Renderer { get; private set; }
    public Memory Memory { get; }

    public Screen(Memory memory)
    {
      Memory = memory;
    }

    public void Render()
    {
      SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF); // Background color (White)
			SDL.SDL_RenderClear(Renderer);

      SDL.SDL_SetRenderDrawColor(Renderer, 0, 0, 0, 0xFF); // Foreground color (black)
      
      for(int i=200; i < 300; i++){
        SDL.SDL_RenderDrawPoint(Renderer, 420, i);
      }

			SDL.SDL_RenderPresent(Renderer);
    }

    public void Initialize()
    {
      var initResult = SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
      if (initResult < 0)
      {
        throw new InvalidOperationException($"SDL Init failed {initResult}");
      }  

			if(SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "1") == SDL.SDL_bool.SDL_FALSE) {
				// some problem
			}

			if ((Window = SDL.SDL_CreateWindow("GB.NET", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, 640, 480, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN)) == IntPtr.Zero) {
        throw new InvalidOperationException($"SDL CreateWindow failed");
			}

      Renderer = SDL.SDL_CreateRenderer(Window, 
                                        -1, 
                                        SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | 
                                        SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
      
      PrimarySurface = SDL.SDL_GetWindowSurface(Window);
      
      if((Renderer == IntPtr.Zero)){
        throw new InvalidOperationException("SDL CreateRenderer Failed");
      }
    }
  }
}