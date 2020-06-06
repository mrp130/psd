using System;

namespace Xyz.Game
{
  public class User
  {
    private Guid _id;
    private string _name;
    private Exp _exp;

    public Guid ID
    {
      get
      {
        return _id;
      }
    }
    public string Name
    {
      get
      {
        return _name;
      }
    }
    public int Exp
    {
      get
      {
        return _exp.Value;
      }
    }

    public User(Guid id, string name) : this(id, name, new Exp(0)) { }

    public User(Guid id, string name, Exp score)
    {
      this._id = id;
      this._name = name;
      this._exp = score;
    }

    public static User NewUser(string name)
    {
      return new User(Guid.NewGuid(), name, new Exp());
    }

    public void AddExp(int value)
    {
      this._exp = this._exp.Add(value);
    }

    public override bool Equals(object obj)
    {
      var user = obj as User;
      if (user == null) return false;

      return this._id == user._id;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int hash = 17;
        hash = hash * 23 + _id.GetHashCode();
        return hash;
      }
    }
  }
}
