using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Utils;


class TraveseIteration {
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

public class BoardController : MonoBehaviour {
  public float boardSize = 8;
  public GameObject chessPrefab;
  public Text text;

  private Grid grid;
  private Chess[,] chesses;

  private bool currentColor = false;

  void Awake () {
    Camera.main.transform.position += new Vector3(boardSize / 2f, 0f, boardSize / 2f);
    transform.position += new Vector3(boardSize / 2f, 0f, boardSize / 2f);
    grid = FindObjectOfType<Grid>();
    chesses = new Chess[(int)boardSize, (int)boardSize];

    InitialChess();
  }

  void Update() {
    HandleClick(Camera.main.ScreenPointToRay(Input.mousePosition));
  }

  void HandleClick (Ray ray) {
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit)) {
      if (Input.GetMouseButtonDown(0)) {
        var gridCoords = grid.Nearest(hit.point);
        if (IsEmpty(gridCoords)) {
          Travese(gridCoords);
          if (CheckEndGame()) {
            Debug.Log("ty loh");
          }
        }
      }
    }
  }

  bool IsEmpty (Vector3 coords) {
    return GetChessByCoords(coords) == null;
  }

  void Insert (Vector3 coords) {
    Chess chess = Instantiate(chessPrefab).GetComponent<Chess>();
    chess.Create(coords, GetCurrentColor());
    InsertChessToGrid(chess);
  }

  Chess GetChessByCoords (Vector3 coords) {
    var (x, y) = GetIndices(coords);
    return chesses[y, x];
  }

  void InsertChessToGrid (Chess che) {
    var (x, y) = GetIndices(che.transform.position);
    chesses[y, x] = che;
  }

  (int x, int y) GetIndices (Vector3 coords) {
    return (x: (int)coords.x, y: (int)coords.z);
  }

  Vector3 GetCoords (int x, int y) {
    return grid.Nearest(new Vector3(x + 0.1f, 0, y + 0.1f));
  }

  Vector3 GetCoords (float x, float y) {
    return grid.Nearest(new Vector3(x + 0.1f, 0, y + 0.1f));
  }

  void InitialChess() {
    // ¯\_(ツ)_/¯
    var x = (boardSize / 2);
    var shift = 1;

    var initial = new List<(float, float)>();

    initial.Add((x, x));
    initial.Add((x - shift, x));
    initial.Add((x - shift, x - shift));
    initial.Add((x, x - shift));

    foreach(var (i, j) in initial) {
      Insert(GetCoords(i, j));
      NextColor();
    }
  }

  Value GetCurrentColor () {
    return !currentColor ? Value.BLACK : Value.WHITE;
  }

  void NextColor () {
    currentColor = !currentColor;
    text.text = GetCurrentColor().ToString();
  }

  bool CanPlaceChess (Vector3 coords) {
    var (x, y) = GetIndices(coords);
    TraveseIteration traverseIteration = new TraveseIteration(chesses, GetCurrentColor());
    return traverseIteration.CanInsert(traverseIteration.TraveseDirections(x, y));
  }

  void Travese (Vector3 coords) {
    var (x, y) = GetIndices(coords);
    TraveseIteration traverseIteration = new TraveseIteration(chesses, GetCurrentColor());
    if (traverseIteration.CheckTurnInsert(traverseIteration.TraveseDirections(x, y))) {
      Insert(coords);
      NextColor();
    }
  }

  bool CheckEndGame() {
    var possibleTurns = new List<(int, int)>();

    for (int y = 0; y < boardSize; y++) {
      for (int x = 0; x < boardSize; x++) {
        if (chesses[y, x] == null) {
          possibleTurns.Add((y, x));
        }
      }
    }

    if (possibleTurns.Count == 0) {
      Debug.Log("All filled");
      return true;
    }

    foreach(var (y, x) in possibleTurns) {
      if (CanPlaceChess(GetCoords(x, y))) return false;
    }

    return true;
  }


}
