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
  }
}