using System;

namespace Xyz.Game
{
  public abstract class GameResult
  {
    public User Player { get; private set; }
    public GameResult(User player)
    {
      this.Player = player;
    }
  }

  public class Win : GameResult
  {
    public Win(User player) : base(player) { }
  }

  public class Lose : GameResult
  {
    public Lose(User player) : base(player) { }
  }

  public class Draw : GameResult
  {
    public Draw(User player) : base(player) { }
  }
}