using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    // THIS IS A PLACEHOLDER SCRIPT

    public int hp;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (hp <= 0) {
            Destroy(this.gameObject);
        }
	}
}
