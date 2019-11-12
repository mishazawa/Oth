using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

public class TraveseIteration {
  private Chess[,] board;
  private bool inserted = false;
  private Value color;
  private int boardSize = 8;

  public TraveseIteration (Chess[,] b, Value c) {
    board = b;
    color = c;
    boardSize = b.GetLength(0) - 1;
  }

  public List<List<Chess>> TraveseDirections (int x, int y) {
    List<List<Chess>> trav = new List<List<Chess>>();
    // traverse up;
    trav.Add(RunRow((x, y + 1), (x, boardSize), (0, 1), color));
    // traverse down;
    trav.Add(RunRow((x, y - 1), (x, 0), (0, -1), color));
      // traverse right;
    trav.Add(RunRow((x + 1, y), (boardSize, y), (1, 0), color));
    // traverse left;
    trav.Add(RunRow((x - 1, y), (0, y), (-1, 0), color));
    // traverse diagonal up right
    trav.Add(RunRow((x + 1, y + 1), (boardSize, boardSize), (1, 1), color));
    // traverse diagonal down right
    trav.Add(RunRow((x + 1, y - 1), (boardSize, 0), (1, -1), color));
    // traverse diagonal up left
    trav.Add(RunRow((x - 1, y + 1), (0, boardSize), (-1, 1), color));
    // traverse diagonal down left
    trav.Add(RunRow((x - 1, y - 1), (0, 0), (-1, -1), color));
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
    foreach (Chess c in row) {
      c.Turn(color);
    }
  }

  private List<Chess> RunRow (
      (int x, int y) start,
      (int x, int y) finish,
      (int x, int y) shift,
      Value color
  ) {
    var row = new List<Chess>();
    start = Clamp(start.x, start.y);

    while (!(start.x == finish.x && start.y == finish.y)) {
      Chess chess = board[start.y, start.x];
      if (chess == null) return null;
      if (chess.color == color) return row;

      row.Add(chess);

      start = Clamp(
        start.x + shift.x,
        start.y + shift.y
      );
    }
    return null;
  }

  (int, int) Clamp (int x, int y) {
    return ((int)Mathf.Clamp(x, 0, boardSize), (int)Mathf.Clamp(y, 0, boardSize));
  }
}
