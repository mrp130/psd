using System;
using System.Collections.Generic;

using Npgsql;
using NpgsqlTypes;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xyz.Game.Database.Postgres
{
  public class PostgresUnitOfWork : UnitOfWork
  {
    private NpgsqlConnection _connection;
    private NpgsqlTransaction _transaction;

    private IRoomRepository _roomRepository;
    private IUserRepository _userRepository;

    public IRoomRepository RoomRepo
    {
      get
      {
        if (_roomRepository == null)
        {
          _roomRepository = new PostgresRoomRepository(_connection, _transaction);
        }
        return _roomRepository;
      }
    }

    public IUserRepository UserRepo
    {
      get
      {
        if (_userRepository == null)
        {
          _userRepository = new PostgresUserRepository(_connection, _transaction);
        }
        return _userRepository;
      }
    }

    public PostgresUnitOfWork(string connectionString)
    {
      _connection = new NpgsqlConnection(connectionString);
      _connection.Open();
      _transaction = _connection.BeginTransaction();
    }

    public void Commit()
    {
      _transaction.Commit();
    }

    public void Rollback()
    {
      _transaction.Rollback();
    }

    private bool disposed = false;
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        if (disposing)
        {
          _connection.Close();
        }

        disposed = true;
      }
    }


  }
}