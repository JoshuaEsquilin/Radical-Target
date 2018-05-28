using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour {

    public int healthBonus;
    public Player player;

    // Use this for initialization
    void Start() {
        player = GameObject.Find("PlayerCarrier").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnHit() {
        if (player.health < player.maxHealth) {
            player.health += healthBonus;
            Destroy(this.gameObject);
        }
    }
}
