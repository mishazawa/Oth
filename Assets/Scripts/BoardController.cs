using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Utils;
using static TraveseIteration;

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

  public void PlaceChess (Vector3 gridCoords) {
    if (IsEmpty(gridCoords)) {
      Travese(gridCoords);
      if (CheckSkipTurn()) {
        Debug.Log(GetCurrentColor() + " can't move.");
        NextColor();
        if (CheckSkipTurn()) {
          Debug.Log(GetCurrentColor() + " can't move.");
          var (white, black) = CountScore();
          Debug.Log(
            "Game over. " +
            (white > black? "WHITE": "BLACK") +
            " win. Score: (w:" +
            white +
            ", b: " +
            black +
            ")");
        }
      }
    }
  }
  void HandleClick (Ray ray) {
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit)) {
      if (Input.GetMouseButtonDown(0)) {
        PlaceChess(grid.Nearest(hit.point));
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

  public Value GetCurrentColor () {
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

  bool CheckSkipTurn() {
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
  (int white, int black) CountScore () {
    var white = new List<Chess>();
    var black = new List<Chess>();

    foreach (Chess c in chesses) {
      if (c == null) continue;
      if (c.color == Value.BLACK) black.Add(c);
      white.Add(c);
    }
    return (white: white.Count, black: black.Count);
  }
}
