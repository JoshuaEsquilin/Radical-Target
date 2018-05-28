using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public int currentWeapon;
    public float maxHealth;
    public float health;
    public float maxArmor;
    public float armor;
    public float damageSplashDuration = 0.1f;
    public bool invincible;
    public Movement movementSystem;
    public CoverSystem coverSystem;
    public GameObject[] weapons;
    public string[] weaponNames;
    public Text currentWeaponUI;
    public Text healthUI;
    public Image damageSplashUI;

    private float fMouseWheel;

    void Awake() {
        healthUI = GameObject.Find("PlayerHealthUI").GetComponent<Text>();
        currentWeaponUI = GameObject.Find("CurrentWeaponUI").GetComponent<Text>();
        foreach (GameObject item in weapons) {
            item.SetActive(false);
        }
        weapons[currentWeapon].SetActive(true);
        currentWeaponUI.text = weaponNames[currentWeapon];
    }

	// Use this for initialization
	void Start () {
        healthUI.text = "Health: " + health;
    }
	
	// Update is called once per frame
	void Update () {
        if (health < 0) {
            Death();
        }
        healthUI.text = "Health: " + health;

        fMouseWheel = Input.GetAxisRaw("Mouse ScrollWheel");
        if (fMouseWheel != 0 && fMouseWheel < 0) {
            nextWeapon();
            currentWeaponUI.text = weaponNames[currentWeapon];
        } else if (fMouseWheel != 0 && fMouseWheel > 0) {
            prevWeapon();
            currentWeaponUI.text = weaponNames[currentWeapon];
        }

    }

    void nextWeapon() {
        if (weapons[currentWeapon].GetComponent<GunBehaviour>().CanSwap()) {
            weapons[currentWeapon].SetActive(false);
            currentWeapon++;
            if (currentWeapon > weapons.Length - 1) {
                currentWeapon = 0;
            }
            weapons[currentWeapon].SetActive(true);
        }
    }

    void prevWeapon() {
        if (weapons[currentWeapon].GetComponent<GunBehaviour>().CanSwap()) {
            weapons[currentWeapon].SetActive(false);
            currentWeapon--;
            if (currentWeapon < 0) {
                currentWeapon = weapons.Length - 1;
            }
            weapons[currentWeapon].SetActive(true);
        }
    }

    // Other object call this to confirm they have 'hit' the player
    public bool OnHit(float damage) {
        if (coverSystem.usingCover) {
            return false;
        } else {
            health -= damage;
            StartCoroutine(PlayerTakeDamageUINotify());
            return true;
        }
    }

    IEnumerator PlayerTakeDamageUINotify() {
        damageSplashUI.enabled = true;
        yield return new WaitForSeconds(damageSplashDuration);
        damageSplashUI.enabled = false;
        yield return null;
    }

    // Called when this object dies
    void Death() {
        if (!invincible) {
            // Game Over? Reload level, lose a life?
            SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
        } else {
            health = maxHealth;
        }
    }
}
