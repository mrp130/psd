using System;
using System.Collections.Generic;

namespace Xyz.Game
{
  public abstract class XyzGame : IObservable<GameResult>
  {
    public abstract void Move(Move move);
    public abstract string Name();

    protected List<IObserver<GameResult>> _observers = new List<IObserver<GameResult>>();
    public void Attach(IObserver<GameResult> obs)
    {
      _observers.Add(obs);
    }

    public void Broadcast(GameResult e)
    {
      foreach (var obs in _observers)
      {
        obs.Update(e);
      }
    }
  }

  public abstract class Move
  {
    protected User _player;

    public User Player
    {
      get
      {
        return _player;
      }
    }

    public Move(User player)
    {
      _player = player;
    }
  }
}
