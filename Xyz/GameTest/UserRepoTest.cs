using System;
using Xunit;

using Npgsql;
using Xyz.Game.Database.Postgres;

namespace Xyz.Game.Test
{
  public class UserRepoTest
  {
    private string connString;

    public UserRepoTest()
    {
      connString = "Host=localhost;Username=postgres;Password=postgres;Database=xyz;Port=5436";
    }

    [Fact]
    public void CreateUser()
    {
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IUserRepository repo = new PostgresUserRepository(_connection, null);

      User u = User.NewUser("Amir");
      repo.Create(u);

      User u2 = repo.FindById(u.ID);
      Assert.NotNull(u2);

      Assert.Equal(u.ID, u2.ID);
      Assert.Equal(u.Name, u2.Name);
      Assert.Equal(0, u.Exp);
      Assert.Equal(0, u2.Exp);

      _connection.Close();
    }

    [Fact]
    public void AddExp()
    {
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IUserRepository repo = new PostgresUserRepository(_connection, null);

      User u = User.NewUser("Amir");
      repo.Create(u);

      repo.AddExp(u, 10);
      repo.AddExp(u, 15);
      repo.AddExp(u, 13);

      User u2 = repo.FindById(u.ID);
      Assert.NotNull(u2);

      Assert.Equal(38, u2.Exp);

      _connection.Close();
    }

    [Fact]
    public void ExpSnapshotText()
    {
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IUserRepository repo = new PostgresUserRepository(_connection, null);

      User u = User.NewUser("Charlie");
      repo.Create(u);

      for (int i = 0; i < 150; i++)
      {
        repo.AddExp(u, 2);
      }

      User u2 = repo.FindById(u.ID);
      Assert.NotNull(u2);

      Assert.Equal(300, u2.Exp);

      int expFromSnapshot = 0;
      string query = "SELECT exp FROM exp_snapshot WHERE user_id = @user_id ORDER BY created_at DESC LIMIT 1";
      using (var cmd = new NpgsqlCommand(query, _connection))
      {
        cmd.Parameters.AddWithValue("user_id", u.ID);

        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            expFromSnapshot = reader.GetInt32(0);
          }
        }
      }

      Assert.Equal(200, expFromSnapshot);

      _connection.Close();
    }
  }
}