using System;

using Npgsql;
using NpgsqlTypes;

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
      string query = "SELECT max_player FROM room WHERE id = @id AND deleted_at is null";
      using (var cmd = new NpgsqlCommand(query, _connection))
      {
        cmd.Parameters.AddWithValue("id", id);
        NpgsqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
          int max = reader.GetInt32(0);
          Room r = new Room(id, max);

          return r;
        }
      }

      return null;
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

  }
}