using System;

namespace Xyz.Game.ExpGainer
{
  public class TicTacToeWin : IExpGainer
  {
    public int Gain()
    {
      return 5;
    }
  }

  public class TicTacToeLose : IExpGainer
  {
    public int Gain()
    {
      return 2;
    }
  }
}