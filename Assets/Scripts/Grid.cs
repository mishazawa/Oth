using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
	
	public float size = 1f;
	
	public Vector3 Nearest (Vector3 position) {
		position -= transform.position;
		
		Vector3 res = new Vector3(
			(float)Mathf.RoundToInt(position.x / size) * size,
			(float)Mathf.RoundToInt(position.y / size) * size,
			(float)Mathf.RoundToInt(position.z / size) * size
		);

		return res + transform.position;
	}

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        for (float x = transform.position.x; x < 4; x += size)
        {
            for (float z = transform.position.z; z < 4; z += size)
            {
                var point = Nearest(new Vector3(x, 0f, z));
                Gizmos.DrawSphere(point, 0.1f);
            }
                
        }
    }
}
