using System;

namespace Xyz.Game
{
  public interface UnitOfWork : IDisposable
  {
    void Commit();
    void Rollback();
  }
}