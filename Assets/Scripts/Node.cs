using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour{

    public Node nextNode;
    public Transform lookAt;
    public List<GameObject> enemies;
    public bool hasEnemies = false;
    public bool playerAtThisNode = false;

    private GameObject player;
    private Movement playerMovement;

    void Awake() {
        updateList();
        if (!lookAt && nextNode) {
            lookAt = nextNode.transform;
        } else if (!lookAt && !nextNode) {
            lookAt = this.transform;
        }
    }

    void Start () {
        player = GameObject.Find("PlayerCarrier");
        playerMovement = player.GetComponent<Movement>();

        if (hasEnemies) {
            for (int i = 0; i < enemies.Count - 1; i++) {
                enemies[i].SetActive(false);
            }
        }
    }

    void Update () {
        if (hasEnemies) {
            updateList();
        }

        if (playerMovement.currentNode == this.gameObject.GetComponent<Node>() && !playerAtThisNode) {
            playerAtThisNode = true;
            PlayerArrived();
        }
    }

    void updateList() {
        for (int i = 0; i < enemies.Count; i++) {
            if (enemies[i] == null) {
                enemies.Remove(enemies[i]);
            }
        }

        if(enemies.Count <= 0) {
            hasEnemies = false;
        } else {
            hasEnemies = true;
        }
    }

    public void AddEnemy(GameObject enemy) {
        enemies.Add(enemy);
    }

    void PlayerArrived() {
        updateList();
        if (hasEnemies) {
            for (int i = 0; i < enemies.Count; i++) {
                enemies[i].SetActive(true);
            }
        }
    }

    void OnDrawGizmos(){
		if(nextNode != null){
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, nextNode.transform.position);
		}
	}
}
