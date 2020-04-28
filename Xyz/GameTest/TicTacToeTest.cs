using System;
using Xunit;

using System.Collections.Generic;

using Xyz.Game.ExpGainer;

namespace Xyz.Game.Test
{
  public class TicTacToeTest
  {
    [Fact]
    public void InvalidPlayers()
    {
      Exception result = null;

      IExpGainer win = new TicTacToeWin();
      IExpGainer lose = new TicTacToeLose();

      List<User> users = new List<User>();
      users.Add(User.NewUser("Amir"));
      try
      {
        XyzGame game = new TicTacToe(win, lose, users);
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
      List<User> users = new List<User>();

      IExpGainer win = new TicTacToeWin();
      IExpGainer lose = new TicTacToeLose();

      User amir = User.NewUser("Amir");
      User budi = User.NewUser("Budi");

      users.Add(amir);
      users.Add(budi);

      XyzGame game = new TicTacToe(win, lose, users);

      Assert.Equal("tic-tac-toe", game.Name());

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
      List<User> users = new List<User>();

      User amir = User.NewUser("Amir");
      User budi = User.NewUser("Budi");
      
      IExpGainer win = new TicTacToeWin();
      IExpGainer lose = new TicTacToeLose();

      users.Add(amir);
      users.Add(budi);

      XyzGame game = new TicTacToe(win, lose, users);

      Assert.Equal("tic-tac-toe", game.Name());

      game.Move(new TicTacToeMove(game, amir, 4));
      game.Move(new TicTacToeMove(game, budi, 3));

      game.Move(new TicTacToeMove(game, amir, 7));
      game.Move(new TicTacToeMove(game, budi, 0));

      bool isEnded = game.Move(new TicTacToeMove(game, amir, 1));
      Assert.True(isEnded);

      Assert.Equal(5, amir.Exp);
      Assert.Equal(2, budi.Exp);
    }

    [Fact]
    public void WinHorizontal()
    {
      List<User> users = new List<User>();

      IExpGainer win = new TicTacToeWin();
      IExpGainer lose = new TicTacToeLose();

      User amir = User.NewUser("Amir");
      User budi = User.NewUser("Budi");

      users.Add(amir);
      users.Add(budi);

      XyzGame game = new TicTacToe(win, lose, users);

      Assert.Equal("tic-tac-toe", game.Name());

      game.Move(new TicTacToeMove(game, amir, 6));
      game.Move(new TicTacToeMove(game, budi, 3));

      game.Move(new TicTacToeMove(game, amir, 7));
      game.Move(new TicTacToeMove(game, budi, 0));

      bool isEnded = game.Move(new TicTacToeMove(game, amir, 8));
      Assert.True(isEnded);

      Assert.Equal(5, amir.Exp);
      Assert.Equal(2, budi.Exp);
    }

    [Fact]
    public void WinDiagonal1()
    {
      List<User> users = new List<User>();

      User amir = User.NewUser("Amir");
      User budi = User.NewUser("Budi");

      IExpGainer win = new TicTacToeWin();
      IExpGainer lose = new TicTacToeLose();

      users.Add(amir);
      users.Add(budi);

      XyzGame game = new TicTacToe(win, lose, users);

      Assert.Equal("tic-tac-toe", game.Name());

      game.Move(new TicTacToeMove(game, amir, 0));
      game.Move(new TicTacToeMove(game, budi, 3));

      game.Move(new TicTacToeMove(game, amir, 4));
      game.Move(new TicTacToeMove(game, budi, 1));

      bool isEnded = game.Move(new TicTacToeMove(game, amir, 8));
      Assert.True(isEnded);

      Assert.Equal(5, amir.Exp);
      Assert.Equal(2, budi.Exp);
    }

    [Fact]
    public void WinDiagonal2()
    {
      List<User> users = new List<User>();

      User amir = User.NewUser("Amir");
      User budi = User.NewUser("Budi");

      IExpGainer win = new TicTacToeWin();
      IExpGainer lose = new TicTacToeLose();

      users.Add(amir);
      users.Add(budi);

      XyzGame game = new TicTacToe(win, lose, users);

      Assert.Equal("tic-tac-toe", game.Name());

      game.Move(new TicTacToeMove(game, amir, 2));
      game.Move(new TicTacToeMove(game, budi, 3));

      game.Move(new TicTacToeMove(game, amir, 4));
      game.Move(new TicTacToeMove(game, budi, 0));

      bool isEnded = game.Move(new TicTacToeMove(game, amir, 6));
      Assert.True(isEnded);

      Assert.Equal(5, amir.Exp);
      Assert.Equal(2, budi.Exp);
    }

    [Fact]
    public void TestMultiplier()
    {
      List<User> users = new List<User>();

      User amir = User.NewUser("Amir");
      User budi = User.NewUser("Budi");

      IExpGainer win = new TicTacToeWin();
      IExpGainer lose = new TicTacToeLose();

      win = new Multiplier(win, 2);
      win = new Multiplier(win, 3);

      users.Add(amir);
      users.Add(budi);

      XyzGame game = new TicTacToe(win, lose, users);

      Assert.Equal("tic-tac-toe", game.Name());

      game.Move(new TicTacToeMove(game, amir, 2));
      game.Move(new TicTacToeMove(game, budi, 3));

      game.Move(new TicTacToeMove(game, amir, 4));
      game.Move(new TicTacToeMove(game, budi, 0));

      bool isEnded = game.Move(new TicTacToeMove(game, amir, 6));
      Assert.True(isEnded);

      Assert.Equal(30, amir.Exp);
      Assert.Equal(2, budi.Exp);
    }
  }
}
