using System;
using System.Collections.Generic;

using Xyz.Game.ExpGainer;

namespace Xyz.Game
{
  public abstract class XyzGame
  {
    protected IExpGainer _winExpGainer;
    protected IExpGainer _loseExpGainer;

    protected bool _gameEnded;
    protected List<User> _players;

    public XyzGame(IExpGainer win, IExpGainer lose, List<User> users)
    {
      _winExpGainer = win;
      _loseExpGainer = lose;

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
