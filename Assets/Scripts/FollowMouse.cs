using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour {

	public float depth = 3.0f;

	void Update (){
		transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth));
    }
}
