using System;

namespace Xyz.Game
{
  public interface IRoomRepository
  {
    Room FindById(Guid id);
    void Create(Room room);
    void UpdateMaxPlayer(Room room, int player);
    void Join(Room room, User user);
    void ChangeGame(Room room, XyzGame game, GameConfig config);
    void Close(Room room);

    void AddMove(Room room, Move move);
  }
}