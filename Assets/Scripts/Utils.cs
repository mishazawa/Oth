﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
  public enum Value {
    EMPTY,
    WHITE,
    BLACK
  };

  class Print {
    public static void Row (List<Chess> row) {
      string a = "[";
      if (row != null) {
        foreach (Chess c in row) {
          a+=" ";
          a+=c.color;
        }
      }
      a += " ]";
      Debug.Log(a);
    }
    public static bool Predicate (bool a, (int, int) b) {
      Debug.Log("Predicate " + a + " " + b);
      return a;
    }
  }

  class Boundaries {
    public static bool Inside (int x, int y, int boardSize) {
      return (x >= 0 && x <= boardSize && y >= 0 && y <= boardSize);
    }
  }
}
