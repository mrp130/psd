using System;
using System.Collections.Generic;

namespace Xyz.Game
{
  public abstract class XyzGame
  {
    protected bool _gameEnded;
    protected List<User> _players;

    public XyzGame(List<User> users)
    {
      _gameEnded = false;
      this._players = users;
      this.Init();
    }

    public bool Move(Move move)
    {
      if (_gameEnded)
      {
        throw new Exception("game already finished");
      }

      bool isEnded = DoMove(move);
      if (!isEnded) return isEnded;

      _gameEnded = true;

      this.GivePlayersExp();

      return isEnded;
    }

    protected abstract void Init();

    protected abstract bool DoMove(Move move);
    protected abstract void GivePlayersExp();

    public abstract string Name();
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
