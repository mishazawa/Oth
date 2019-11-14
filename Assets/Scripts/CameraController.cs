using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  public GameObject target;

  void Start()
  {
    transform.LookAt(target.transform.position);
  }

  void Update()
  {
    var rotation = Vector3.zero;
    if (Input.GetKey(KeyCode.A)) rotation = Vector3.up;
    if (Input.GetKey(KeyCode.D)) rotation = Vector3.down;
    transform.RotateAround (target.transform.position, rotation , 20 * Time.deltaTime * 10f);
  }
}
