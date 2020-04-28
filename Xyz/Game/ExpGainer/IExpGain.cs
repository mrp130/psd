using System;

namespace Xyz.Game.ExpGainer
{
  public interface IExpGain
  {
    void Gain(XyzGame game);
  }

  public class TicTacToeGain : IExpGain
  {
    IExpGainer _winner;
    IExpGainer _loser;

    public TicTacToeGain(IExpGainer winner, IExpGainer loser)
    {
      _winner = winner;
      _loser = loser;
    }

    public void Gain(XyzGame game)
    {
      game.Winner.AddExp(_winner.Gain());
      game.Loser.AddExp(_loser.Gain());
    }
  }
}