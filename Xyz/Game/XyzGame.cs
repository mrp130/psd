using System;
using System.Collections.Generic;

using Xyz.Game.ExpGainer;

namespace Xyz.Game
{
  public abstract class XyzGame
  {
    protected User _winner;
    protected User _loser;

    public User Winner
    {
      get
      {
        return _winner;
      }
    }

    public User Loser
    {
      get
      {
        return _loser;
      }
    }

    protected bool _gameEnded;
    protected List<User> _players;

    public XyzGame(List<User> users)
    {
      _gameEnded = false;
      this._players = users;
      this.Init();
    }

    public bool Move(IExpGain gainer, Move move)
    {
      if (_gameEnded)
      {
        throw new Exception("game already finished");
      }

      bool isEnded = DoMove(move);
      if (!isEnded) return isEnded;

      _gameEnded = true;

      this.SetWinnerLoser();
      gainer.Gain(this);

      return isEnded;
    }

    protected abstract void Init();

    protected abstract bool DoMove(Move move);
    protected abstract void SetWinnerLoser();

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
