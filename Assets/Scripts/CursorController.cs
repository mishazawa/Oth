﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class CursorController : MonoBehaviour
{
  [Range(0.1f, 0.5f)]
  public float countdown = 0.5f;

  private float boardSize = 8f;
  private Grid grid;
  private float cnt;
  private Material glowMaterial;
  private BoardController board;

  void Awake()
  {
    glowMaterial = GameObject.Find("/Field/Cursor/Glowing").GetComponent<Renderer>().material;
    board = FindObjectOfType<BoardController>();
    grid = FindObjectOfType<Grid>();

    cnt = countdown;
    boardSize = board.boardSize - 0.5f; // ??? idk
    transform.position = grid.Nearest(transform.position);
  }

  void Update()
  {

    if (cnt <= 0) {
      var newPos = transform.position;

      var forward = Camera.main.transform.forward;
      var right = Camera.main.transform.right;

      forward.y = 0f;
      right.y = 0f;
      forward.Normalize();
      right.Normalize();

      if (Input.GetKey(KeyCode.LeftArrow)) {
        newPos -= right;
      }

      if (Input.GetKey(KeyCode.RightArrow)) {
        newPos += right;
      }

      if (Input.GetKey(KeyCode.UpArrow)) {
        newPos += forward;
      }

      if (Input.GetKey(KeyCode.DownArrow)) {
        newPos -= forward;
      }


      if (newPos != transform.position) {
        newPos = new Vector3(
          Mathf.Clamp(newPos.x, 0, boardSize),
          Mathf.Clamp(newPos.y, 0, boardSize),
          Mathf.Clamp(newPos.z, 0, boardSize)
        );
        newPos = grid.Nearest(newPos);
        transform.position = newPos;
        cnt = countdown;
      }
      if (Input.GetKey(KeyCode.X)) {
        board.PlaceChess(transform.position);
        glowMaterial.SetColor("_Color", DrawColoredCursor(board.GetCurrentColor()));
      }
      return;
    }
    cnt -= Time.deltaTime;
  }

  Color DrawColoredCursor (Value v) {
    return v == Value.BLACK ? Color.black : Color.white;
  }
}
