using System;
using System.Collections.Generic;

using Npgsql;
using NpgsqlTypes;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xyz.Game.Database.Postgres
{
  public class PostgresRoomRepository : IRoomRepository
  {
    private NpgsqlConnection _connection;
    private NpgsqlTransaction _transaction;

    public PostgresRoomRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
      _connection = connection;
      _transaction = transaction;
    }

    public Room FindById(Guid id)
    {
      Room r;
      string query = @"SELECT max_player FROM room WHERE id = @id AND deleted_at is null";
      using (var cmd = new NpgsqlCommand(query, _connection))
      {
        cmd.Parameters.AddWithValue("id", id);
        NpgsqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
          int max = reader.GetInt32(0);
          r = new Room(id, max);
        }
        else
        {
          return null;
        }

        reader.Close();
      }

      r.Game = getGame(r);
      return r;
    }

    public XyzGame getGame(Room room)
    {
      Guid id;
      string type;
      GameConfig config;
      List<Guid> users = new List<Guid>();
      string lastState = "";

      string query = "SELECT id, game_type, game_config FROM game WHERE room_id = @room_id AND deleted_at is null ORDER BY created_at DESC LIMIT 1";
      using (var cmd = new NpgsqlCommand(query, _connection))
      {
        cmd.Parameters.AddWithValue("room_id", room.ID);
        NpgsqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
          id = reader.GetGuid(0);
          type = reader.GetString(1);
          config = reader.GetFieldValue<GameConfig>(2);
        }
        else
        {
          return null;
        }
        reader.Close();
      }

      query = "SELECT user_id FROM room_player WHERE room_id = @room_id AND deleted_at is null";
      using (var cmd = new NpgsqlCommand(query, _connection))
      {
        cmd.Parameters.AddWithValue("room_id", room.ID);
        NpgsqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          Guid userID = reader.GetGuid(0);
          users.Add(userID);
        }
        reader.Close();
      }

      query = "SELECT state FROM game_move WHERE game_id = @game_id ORDER BY created_at DESC limit 1";
      using (var cmd = new NpgsqlCommand(query, _connection))
      {
        cmd.Parameters.AddWithValue("game_id", id);
        NpgsqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
          lastState = reader.GetString(0);
        }
        reader.Close();
      }

      return GameFactory.Create(type, users, config, lastState, new PostgresUserRepository(_connection, _transaction));
    }

    public void Create(Room room)
    {
      string query = "INSERT INTO room (id, max_player) VALUES(@id, @max)";
      using (var cmd = new NpgsqlCommand(query, _connection))
      {
        cmd.Parameters.AddWithValue("id", room.ID);
        cmd.Parameters.AddWithValue("max", room.Max);
        cmd.ExecuteNonQuery();
      }
    }

    public void UpdateMaxPlayer(Room room, int player)
    {
      string query = "UPDATE room SET max_player = @max, updated_at = CURRENT_TIMESTAMP WHERE id = @id AND deleted_at is null";
      using (var cmd = new NpgsqlCommand(query, _connection))
      {
        cmd.Parameters.AddWithValue("id", room.ID);
        cmd.Parameters.AddWithValue("max", player);
        cmd.ExecuteNonQuery();
      }
    }

    public void Join(Room room, User user)
    {
      string query = "INSERT INTO room_player(user_id, room_id) VALUES(@user_id, @room_id)";
      using (var cmd = new NpgsqlCommand(query, _connection))
      {
        cmd.Parameters.AddWithValue("user_id", user.ID);
        cmd.Parameters.AddWithValue("room_id", room.ID);
        cmd.ExecuteNonQuery();
      }
    }

    public void ChangeGame(Room room, XyzGame game, GameConfig config)
    {
      string query = "INSERT INTO game(id, room_id, game_type, game_config) VALUES(@id, @room_id, @game_type, @game_config)";
      using (var cmd = new NpgsqlCommand(query, _connection))
      {
        cmd.Parameters.AddWithValue("id", game.ID);
        cmd.Parameters.AddWithValue("room_id", room.ID);
        cmd.Parameters.AddWithValue("game_type", game.Name());

        cmd.Parameters.Add(new NpgsqlParameter("game_config", NpgsqlDbType.Jsonb) { Value = config });

        cmd.ExecuteNonQuery();
      }
    }

    public void Close(Room room)
    {
      string query = "UPDATE room SET deleted_at = CURRENT_TIMESTAMP WHERE id = @id AND deleted_at is null";
      using (var cmd = new NpgsqlCommand(query, _connection))
      {
        cmd.Parameters.AddWithValue("id", room.ID);
        cmd.ExecuteNonQuery();
      }
    }

    public void AddMove(Room room, Move move)
    {
      string query = "INSERT INTO game_move(id, game_id, move, state) VALUES(@id, @game_id, @move, @state)";
      using (var cmd = new NpgsqlCommand(query, _connection))
      {
        cmd.Parameters.AddWithValue("id", Guid.NewGuid());
        cmd.Parameters.AddWithValue("game_id", room.Game.ID);

        string jsonString = JsonSerializer.Serialize(room.Game.GetMemento());

        cmd.Parameters.Add(new NpgsqlParameter("move", NpgsqlDbType.Jsonb) { Value = move });
        cmd.Parameters.Add(new NpgsqlParameter("state", NpgsqlDbType.Jsonb) { Value = jsonString });

        cmd.ExecuteNonQuery();
      }
    }
  }
}