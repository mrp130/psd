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

    [Fact]
    public void ChangeGame()
    {
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IRoomRepository repo = new PostgresRoomRepository(_connection, null);
      IUserRepository userRepo = new PostgresUserRepository(_connection, null);

      Room r = new Room(4);
      repo.Create(r);

      User amir = User.NewUser("Amir");
      User budi = User.NewUser("Budi");

      userRepo.Create(amir);
      userRepo.Create(budi);

      r.Join(amir);
      r.Join(budi);

      repo.Join(r, amir);
      repo.Join(r, budi);

      GameConfig config = new GameConfig(2, 2);
      r.StartGame("tic-tac-toe", config);

      repo.ChangeGame(r, r.Game, config);

      Room r2 = repo.FindById(r.ID);
      Assert.NotNull(r2);

      Assert.Equal("tic-tac-toe", r2.Game.Name());

      Move m = new TicTacToeMove(amir.ID, 3);
      r.Move(m);
      repo.AddMove(r, m);

      Move m2 = new TicTacToeMove(budi.ID, 2);
      r.Move(m2);
      repo.AddMove(r, m2);

      Room r3 = repo.FindById(r.ID);
      char[] board = ((TicTacToeMemento) r3.Game.GetMemento()).Board;
      Assert.Equal('-', board[1]);
      Assert.Equal('O', board[2]);
      Assert.Equal('X', board[3]);

      _connection.Close();
    }
  }
}