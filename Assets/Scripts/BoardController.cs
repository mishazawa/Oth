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
	    		}
	        }
	    	if (Input.GetMouseButtonDown(1)) {
	    		var gridCoords = grid.Nearest(hit.point);
	    		if (!IsEmpty(gridCoords)) {
					// GetChessByCoords(gridCoords).Turn();	
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

    void InitialChess() {
    	// ¯\_(ツ)_/¯
    	var x = (boardSize / 2);
	    Insert(grid.Nearest(new Vector3(x, 0, x)));
	    currentColor = !currentColor;
	    Insert(grid.Nearest(new Vector3(x - 0.5f, 0, x)));
	    currentColor = !currentColor;
	    Insert(grid.Nearest(new Vector3(x - 0.5f, 0, x - 0.5f)));
	    currentColor = !currentColor;
	    Insert(grid.Nearest(new Vector3(x, 0, x - 0.5f)));
	    currentColor = !currentColor;
    }

	Value GetCurrentColor () {
    	return !currentColor ? Value.BLACK : Value.WHITE;
    }

    void Travese (Vector3 coords) {
    	// rewrite this

		Value startColor = GetCurrentColor();
    	var (x, y) = GetIndices(coords);
		var inserted = false;
		
		// traverse up;
		var row = new List<Chess>();
    	for (int i = y + 1; i < boardSize; i++) {
    		if (chesses[i, x] == null) break;
			if (chesses[i, x].color == startColor) {
				if (row.Count == 0) break;
				if (!inserted) {
					Insert(coords);
					inserted = true;
				}
				TurnRow(row);
				break;
			}
    		row.Add(chesses[i, x]);
    	}
		row.Clear();
		
		// traverse down;
    	for (int i = y - 1; i >= 0; i--) {
    		if (chesses[i, x] == null) break;
    		if (chesses[i, x].color == startColor) {
				if (row.Count == 0) break;
				if (!inserted) {
					Insert(coords);
					inserted = true;
				}
				TurnRow(row);
				break;
			} 	
    		row.Add(chesses[i, x]);
    	}
		row.Clear();

    	// traverse right;
    	for (int i = x + 1; i < boardSize; i++) {
    		if (chesses[y, i] == null) break;
    		if (chesses[y, i].color == startColor) {
				if (row.Count == 0) break;
				if (!inserted) {
					Insert(coords);
					inserted = true;
				}
				TurnRow(row);
				break;
			} 	
    		row.Add(chesses[y, i]);
    	}
		row.Clear();
		
		// traverse left;
    	for (int i = x - 1; i >= 0; i--) {
    		if (chesses[y, i] == null) break;
    		if (chesses[y, i].color == startColor) {
				if (row.Count == 0) break;
				if (!inserted) {
					Insert(coords);
					inserted = true;
				}
				TurnRow(row);
				break;
			}
    		row.Add(chesses[y, i]);
    	}
		row.Clear();

		// traverse diagonal
		// traverse diagonal up right
		int ii = y + 1;
		int jj = x + 1;
    	for (;(ii < boardSize && jj < boardSize);) {
    		if (chesses[ii, jj] == null) break;
			if (chesses[ii, jj].color == startColor) {
				if (row.Count == 0) break;
				if (!inserted) {
					Insert(coords);
					inserted = true;
				}
				TurnRow(row);
				break;
			}
    		row.Add(chesses[ii, jj]);	
			ii++; jj++;
		}
		row.Clear();
		
		// traverse diagonal down right
		ii = y - 1;
		jj = x + 1;
    	for (;(ii >= 0 && jj < boardSize);) {
    		if (chesses[ii, jj] == null) break;
			if (chesses[ii, jj].color == startColor) {
				if (row.Count == 0) break;
				if (!inserted) {
					Insert(coords);
					inserted = true;
				}
				TurnRow(row);
				break;
			}
    		row.Add(chesses[ii, jj]);	
			ii--; jj++;
		}
		row.Clear();

		// traverse diagonal up left
		ii = y + 1;
		jj = x - 1;
    	for (;(ii < boardSize && jj >= 0);) {
    		if (chesses[ii, jj] == null) break;
			if (chesses[ii, jj].color == startColor) {
				if (row.Count == 0) break;
				if (!inserted) {
					Insert(coords);
					inserted = true;
				}
				TurnRow(row);
				break;
			}
    		row.Add(chesses[ii, jj]);	
			ii++; jj--;
		}
		row.Clear();

		// traverse diagonal down left
		ii = y - 1;
		jj = x - 1;
    	for (;(ii >= 0 && jj >= 0);) {
    		if (chesses[ii, jj] == null) break;
			if (chesses[ii, jj].color == startColor) {
				if (row.Count == 0) break;
				if (!inserted) {
					Insert(coords);
					inserted = true;
				}
				TurnRow(row);
				break;
			}
    		row.Add(chesses[ii, jj]);	
			ii--; jj--;
		}
		row.Clear();

		if (inserted) {
			currentColor = !currentColor;
		}
    }

    void TurnRow (List<Chess> row) {
		foreach (Chess c in row) {
    		c.Turn(GetCurrentColor());
		}    	
		row.Clear();
    }

}
