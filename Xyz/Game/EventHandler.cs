using System;

using Xyz.Game.ExpGainer;

namespace Xyz.Game
{
  public abstract class GameResultHandler : IObserver<GameResult>
  {
    protected IExpGainer _gainer;

    public GameResultHandler(IExpGainer gainer)
    {
      _gainer = gainer;
    }

    public abstract void Update(GameResult e);
  }

  public class WinHandler : GameResultHandler
  {
    public WinHandler(IExpGainer gainer) : base(gainer) { }

    public override void Update(GameResult e)
    {
      Win ev = e as Win;
      if (ev == null) return;

      ev.Player.AddExp(_gainer.Gain());
    }
  }

  public class LoseHandler : GameResultHandler
  {
    public LoseHandler(IExpGainer gainer) : base(gainer) { }

    public override void Update(GameResult e)
    {
      Lose ev = e as Lose;
      if (ev == null) return;

      ev.Player.AddExp(_gainer.Gain());
    }
  }
}