using System.Collections;
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
}
