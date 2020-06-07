using System;

using Xyz.Game.ExpGainer;

namespace Xyz.Game
{
  public abstract class GameResultHandler : IObserver<GameResult>
  {
    protected IUserRepository userRepository;
    protected IExpGainer _gainer;

    public GameResultHandler(IUserRepository repo, IExpGainer gainer)
    {
      userRepository = repo;
      _gainer = gainer;
    }

    public abstract void Update(GameResult e);
  }

  public class WinHandler : GameResultHandler
  {
    public WinHandler(IUserRepository repo, IExpGainer gainer) : base(repo, gainer) { }

    public override void Update(GameResult e)
    {
      if(userRepository == null) return;
      
      Win ev = e as Win;
      if (ev == null) return;

      User u = userRepository.FindById(ev.Player);
      userRepository.AddExp(u, _gainer.Gain());
    }
  }

  public class LoseHandler : GameResultHandler
  {
    public LoseHandler(IUserRepository repo, IExpGainer gainer) : base(repo, gainer) { }

    public override void Update(GameResult e)
    {
      if(userRepository == null) return;

      Lose ev = e as Lose;
      if (ev == null) return;

      User u = userRepository.FindById(ev.Player);
      userRepository.AddExp(u, _gainer.Gain());
    }
  }
}