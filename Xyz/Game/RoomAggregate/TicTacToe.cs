using System;

namespace Xyz.Game
{
  public class TicTacToeMemento
  {
    public bool GameEnded { set; get; }
    public Guid P1 { set; get; }
    public Guid P2 { set; get; }
    public int Size { set; get; }
    public Guid CurrentPlayer { set; get; }
    public char CurrentSymbol { set; get; }
    public char[] Board { set; get; }

    public TicTacToeMemento() { }
    public TicTacToeMemento(Guid p1, Guid p2, Guid current, int size, bool gameEnded, char currentSymbol, char[] board)
    {
      this.GameEnded = gameEnded;
      this.P1 = p1;
      this.P2 = p2;
      this.Size = size;
      this.CurrentPlayer = current;
      this.CurrentSymbol = currentSymbol;
      this.Board = board;
    }
  }
  public class TicTacToe : XyzGame
  {
    protected bool _gameEnded;

    private Guid _p1;
    private Guid _p2;
    private int _size;
    public int Size
    {
      get
      {
        return _size;
      }
    }

    private Guid _currentPlayer;
    private char _currentSymbol;
    private char[] _board;

    public TicTacToe(Guid p1, Guid p2, int size) : base()
    {
      if (p1 == null || p2 == null)
      {
        throw new Exception("tic-tac-toe must be played with 2 players");
      }

      _size = size;

      _p1 = p1;
      _p2 = p2;

      _currentPlayer = _p1;
      _currentSymbol = 'X';
      _board = new char[_size * _size];
      for (int i = 0; i < _size * _size; i++)
      {
        _board[i] = '-';
      }

      _gameEnded = false;
    }

    public override void Move(Move move)
    {
      TicTacToeMove m = move as TicTacToeMove;
      if (m == null)
      {
        throw new Exception("invalid move: not a tic tac toe move");
      }

      if (m.GridIndex < 0 || m.GridIndex > _size * _size - 1)
      {
        throw new Exception("invalid move: grid index out of bound");
      }

      if (!m.Player.Equals(_currentPlayer))
      {
        throw new Exception("invalid move: not current player");
      }

      if (_board[m.GridIndex] != '-')
      {
        throw new Exception("invalid move: already filled");
      }

      _board[m.GridIndex] = _currentSymbol;

      if (IsWin())
      {
        _gameEnded = true;

        Broadcast(new Win(_currentPlayer));

        _currentPlayer = _currentPlayer.Equals(_p1) ? _p2 : _p1;
        Broadcast(new Lose(_currentPlayer));
      }

      if (IsDraw())
      {
        Broadcast(new Draw(_p1));
        Broadcast(new Draw(_p2));
      }

      _currentPlayer = _currentPlayer.Equals(_p1) ? _p2 : _p1;
      _currentSymbol = _currentSymbol == 'X' ? 'O' : 'X';
    }

    private bool IsDraw()
    {
      for (int i = 0; i < _size; i++)
      {
        for (int j = 0; j < _size; j++)
        {
          int idx = i * _size + j;
          if (_board[idx] == '-') return false;
        }
      }

      return true;
    }

    private bool IsWin()
    {
      return checkHorizontal() || checkVertical() || checkDiagonal();
    }

    private bool checkHorizontal()
    {
      for (int i = 0; i < _size; i++)
      {
        int count = 0;
        for (int j = 0; j < _size; j++)
        {
          int idx = i * _size + j;
          if (_board[idx] != _currentSymbol) break;
          count++;
        }
        if (count == _size) return true;
      }

      return false;
    }

    private bool checkVertical()
    {
      for (int i = 0; i < _size; i++)
      {
        int count = 0;
        for (int j = 0; j < _size; j++)
        {
          int idx = j * _size + i;
          if (_board[idx] != _currentSymbol) break;
          count++;
        }
        if (count == _size) return true;
      }

      return false;
    }

    private bool checkDiagonal()
    {
      int count = 0;
      for (int i = 0, j = 0; i < _size; i++, j++)
      {
        int idx = i * _size + j;
        if (_board[idx] != _currentSymbol) break;
        count++;
      }
      if (count == _size) return true;

      count = 0;
      for (int i = 0, j = _size - 1; i < _size; i++, j--)
      {
        int idx = i * _size + j;
        if (_board[idx] != _currentSymbol) break;
        count++;
      }

      return count == _size;
    }

    public override string Name() { return "tic-tac-toe"; }
    public override object GetMemento()
    {
      return new TicTacToeMemento(_p1, _p2, _currentPlayer, _size, _gameEnded, _currentSymbol, _board);
    }

    public override void LoadMemento(object memento)
    {
      var m = memento as TicTacToeMemento;
      if (m == null) throw new Exception("wrong memento");

      this._p1 = m.P1;
      this._p2 = m.P2;
      this._currentPlayer = m.CurrentPlayer;
      this._currentSymbol = m.CurrentSymbol;
      this._gameEnded = m.GameEnded;
      this._board = m.Board;
      this._size = m.Size;
    }

  }

  public class TicTacToeMove : Move
  {
    private int _gridIndex;

    public int GridIndex
    {
      get
      {
        return _gridIndex;
      }
    }

    public TicTacToeMove(Guid player, int gridIndex) : base(player)
    {
      this._gridIndex = gridIndex;
    }

    public override bool Equals(object obj)
    {
      var move = obj as TicTacToeMove;
      if (move == null) return false;

      return this._gridIndex == move.GridIndex && this._player == move._player;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int hash = 17;
        hash = hash * 23 + _player.GetHashCode();
        hash = hash * 23 + _gridIndex.GetHashCode();
        return hash;
      }
    }
  }
}