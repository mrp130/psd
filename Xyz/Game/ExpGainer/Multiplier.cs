using System;

namespace Xyz.Game.ExpGainer
{
  public class Multiplier : IExpGainer
  {
    private int _mult;
    private IExpGainer _wrappee;

    public Multiplier(IExpGainer wrappee, int mult)
    {
      _mult = mult;
      _wrappee = wrappee;
    }

    public int Gain()
    {
      return _wrappee.Gain() * _mult;
    }
  }
}