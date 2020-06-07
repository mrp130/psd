using System;

namespace Xyz.Game
{
  public abstract class GameResult
  {
    public Guid Player { get; private set; }
    public GameResult(Guid player)
    {
      this.Player = player;
    }
  }

  public class Win : GameResult
  {
    public Win(Guid player) : base(player) { }
  }

  public class Lose : GameResult
  {
    public Lose(Guid player) : base(player) { }
  }

  public class Draw : GameResult
  {
    public Draw(Guid player) : base(player) { }
  }
}