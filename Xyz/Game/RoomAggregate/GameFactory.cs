using System;
using System.Collections.Generic;

using Xyz.Game.ExpGainer;

using System.Text.Json;
using System.Text.Json.Serialization;

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
    public static XyzGame Create(String game, List<Guid> users, GameConfig config = null, string lastState = "", IUserRepository userRepo = null)
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
        GameResultHandler win = new WinHandler(userRepo, new Multiplier(new TicTacToeWin(), config.WinMult));
        GameResultHandler lose = new LoseHandler(userRepo, new Multiplier(new TicTacToeLose(), config.LoseMult));

        result.Attach(win);
        result.Attach(lose);
      }
      else
      {
        throw new Exception("game not found");
      }

      if (lastState == null || lastState == "")
      {
        return result;
      }

      if (game.Equals("tic-tac-toe"))
      {
        TicTacToeMemento memento = JsonSerializer.Deserialize<TicTacToeMemento>(lastState);
        result.LoadMemento(memento);
      }
      return result;
    }
  }
}