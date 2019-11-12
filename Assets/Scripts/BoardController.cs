using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

public class BoardController : MonoBehaviour {
  public float boardSize = 8;
  public GameObject chessPrefab;

  private Grid grid;

  private Chess[,] chesses;

  private bool currentColor = false;

  void Awake () {
    Camera.main.transform.position += new Vector3(boardSize / 2f, 0f, boardSize / 2f);
    transform.position += new Vector3(boardSize / 2f, 0f, boardSize / 2f);

    grid = FindObjectOfType<Grid>();
    chesses = new Chess[8,8];

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
          CheckEndGame();
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
  }

  List<Chess> RunRow (
    (int x, int y) start,
    (int x, int y) finish,
    (int x, int y) shift,
    Value color
  ) {
    var row = new List<Chess>();

    while (!(start.x == finish.x && start.y == finish.y)) {

      Chess chess = chesses[start.y, start.x];
      if (chess == null) return null;
      if (chess.color == color) break;
      row.Add(chess);

      start.x += shift.x;
      start.y += shift.y;

    }

    return row;
  }

  bool CanPlaceChess (Vector3 coords) {
    // todo
    return false;
  }

  bool CheckTurnInsert (List<Chess> row, bool inserted, Vector3 coords) {
    if (row != null && row.Count > 0) {
      TurnRow(row);
      if (!inserted) {
        Insert(coords);
        return true;
      }
    }
    return inserted;
  }

  void Travese (Vector3 coords) {
    var inserted = false;
    var (x, y) = GetIndices(coords);
    Value startColor = GetCurrentColor();

    // traverse up;
    inserted = CheckTurnInsert(RunRow((x, y + 1), (x, (int)boardSize), (0, 1), startColor), inserted, coords);
    // traverse down;
    inserted = CheckTurnInsert(RunRow((x, y - 1), (x, 0), (0, -1), startColor), inserted, coords);
      // traverse right;
    inserted = CheckTurnInsert(RunRow((x + 1, y), ((int)boardSize, y), (1, 0), startColor), inserted, coords);
    // traverse left;
    inserted = CheckTurnInsert(RunRow((x - 1, y), (0, y), (-1, 0), startColor), inserted, coords);
    // traverse diagonal up right
    inserted = CheckTurnInsert(RunRow((x + 1, y + 1), ((int)boardSize, (int)boardSize), (1, 1), startColor), inserted, coords);
    // traverse diagonal down right
    inserted = CheckTurnInsert(RunRow((x + 1, y - 1), ((int)boardSize, 0), (1, -1), startColor), inserted, coords);
    // traverse diagonal up left
    inserted = CheckTurnInsert(RunRow((x - 1, y + 1), (0, (int)boardSize), (-1, 1), startColor), inserted, coords);
    // traverse diagonal down left
    inserted = CheckTurnInsert(RunRow((x - 1, y - 1), (0, 0), (-1, -1), startColor), inserted, coords);

    if (inserted) NextColor();
  }

  void TurnRow (List<Chess> row) {
    foreach (Chess c in row) {
      c.Turn(GetCurrentColor());
    }
  }

  void CheckEndGame() {
    var possibleTurns = new List<(int, int)>();
    for (int y = 0; y < boardSize; y++) {
      for (int x = 0; x < boardSize; x++) {
        if (chesses[y, x] == null) {
          possibleTurns.Add((x, y));
        }
      }
    }
    if (possibleTurns.Count == 0) {
      Debug.Log("All filled");
    }

    foreach(var (x, y) in possibleTurns) {
      // travese and check;

    }
  }

}
