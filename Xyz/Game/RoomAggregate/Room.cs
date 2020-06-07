using System;
using System.Collections.Generic;

namespace Xyz.Game
{
  public class Room
  {
    private Guid _id;
    private List<Guid> _users;
    private int _max;
    private XyzGame _game;

    public Guid ID
    {
      get
      {
        return _id;
      }
    }

    public int Max
    {
      get
      {
        return _max;
      }
    }

    public XyzGame Game
    {
      set
      {
        _game = value;
      }
      get
      {
        return _game;
      }
    }

    public int UserCount
    {
      get
      {
        return _users.Count;
      }
    }

    public Room(Guid id, int max) : this(max)
    {
      this._id = id;
    }

    public Room(int max)
    {
      if (max < 1)
      {
        throw new Exception("max must be greater than 1");
      }

      _id = Guid.NewGuid();
      _users = new List<Guid>();
      _max = max;
      _game = null;
    }

    public void Join(User u)
    {
      if (_users.Count >= _max)
      {
        throw new Exception("room is full");
      }

      if (_game != null)
      {
        throw new Exception("game already started");
      }

      _users.Add(u.ID);
    }

    public void StartGame(String game, GameConfig config = null)
    {
      _game = GameFactory.Create(game, _users, config);
    }

    public void Move(Move move)
    {
      if (_game == null)
      {
        throw new Exception("game not started yet");
      }

      _game.Move(move);
    }

    public override bool Equals(object obj)
    {
      var room = obj as Room;
      if (room == null) return false;

      return this._id == room.ID;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return this._id.GetHashCode();
      }
    }
  }
}