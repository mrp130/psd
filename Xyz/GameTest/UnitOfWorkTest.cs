using System;
using Xunit;

using Npgsql;
using Xyz.Game.Database.Postgres;

namespace Xyz.Game.Test
{
  public class UnitOfWorkTest
  {
    private string connString;

    public UnitOfWorkTest()
    {
      connString = "Host=localhost;Username=postgres;Password=postgres;Database=xyz;Port=5436";
    }

    [Fact]
    public void CreateRoom()
    {
      using (var uw = new PostgresUnitOfWork(connString))
      {
        Room r = new Room(4);
        uw.RoomRepo.Create(r);
        uw.Rollback();

        Room r2 = uw.RoomRepo.FindById(r.ID);
        Assert.Null(r2);
      }
    }

    [Fact]
    public void WinVertical()
    {
      using (var uw = new PostgresUnitOfWork(connString))
      {
        User amir = User.NewUser("Amir");
        User budi = User.NewUser("Budi");

        uw.UserRepo.Create(amir);
        uw.UserRepo.Create(budi);

        Room room = new Room(2);
        uw.RoomRepo.Create(room);

        room.Join(amir);
        uw.RoomRepo.Join(room, amir);

        room.Join(budi);
        uw.RoomRepo.Join(room, budi);

        room.StartGame("tic-tac-toe", GameConfig.Default(), uw.UserRepo);
        uw.RoomRepo.ChangeGame(room, room.Game, GameConfig.Default());

        Move move;

        move = new TicTacToeMove(amir.ID, 4);
        room.Move(move);
        uw.RoomRepo.AddMove(room, move);

        move = new TicTacToeMove(budi.ID, 3);
        room.Move(move);
        uw.RoomRepo.AddMove(room, move);

        move = new TicTacToeMove(amir.ID, 7);
        room.Move(move);
        uw.RoomRepo.AddMove(room, move);

        move = new TicTacToeMove(budi.ID, 0);
        room.Move(move);
        uw.RoomRepo.AddMove(room, move);

        move = new TicTacToeMove(amir.ID, 1);
        room.Move(move);
        uw.RoomRepo.AddMove(room, move);

        move = new TicTacToeMove(budi.ID, 3);
        Exception ex = Assert.Throws<Exception>(() => room.Move(move));
        uw.RoomRepo.AddMove(room, move);

        Assert.Equal("game already ended", ex.Message);

        Assert.Equal(5, uw.UserRepo.FindById(amir.ID).Exp);
        Assert.Equal(2, uw.UserRepo.FindById(budi.ID).Exp);
      }

    }
  }
}