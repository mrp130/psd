using System;
using Xunit;

using Xyz.Game.ExpGainer;

namespace Xyz.Game.Test
{
  public class RoomTest
  {
    User amir, budi;
    Room room;

    public RoomTest()
    {
      amir = User.NewUser("Amir");
      budi = User.NewUser("Budi");

      room = new Room(2);
      room.Join(amir);
      room.Join(budi);

      room.StartGame("tic-tac-toe");
    }

    [Fact]
    public void MaxUserExceeded()
    {
      var ex = Assert.Throws<Exception>(() => room.Join(User.NewUser("Charlie")));
      Assert.Equal("room is full", ex.Message);
    }

    [Fact]
    public void WinVertical()
    {
      room.Move(new TicTacToeMove(amir.ID, 4));
      room.Move(new TicTacToeMove(budi.ID, 3));

      room.Move(new TicTacToeMove(amir.ID, 7));
      room.Move(new TicTacToeMove(budi.ID, 0));

      room.Move(new TicTacToeMove(amir.ID, 1));
    }
  }
}