using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Value {
	EMPTY,
	WHITE,
	BLACK
};

public class BoardController : MonoBehaviour {
	public float boardSize = 8;
	public GameObject chessPrefab;

	private Grid grid;
	private Value[] chesses; 

	private bool currentColor = false;

	void Awake () {
		chesses = new Value[(int)(boardSize * boardSize)];
		grid = FindObjectOfType<Grid>();
		transform.position += new Vector3(boardSize / 2f, 0f, boardSize / 2f);
		Camera.main.transform.position += new Vector3(boardSize / 2f, 0f, boardSize / 2f);
	}

    void Update() {
		HandleClick(Camera.main.ScreenPointToRay(Input.mousePosition));
    }

    void HandleClick (Ray ray) {
		RaycastHit hit;
    	if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit)) {
    		var gridCoords = grid.Nearest(hit.point);
    		if (IsEmpty(gridCoords)) {
    			Value color = GetCurrentColor();
    			GameObject chess = Instantiate(chessPrefab, gridCoords, Quaternion.identity);
    			if (color == Value.BLACK) chess.transform.Rotate(new Vector3(180f, 0, 0));


    			Insert(gridCoords, color);
    			currentColor = !currentColor;
    		}
        }
    }

    bool IsEmpty (Vector3 coords) {
    	int i = GetIndexByCoords(coords);
    	return chesses[i] == Value.EMPTY;
    }

    void Insert (Vector3 coords, Value v) {
    	chesses[GetIndexByCoords(coords)] = v;
    }

    int GetIndexByCoords (Vector3 coords) {
    	return (int) coords.x + (int) coords.z * (int) boardSize;
    }

    Value GetCurrentColor () {
    	return !currentColor ? Value.BLACK : Value.WHITE;
    }
}
