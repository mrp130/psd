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
      string query = @"select name, coalesce(exp, 0) from ""user""
left join (
    select user_id, sum(exp) as exp from exp group by user_id
) e on id = e.user_id where id = @id";

      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("id", id);
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            string name = reader.GetString(0);
            int exp = reader.GetInt32(1);

            User u = new User(id, name, new Exp(exp));
            return u;
          }
        }
      }

      return null;
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
    }

  }
}