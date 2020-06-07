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

      List<Guid> users = new List<Guid>();
      users.Add(amir.ID);
      users.Add(budi.ID);

      game = GameFactory.Create("tic-tac-toe", users);
    }

    [Fact]
    public void InvalidPlayers()
    {
      Exception result = null;

      List<Guid> users = new List<Guid>();
      users.Add(User.NewUser("Amir").ID);
      try
      {
        XyzGame game = GameFactory.Create("tic-tac-toe", users);
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
      game.Move(new TicTacToeMove(amir.ID, 3));

      Exception ex = null;

      ex = Assert.Throws<Exception>(() => game.Move(null));
      Assert.Equal("invalid move: not a tic tac toe move", ex.Message);

      ex = Assert.Throws<Exception>(() => game.Move(new TicTacToeMove(amir.ID, 3)));
      Assert.Equal("invalid move: not current player", ex.Message);

      ex = Assert.Throws<Exception>(() => game.Move(new TicTacToeMove(budi.ID, 3)));
      Assert.Equal("invalid move: already filled", ex.Message);
    }

    [Fact]
    public void WinVertical()
    {
      game.Move(new TicTacToeMove(amir.ID, 4));
      game.Move(new TicTacToeMove(budi.ID, 3));

      game.Move(new TicTacToeMove(amir.ID, 7));
      game.Move(new TicTacToeMove(budi.ID, 0));

      game.Move(new TicTacToeMove(amir.ID, 1));
    }

    [Fact]
    public void WinHorizontal()
    {
      game.Move(new TicTacToeMove(amir.ID, 6));
      game.Move(new TicTacToeMove(budi.ID, 3));

      game.Move(new TicTacToeMove(amir.ID, 7));
      game.Move(new TicTacToeMove(budi.ID, 0));

      game.Move(new TicTacToeMove(amir.ID, 8));
    }

    [Fact]
    public void WinDiagonal1()
    {
      game.Move(new TicTacToeMove(amir.ID, 0));
      game.Move(new TicTacToeMove(budi.ID, 3));

      game.Move(new TicTacToeMove(amir.ID, 4));
      game.Move(new TicTacToeMove(budi.ID, 1));

      game.Move(new TicTacToeMove(amir.ID, 8));
    }

    [Fact]
    public void WinDiagonal2()
    {
      game.Move(new TicTacToeMove(amir.ID, 2));
      game.Move(new TicTacToeMove(budi.ID, 3));

      game.Move(new TicTacToeMove(amir.ID, 4));
      game.Move(new TicTacToeMove(budi.ID, 0));

      game.Move(new TicTacToeMove(amir.ID, 6));
    }
  }
}
