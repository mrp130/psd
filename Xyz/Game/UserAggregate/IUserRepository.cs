using System;

namespace Xyz.Game
{
  public interface IUserRepository
  {
    User FindById(Guid id);
    void Create(User user);

    void AddExp(User user, int exp);
  }
}