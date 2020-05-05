using System;
using System.Collections.Generic;

namespace Xyz.Game
{
  public abstract class XyzGame : IObservable<GameResult>
  {
    protected bool _gameEnded;
    protected List<User> _players;

    public XyzGame(List<User> users)
    {
      _gameEnded = false;
      this._players = users;
      this.Init();
    }

    public void Move(Move move)
    {
      if (_gameEnded)
      {
        throw new Exception("game already finished");
      }

      DoMove(move);
    }

    protected abstract void Init();
    protected abstract void DoMove(Move move);
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
    protected XyzGame _game;
    protected User _player;

    public User Player
    {
      get
      {
        return _player;
      }
    }

    public Move(XyzGame game, User player)
    {
      _game = game;
      _player = player;
    }
  }
}
