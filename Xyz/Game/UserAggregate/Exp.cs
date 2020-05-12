using System;

namespace Xyz.Game
{
  public class Exp
  {
    private int _value;
    public int Value
    {
      get
      {
        return _value;
      }
    }

    public Exp()
    {
      _value = 0;
    }

    public Exp(int score)
    {
      if (score < 0)
      {
        throw new Exception("score cannot be negative");
      }

      this._value = score;
    }

    public Exp Add(int value)
    {
      if (value < 0)
      {
        throw new Exception("value cannot be negative");
      }

      return new Exp(this._value + value);
    }

    public override bool Equals(object obj)
    {
      var score = obj as Exp;
      if (score == null) return false;

      return this._value == score._value;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

  }
}