using System;

public class Clock
{
  private int _timeLastInstructionTook; 
  private int _totalTimeElapsed;
  internal void SetValue(int mTimes)
  {
    _timeLastInstructionTook = mTimes;
    _totalTimeElapsed = mTimes * 4;
  }

  internal void Add(Clock clock)
  {
    _timeLastInstructionTook += clock._timeLastInstructionTook;
    _totalTimeElapsed += clock._totalTimeElapsed;
  }
}