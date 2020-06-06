using System;
using Xunit;

using Npgsql;
using Xyz.Game.Database.Postgres;

namespace Xyz.Game.Test
{
  public class RoomRepoTest
  {
    private string connString;

    public RoomRepoTest()
    {
      connString = "Host=localhost;Username=postgres;Password=postgres;Database=xyz;Port=5436";
    }

    [Fact]
    public void CreateRoom()
    {
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IRoomRepository repo = new PostgresRoomRepository(_connection, null);

      Room r = new Room(4);
      repo.Create(r);

      Room r2 = repo.FindById(r.ID);
      Assert.NotNull(r2);

      Assert.Equal(r.ID, r2.ID);
      Assert.Equal(r.Max, r2.Max);

      _connection.Close();
    }

    [Fact]
    public void UpdateMaxPlayer()
    {
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IRoomRepository repo = new PostgresRoomRepository(_connection, null);

      Room r = new Room(4);
      repo.Create(r);

      repo.UpdateMaxPlayer(r, 5);

      Room r2 = repo.FindById(r.ID);
      Assert.NotNull(r2);

      Assert.Equal(5, r2.Max);

      _connection.Close();
    }

    [Fact]
    public void Close()
    {
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IRoomRepository repo = new PostgresRoomRepository(_connection, null);

      Room r = new Room(2);
      repo.Create(r);
      repo.Close(r);

      Room r2 = repo.FindById(r.ID);
      Assert.Null(r2);

      _connection.Close();
    }

    // [Fact]
    // public void ChangeGame()
    // {
    //   NpgsqlConnection _connection = new NpgsqlConnection(connString);
    //   _connection.Open();

    //   IRoomRepository repo = new PostgresRoomRepository(_connection, null);

    //   Room r = new Room(4);
    //   repo.Create(r);

    //   User amir = User.NewUser("Amir");
    //   User budi = User.NewUser("Budi");
    //   r.Join(amir);
    //   r.Join(budi);
    //   r.StartGame("tic-tac-toe");

    //   repo.ChangeGame(r, r.Game, GameConfig.Default());

    //   Room r2 = repo.FindById(r.ID);
    //   Assert.NotNull(r2);

    //   Assert.Equal("tic-tac-toe", r2.Game.Name());

    //   _connection.Close();
    // }
  }
}