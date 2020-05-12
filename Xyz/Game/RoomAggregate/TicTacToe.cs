using System;

namespace Xyz.Game
{
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

    public TicTacToeMove(User player, int gridIndex) : base(player)
    {
      this._gridIndex = gridIndex;
    }
  }

  public class TicTacToe : XyzGame
  {
    protected bool _gameEnded;

    private User _p1;
    private User _p2;
    private int _size;
    public int Size
    {
      get
      {
        return _size;
      }
    }

    private User _currentPlayer;
    private char _currentSymbol;
    private char[] _board;

    public TicTacToe(User p1, User p2, int size)
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

      if (_board[m.GridIndex] != '\0')
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
          if (_board[idx] == '\0') return false;
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
  }
}