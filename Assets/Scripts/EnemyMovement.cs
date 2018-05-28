using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    public Transform spawn, enemyPos;
    public Node myNode;
    public float speed;
    public bool moving, rotating;
    private Movement player;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("PlayerCarrier").GetComponent<Movement>();
        moving = false;

        transform.position = spawn.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position != enemyPos.transform.position && player.currentNode == myNode && !moving) {
            //StartCoroutine(moveTowards(enemyPos));
            transform.position = Vector3.MoveTowards(transform.position, enemyPos.position, speed * Time.deltaTime);
        }
	}

    IEnumerator moveTowards(Transform target) {
        moving = true;
        while(true) {

            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (transform.position == target.position) {
                moving = false;
                yield break;
            }
        }
    }
}
