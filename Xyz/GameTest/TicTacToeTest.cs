using System;
using Xunit;

using System.Collections.Generic;

using Xyz.Game.ExpGainer;

namespace Xyz.Game.Test
{
  public class TicTacToeTest
  {
    User amir, budi;
    XyzGame game;

    public TicTacToeTest()
    {
      amir = User.NewUser("Amir");
      budi = User.NewUser("Budi");

      List<User> users = new List<User>();
      users.Add(amir);
      users.Add(budi);

      game = new TicTacToe(users);

      GameResultHandler win = new WinHandler(new TicTacToeWin());
      GameResultHandler lose = new LoseHandler(new TicTacToeLose());

      game.Attach(win);
      game.Attach(lose);
    }

    [Fact]
    public void InvalidPlayers()
    {
      Exception result = null;

      List<User> users = new List<User>();
      users.Add(User.NewUser("Amir"));
      try
      {
        XyzGame game = new TicTacToe(users);
      }
      catch (Exception e)
      {
        result = e;
      }

      Assert.True(result != null);
    }

    [Fact]
    public void InvalidMove()
    {
      game.Move(new TicTacToeMove(game, amir, 3));

      Exception ex = null;

      ex = Assert.Throws<Exception>(() => game.Move(null));
      Assert.Equal("invalid move: not a tic tac toe move", ex.Message);

      ex = Assert.Throws<Exception>(() => game.Move(new TicTacToeMove(game, amir, 3)));
      Assert.Equal("invalid move: not current player", ex.Message);

      ex = Assert.Throws<Exception>(() => game.Move(new TicTacToeMove(game, budi, 3)));
      Assert.Equal("invalid move: already filled", ex.Message);
    }

    [Fact]
    public void WinVertical()
    {
      game.Move(new TicTacToeMove(game, amir, 4));
      game.Move(new TicTacToeMove(game, budi, 3));

      game.Move(new TicTacToeMove(game, amir, 7));
      game.Move(new TicTacToeMove(game, budi, 0));

      game.Move(new TicTacToeMove(game, amir, 1));

      Assert.Equal(5, amir.Exp);
      Assert.Equal(2, budi.Exp);
    }

    [Fact]
    public void WinHorizontal()
    {
      game.Move(new TicTacToeMove(game, amir, 6));
      game.Move(new TicTacToeMove(game, budi, 3));

      game.Move(new TicTacToeMove(game, amir, 7));
      game.Move(new TicTacToeMove(game, budi, 0));

      game.Move(new TicTacToeMove(game, amir, 8));

      Assert.Equal(5, amir.Exp);
      Assert.Equal(2, budi.Exp);
    }

    [Fact]
    public void WinDiagonal1()
    {
      game.Move(new TicTacToeMove(game, amir, 0));
      game.Move(new TicTacToeMove(game, budi, 3));

      game.Move(new TicTacToeMove(game, amir, 4));
      game.Move(new TicTacToeMove(game, budi, 1));

      game.Move(new TicTacToeMove(game, amir, 8));

      Assert.Equal(5, amir.Exp);
      Assert.Equal(2, budi.Exp);
    }

    [Fact]
    public void WinDiagonal2()
    {
      game.Move(new TicTacToeMove(game, amir, 2));
      game.Move(new TicTacToeMove(game, budi, 3));

      game.Move(new TicTacToeMove(game, amir, 4));
      game.Move(new TicTacToeMove(game, budi, 0));

      game.Move(new TicTacToeMove(game, amir, 6));

      Assert.Equal(5, amir.Exp);
      Assert.Equal(2, budi.Exp);
    }
  }
}
