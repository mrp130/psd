using System;
using System.Collections.Generic;

using Xyz.Game.ExpGainer;

namespace Xyz.Game
{
  public class GameConfig
  {
    public int WinMult { get; set; }
    public int LoseMult { get; set; }

    public static GameConfig Default()
    {
      return new GameConfig(1, 1);
    }

    public GameConfig(int winMult, int loseMult)
    {
      WinMult = winMult;
      LoseMult = loseMult;
    }

    public GameConfig() : this(1, 1) { }
  }

  public class GameFactory
  {
    public static XyzGame Create(String game, List<User> users, GameConfig config = null, List<string> moves = null)
    {
      if (config == null)
      {
        config = GameConfig.Default();
      }

      XyzGame result;

      if (game.Equals("tic-tac-toe"))
      {
        if (users.Count != 2)
        {
          throw new Exception("tic-tac-toe must be played with 2 players");
        }

        result = new TicTacToe(users[0], users[1], 3);
        GameResultHandler win = new WinHandler(new Multiplier(new TicTacToeWin(), config.WinMult));
        GameResultHandler lose = new LoseHandler(new Multiplier(new TicTacToeLose(), config.LoseMult));

        result.Attach(win);
        result.Attach(lose);
      }
      else
      {
        throw new Exception("game not found");
      }

      if (moves == null)
      {
        return result;
      }

      foreach (var move in moves)
      {
        // result.Move(move);
      }

      return result;
    }
  }
}