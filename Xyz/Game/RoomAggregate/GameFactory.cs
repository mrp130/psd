using System;
using System.Collections.Generic;

using Xyz.Game.ExpGainer;

namespace Xyz.Game
{
  public class GameConfig
  {
    public int WinMult;
    public int LoseMult;

    public static GameConfig Default()
    {
      return new GameConfig(1, 1);
    }

    public GameConfig(int winMult, int loseMult)
    {
      WinMult = winMult;
      LoseMult = loseMult;
    }
  }

  public class GameFactory
  {
    public static XyzGame Create(String game, List<User> users, GameConfig config = null)
    {
      if (config == null)
      {
        config = GameConfig.Default();
      }

      if (game.Equals("tic-tac-toe"))
      {
        if (users.Count != 2)
        {
          throw new Exception("tic-tac-toe must be played with 2 players");
        }

        XyzGame result = new TicTacToe(users[0], users[1], 3);
        GameResultHandler win = new WinHandler(new Multiplier(new TicTacToeWin(), config.WinMult));
        GameResultHandler lose = new LoseHandler(new Multiplier(new TicTacToeLose(), config.LoseMult));

        result.Attach(win);
        result.Attach(lose);

        return result;
      }

      throw new Exception("game not found");
    }
  }
}