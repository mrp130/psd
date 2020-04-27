using System;
using System.Collections.Generic;

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

    public TicTacToeMove(XyzGame game, User player, int gridIndex) : base(game, player)
    {
      TicTacToe g = game as TicTacToe;
      if (g == null)
      {
        throw new Exception("invalid game");
      }

      int max = (g.Size * g.Size - 1);
      if (gridIndex < 0 || gridIndex > max)
      {
        throw new Exception($"index must be between 0-{max}");
      }

      this._gridIndex = gridIndex;
    }
  }

  public class TicTacToe : XyzGame
  {
    public TicTacToe(List<User> users) : base(users) { }

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

    protected override void Init()
    {
      if (this._players.Count != 2)
      {
        throw new Exception("tic-tac-toe must be played with 2 players");
      }

      this._size = 3;

      _p1 = this._players[0];
      _p2 = this._players[1];

      _currentPlayer = _p1;
      _currentSymbol = 'X';
      _board = new char[_size * _size];
      _gameEnded = false;
    }

    protected override bool DoMove(Move move)
    {
      TicTacToeMove m = move as TicTacToeMove;
      if (m == null)
      {
        throw new Exception("invalid move: not a tic tac toe move");
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

      if (IsWin()) return true;

      _currentPlayer = _currentPlayer.Equals(_p1) ? _p2 : _p1;
      _currentSymbol = _currentSymbol == 'X' ? 'O' : 'X';
      return false;
    }


    protected override void GivePlayersExp()
    {
      _currentPlayer.AddExp(5);

      _currentPlayer = _currentPlayer.Equals(_p1) ? _p2 : _p1;
      _currentPlayer.AddExp(2);
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
      for (int i = 0, j = _size-1; i < _size; i++, j--)
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