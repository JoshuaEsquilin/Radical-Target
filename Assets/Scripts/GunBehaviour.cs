using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GunBehaviour : MonoBehaviour {

    public Camera userCam;
    public AudioSource audioProducer;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip dryFireSound;
	public GameObject needtoReloadUI;
    public GameObject reloadNotifyUI;
    public GameObject outOfAmmoNotifyUI;
    public GameObject muzzleFlashParticleEffect;
    public Text ammoUI;
    public int damage; // How much damage this weapon deals (per shot)
    public int mag; // Magazine currently in this gun
    public int magSize; // Magazine size
    public int reserveMags; // How many mags are available for reloading
    public int reserveAmmo; // How much ammo is available for reloading
    public bool reloadAllowed = true;
    public bool infiniteAmmo = false;
    public bool reloadsWholeMag = false;
    public bool automaticFire = false;
    public bool reloading = false; // State of reloading
    public bool firing = false; // State of firing the weapon
    public bool drawGunShotRay = true;
    public float range = 1000f; // How far this gun can shoot
    public float fireRate = 100f; // How fast this gun can shoot
    public float reloadTime = 2.0f;

    private Ray gunShotRay;
    private RaycastHit hit;
    private ParticleSystem muzzleFlash;
    private float lastTimeFired;

	public LayerMask lMask;
	//public ParticleSystem shoteffect;
    // Use this for initialization
    void Awake() {
		needtoReloadUI.SetActive (false);
        reloadNotifyUI.SetActive(false);
        outOfAmmoNotifyUI.SetActive(false);
        reloading = false;
        if (reserveMags == 0 && reserveAmmo > 0) {
            reserveMags = (int)Math.Ceiling((double)reserveAmmo / (double)magSize);
        } else if (reserveAmmo == 0 && reserveMags > 0) {
            reserveAmmo = reserveMags * magSize;
        }
    }

    void Start() {
        audioProducer = GetComponent<AudioSource>();
        muzzleFlash = muzzleFlashParticleEffect.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update() {
        ammoUI.text = mag + " | Ammo: " + reserveAmmo + " | Mags: " + reserveMags;

        if (!reloading && mag <= 0 && reserveAmmo <= 0) {
            outOfAmmoNotifyUI.SetActive(true);
        } else {
            outOfAmmoNotifyUI.SetActive(false);
        }

		if (!reloading && mag <= 0 && reserveAmmo >= 0) {
			needtoReloadUI.SetActive(true);
		} else {
			needtoReloadUI.SetActive(false);
		}


        if (reloading) {
            reloadNotifyUI.SetActive(true);
        } else {
            reloadNotifyUI.SetActive(false);
        }

        if (automaticFire) {
            if (Input.GetButton("Fire1") && Time.time - lastTimeFired > 1 / fireRate) {
                lastTimeFired = Time.time;
                PrimaryFire();
            } else if (Input.GetButtonDown("Reload")) {
                Reload();
            }
        } else if (Input.GetButtonDown("Fire1") && Time.time - lastTimeFired > 1 / fireRate) {
            lastTimeFired = Time.time;
            PrimaryFire();
        } else if (Input.GetButtonDown("Reload")) {
            Reload();
        }
    }

    void PrimaryFire() {
        if (mag <= 0 && reserveAmmo > 0 && reloading == false) {
            OutOfAmmo(); // Reload();
        } else if (mag <= 0 && reserveAmmo <= 0 && reloading == false) {
            OutOfAmmo();
        } else if (reloading == false) {
            if (muzzleFlash.isPlaying) {
                muzzleFlash.Stop();
                muzzleFlash.Play();
            } else {
                muzzleFlash.Play();
            }
            firing = true;
            gunShotRay = userCam.ScreenPointToRay(Input.mousePosition);
            mag--;
            if (Physics.Raycast(gunShotRay, out hit, range)) {
                if (hit.collider) {
                    hit.transform.gameObject.SendMessage("OnHit", damage);
                } else if (hit.collider.tag == "Pickup") {
                    hit.transform.gameObject.SendMessage("OnHit");
                }
            }
            if (drawGunShotRay) {
                Debug.DrawLine(gunShotRay.origin, gunShotRay.direction * range, Color.red);
            }
            if (audioProducer) {
                audioProducer.clip = fireSound;
                audioProducer.Play();
            }
        }
        firing = false;
    }

    void Reload() {
        if (!reloadAllowed) {
            return;
        }

        if (infiniteAmmo) {
            if (mag < magSize) {
                StartCoroutine(ReloadInf());
            } else {
                return;
            }
        } else if (reloadsWholeMag) {
            if (mag < magSize && reserveMags > 0 && reloading == false) {
                StartCoroutine(ReloadByMag());
            } else {
                return;
            }
        } else {
            if (mag < magSize && reserveAmmo > 0 && reloading == false) {
                StartCoroutine(ReloadByAmmo());
            } else {
                return;
            }

        }
    }

    IEnumerator ReloadInf() {
        if (audioProducer) {
            audioProducer.clip = reloadSound;
            audioProducer.Play();
        }

        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        mag = magSize;
        reloading = false;
    }

    // Reloads the weapon's magazine
    IEnumerator ReloadByMag() {
        if (audioProducer) {
            audioProducer.clip = reloadSound;
            audioProducer.Play();
        }

        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        if (reserveAmmo < magSize) {
            mag = reserveAmmo;
            reserveAmmo = 0;
            reserveMags = 0;
        } else {
            reserveAmmo -= magSize;
            reserveMags--;
            mag = magSize;
        }
        reloading = false;
    }

    // Reloads the ammo in the weapon's magazine with reserve ammo
    IEnumerator ReloadByAmmo() {
        if (audioProducer) {
            audioProducer.clip = reloadSound;
            audioProducer.Play();
        }

        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        int reloadBuf = magSize - mag;
        if (reloadBuf > reserveAmmo) {
            reloadBuf = reserveAmmo;
        }
        reserveAmmo -= reloadBuf;
        mag += reloadBuf;
        reserveMags = (int)Math.Ceiling((double)reserveAmmo / (double)magSize);
        reloading = false;
    }

    void OutOfAmmo() {
        if (audioProducer) {
            audioProducer.clip = dryFireSound;
            audioProducer.Play();
        }
    }

    public bool CanSwap() {
        if (!reloading && !firing) {
            return true;
        } else {
            return false;
        }
    }
}
