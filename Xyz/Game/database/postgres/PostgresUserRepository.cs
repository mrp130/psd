using System;

using Npgsql;
using NpgsqlTypes;

namespace Xyz.Game.Database.Postgres
{
  public class PostgresUserRepository : IUserRepository
  {
    private NpgsqlConnection _connection;
    private NpgsqlTransaction _transaction;

    public PostgresUserRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
      _connection = connection;
      _transaction = transaction;
    }

    public User FindById(Guid id)
    {
      string query = "select name from \"user\" where id = @id";

      string name = "";

      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("id", id);
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            name = reader.GetString(0);
          }
          else
          {
            return null;
          }
        }
      }

      User u = new User(id, name, new Exp(getExp(id)));
      return u;
    }

    public void Create(User user)
    {
      string query = "INSERT INTO \"user\" (id, name) VALUES(@id, @name)";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("id", user.ID);
        cmd.Parameters.AddWithValue("name", user.Name);
        cmd.ExecuteNonQuery();
      }
    }

    public void AddExp(User user, int exp)
    {
      string query = "INSERT INTO exp (id, user_id, exp) VALUES(@id, @user_id, @exp)";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("id", Guid.NewGuid());
        cmd.Parameters.AddWithValue("user_id", user.ID);
        cmd.Parameters.AddWithValue("exp", exp);

        cmd.ExecuteNonQuery();
      }

      query = "SELECT count(1) FROM exp WHERE user_id = @user_id";
      int count = 0;
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("user_id", user.ID);
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            count = reader.GetInt32(0);
          }
        }
      }

      if (count % 100 == 0)
      {
        createExpSnapshot(user.ID);
      }
    }

    private int getExp(Guid id)
    {
      NpgsqlDateTime lastExpCreatedAt = new NpgsqlDateTime(0);
      int sumExp = 0;

      string query = "SELECT exp, last_exp_created_at FROM exp_snapshot WHERE user_id = @user_id ORDER BY created_at DESC LIMIT 1";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("user_id", id);

        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            sumExp = reader.GetInt32(0);
            lastExpCreatedAt = reader.GetTimeStamp(1);
          }
        }
      }

      query = "SELECT coalesce(sum(exp),0) FROM exp WHERE user_id = @user_id AND created_at > @last_exp_created_at";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("user_id", id);
        cmd.Parameters.AddWithValue("last_exp_created_at", lastExpCreatedAt);

        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            int exp = reader.GetInt32(0);
            sumExp += exp;
          }
        }
      }

      return sumExp;
    }

    private void createExpSnapshot(Guid id)
    {
      string query = "SELECT id, created_at FROM exp WHERE user_id = @user_id ORDER BY created_at DESC LIMIT 1";
      Guid lastExpId;
      NpgsqlDateTime lastExpCreatedAt;

      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("user_id", id);
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            lastExpId = reader.GetGuid(0);
            lastExpCreatedAt = reader.GetTimeStamp(1);
          }
          else
          {
            throw new Exception("last exp not found");
          }
        }
      }

      int sumExp = 0;
      query = "SELECT SUM(exp) FROM exp WHERE user_id = @user_id AND created_at <= @last_exp_created_at";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("user_id", id);
        cmd.Parameters.AddWithValue("last_exp_created_at", lastExpCreatedAt);

        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            sumExp = reader.GetInt32(0);
          }
        }
      }

      query = "INSERT INTO exp_snapshot (id, user_id, exp, last_exp_id, last_exp_created_at) VALUES(@id, @user_id, @exp, @last_exp_id, @last_exp_created_at)";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("id", Guid.NewGuid());
        cmd.Parameters.AddWithValue("user_id", id);
        cmd.Parameters.AddWithValue("exp", sumExp);
        cmd.Parameters.AddWithValue("last_exp_id", lastExpId);
        cmd.Parameters.AddWithValue("last_exp_created_at", lastExpCreatedAt);

        cmd.ExecuteNonQuery();
      }
    }

  }
}