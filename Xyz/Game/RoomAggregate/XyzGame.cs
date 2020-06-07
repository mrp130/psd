using System;
using System.Collections.Generic;

namespace Xyz.Game
{
  public abstract class XyzGame : IObservable<GameResult>
  {
    private Guid _id;

    public Guid ID
    {
      get
      {
        return _id;
      }
    }

    public XyzGame()
    {
      _id = Guid.NewGuid();
    }

    public abstract void Move(Move move);
    public abstract string Name();
    public abstract object GetMemento();
    public abstract void LoadMemento(object memento);

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

    public override bool Equals(object obj)
    {
      var game = obj as XyzGame;
      if (game == null) return false;

      return this._id == game.ID;
    }

    public override int GetHashCode()
    {
      return this._id.GetHashCode();
    }
  }

  public abstract class Move
  {
    protected Guid _player;

    public Guid Player
    {
      get
      {
        return _player;
      }
    }

    public Move(Guid player)
    {
      _player = player;
    }
  }
}
