using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour {

    public int ammoType = -1;
    public int ammoBonus;
    public Player player;

    // Use this for initialization
    void Start() {
        player = GameObject.Find("PlayerCarrier").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnHit() {
        if (ammoType >= 0 && ammoType < player.weapons.Length) {
            player.weapons[ammoType].GetComponent<GunBehaviour>().reserveAmmo += ammoBonus;
            Destroy(this.gameObject);
        }
    }
}
