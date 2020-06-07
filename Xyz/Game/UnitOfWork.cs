using System;

namespace Xyz.Game
{
  public interface UnitOfWork
  {
    void Commit();
    void Rollback();
  }
}