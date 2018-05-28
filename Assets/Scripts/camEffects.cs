using UnityEngine;
using System.Collections;

public class camEffects : MonoBehaviour {

    public ParticleSystem shotEffect;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    public void PlayShotEffect() {
        shotEffect.Play();
    }
}
