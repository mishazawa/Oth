using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

public class Chess : MonoBehaviour
{
  public Value color = Value.WHITE;
  public float smooth = 1f;
  private Vector3 targetRotation;

  private bool resetRotation = false;

  public void Create (Vector3 position, Value col) {
    transform.position = new Vector3(position.x, 1f, position.z);
    color = col;
    Turn(col);
  }

  public void Turn (Value clr) {
    if (clr == Value.BLACK) {
      targetRotation = Quaternion.LookRotation(Vector3.forward, Vector3.down).eulerAngles;
    }
    if (clr == Value.WHITE) {
      targetRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up).eulerAngles;
    }
    color = clr;
    resetRotation = true;
  }


  void Update () {
    if (targetRotation != transform.eulerAngles) {
      transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, 10 * smooth * Time.deltaTime);
    } else {
      ResetTargetRotation();
    }
  }

  void ResetTargetRotation () {
    if (!resetRotation) return;
    var face = color == Value.BLACK ? Vector3.down : Vector3.up;
    transform.eulerAngles = Quaternion.LookRotation(Vector3.forward, face).eulerAngles;
    targetRotation = transform.eulerAngles;
    resetRotation = false;
  }

  public override string ToString() {
    return (int)transform.position.x + ", " + (int)transform.position.z + " " + color;
  }
}
