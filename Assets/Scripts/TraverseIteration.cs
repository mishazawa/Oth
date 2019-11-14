using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

public class TraveseIteration {
  private Chess[,] board;
  private Value color;
  private bool inserted = false;
  private int boardSize = 7;
  private int ZERO = -1; // shrug
  // DIRECTIONS
  private (int, int) UP         = (0, 1);
  private (int, int) DOWN       = (0, -1);
  private (int, int) LEFT       = (-1, 0);
  private (int, int) RIGHT      = (1, 0);
  private (int, int) UP_RIGHT   = (1, 1);
  private (int, int) UP_LEFT    = (-1, 1);
  private (int, int) DOWN_RIGHT = (1, -1);
  private (int, int) DOWN_LEFT  = (-1, -1);

  public TraveseIteration (Chess[,] b, Value c) {
    board = b;
    color = c;
    boardSize = b.GetLength(0);
  }

  public List<List<Chess>> TraveseDirections (int x, int y) {
    List<List<Chess>> trav = new List<List<Chess>>();

    // LIMITS
    var LIM_UP         = (x, boardSize);
    var LIM_DOWN       = (x, ZERO);
    var LIM_LEFT       = (boardSize, y);
    var LIM_RIGHT      = (ZERO, y);

    var LIM_UP_RIGHT   = (boardSize, boardSize);
    var LIM_UP_LEFT    = (ZERO, boardSize);
    var LIM_DOWN_RIGHT = (boardSize, ZERO);
    var LIM_DOWN_LEFT  = (ZERO, ZERO);

    trav.Add(RunRow((x, y), LIM_UP,    UP));
    trav.Add(RunRow((x, y), LIM_DOWN,  DOWN));
    trav.Add(RunRow((x, y), LIM_RIGHT, RIGHT));
    trav.Add(RunRow((x, y), LIM_LEFT,  LEFT));

    trav.Add(RunRow((x, y), LIM_UP_RIGHT,   UP_RIGHT));
    trav.Add(RunRow((x, y), LIM_UP_LEFT,    UP_LEFT));
    trav.Add(RunRow((x, y), LIM_DOWN_RIGHT, DOWN_RIGHT));
    trav.Add(RunRow((x, y), LIM_DOWN_LEFT,  DOWN_LEFT));

    return trav;
  }

  public bool CanInsert (List<List<Chess>> directions) {
    foreach(List<Chess> row in directions) {
      if (row != null && row.Count > 0) return true;
    }
    return false;
  }

  public bool CheckTurnInsert (List<List<Chess>> directions) {
    foreach(List<Chess> row in directions) {
      if (row != null && row.Count > 0) {
        TurnRow(row);
        if (!inserted) inserted = true;
      }
    }
    if (inserted) return true;
    return false;
  }

  private void TurnRow (List<Chess> row) {
    foreach (Chess c in row) c.Turn(color);
  }

  private List<Chess> RunRow (
    (int x, int y) start,
    (int x, int y) finish,
    (int x, int y) shift
  ) {
    var row = new List<Chess>();

    start = Clamp(start.x + shift.x, start.y + shift.y);

    do {
      if (!Boundaries.Inside(start.x, start.y, boardSize - 1)) return null;
      Chess chess = board[start.y, start.x];
      if (chess == null) return null;
      if (chess.color == color) return row;
      row.Add(chess);
      start = (start.x + shift.x, start.y + shift.y);
    } while (start.x != finish.x || start.y != finish.y);

    return null;
  }

  (int, int) Clamp (int x, int y) {
    return ((int)Mathf.Clamp(x, 0, boardSize - 1), (int)Mathf.Clamp(y, 0, boardSize - 1));
  }

}
