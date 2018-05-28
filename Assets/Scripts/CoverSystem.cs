using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoverSystem : MonoBehaviour {

    public float offsetX, offsetY, offsetZ;
    public GameObject player;
    public bool moving, rotating, usingCover, reloadOnlyUnderCover;

    private Vector3 origPos;
    private GameObject coverNotifyUI;
    private Movement playerMovement;
    private Player playerController;

    // Use this for initialization
	void Start () {
        origPos = transform.localPosition;
        player = GameObject.Find("PlayerCarrier");
        playerMovement = player.GetComponent<Movement>();
        playerController = player.GetComponent<Player>();
        coverNotifyUI = GameObject.Find("CoverNotifyUI");
        coverNotifyUI.SetActive(false);

        if (reloadOnlyUnderCover) {
            playerController.weapons[playerController.currentWeapon].GetComponent<GunBehaviour>().reloadAllowed = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Cover") && !playerMovement.moving && !playerMovement.rotating) {
            usingCover = true;
            coverNotifyUI.SetActive(true);
        } else  {
            usingCover = false;
            coverNotifyUI.SetActive(false);
        }

        if (reloadOnlyUnderCover) {
            playerController.weapons[playerController.currentWeapon].GetComponent<GunBehaviour>().reloadAllowed = usingCover;
        }
    }

    bool PlayerUsingCover() {
        return usingCover;
    }
}
