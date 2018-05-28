using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TrashmanScript : MonoBehaviour {

	public Renderer thisRenderer;
	public Transform spawn;
	public Node myNode;
	private Animator anim;
	public AudioClip attackSound;
	public AudioClip deathSound;
	public AudioClip healSound;
	public AudioSource healSoundSource;
	public AudioSource attackSoundSource;
	public AudioSource deathSoundSource;
	public float speed, health, damage, restTime, restTimeRange, coverTime, coverTimeRange, reloadTime, damageFlashDuration;
	public bool moving, rotating, active, usingCover;

	private Player player;
	private Movement playerMovement;
	private Vector3 enemyPos;
	private bool firstActive, death, walking;
	private int decision;
	private Shader[] originalShaders;
	private Shader damageShader;
	private Color[] originalColors;
	public string damageShaderName = "Unlit/Color Shader";

    public int mHealth;             // The bosses max health
    public float healthS;           // The speed that the health goes down
    public Text healthT;            // The text for the health bar showing how much is left
    public Image vHealth;           // The visual health
    private float currValue;        // The health at the current time
    public Canvas canvas;           // The canvas for the healthbar
    public GameObject BossesHealth;
    void Awake() {

		anim = GetComponent<Animator>();
		health = 200;
		damage = 7;
		restTime = 1;
		speed = 5;
		restTimeRange = .6f;
		enemyPos = transform.position;
		player = GameObject.Find("PlayerCarrier").GetComponent<Player>();
		playerMovement = GameObject.Find("PlayerCarrier").GetComponent<Movement>();
		active = false;
		originalShaders = new Shader[thisRenderer.materials.Length];
		originalColors = new Color[thisRenderer.materials.Length];
		for (int i = 0; i < thisRenderer.materials.Length; i++) {
			originalShaders[i] = thisRenderer.materials[i].shader;
			originalColors[i] = thisRenderer.materials[i].color;
		}
		damageShader = Shader.Find(damageShaderName);

		if (health <= 0) {
			Death();
		}
	}

	// Use this for initialization
	void Start () {
		moving = false;
		anim = GetComponent<Animator>();
		if (healSoundSource && healSound) {
			healSoundSource.clip = healSound;
		}
		if (attackSoundSource && attackSound) {
			attackSoundSource.clip = attackSound;
		}
		if (deathSoundSource && deathSound) {
			deathSoundSource.clip = deathSound;
		}

		myNode.AddEnemy(this.gameObject);
		transform.position = spawn.position;

	
	}

	// Update is called once per frame
	void Update() {
        HHealthbar();
        decision = Random.Range (1, 3);
		if (health <= 0) {
			Death();
		}

		if (!active && playerMovement.currentNode == myNode) {
			active = true;
            BossesHealth.SetActive(true);
            StartCoroutine(AIRoutine());
		}

		}
	// Enemy Attack
	void Attack() {
		bool hitConfirm = player.OnHit (damage); // Enemy only attacks player
		attackSoundSource.Play ();
	
		anim.SetTrigger("Attack");
		print ("ayy");
		if (hitConfirm) {
			// React to hitting player
		} else {
			//react to missing player
		}
		anim.SetTrigger ("Idle");
	}

	// Other object call this to confirm they have 'hit' this object
	public void OnHit(float damage) {
		StartCoroutine(TakeDamageEffect());
        health = health - damage;
    }

	public void heal(){
		healSoundSource.Play ();
		health = health +5;
	}
	// AI Loop
	IEnumerator AIRoutine() {
		//int cycle = 0;
		active = true;
		moving = true;
		while (active) {

			yield return new WaitForSeconds(restTime + Random.Range(-restTimeRange, restTimeRange));
            //if (decision == 1)  {
			Attack ();

			//} else if (decision == 2) {
			//	heal ();
			//}
			yield return new WaitForSeconds(reloadTime);
			//print("AI Cycle: " + cycle++);
		}
		yield break;
	}

	// Enemy takes cover
	IEnumerator takeCover(float timeInSec) {
		// go to cover position
		yield return new WaitForSeconds(timeInSec);
		// return from cover position
		yield return null;
	}

    private void HHealthbar()
    {
        // Replaces the text with the updated health
        healthT.text = ("Boss health: " + health);

        // Finds the current value by mapping the max and min position in the middle of 0 and max health
        currValue = Map(health, 0, mHealth, 0, 1);

        // The healthbar's visual amount is reflected by the remaining health
        vHealth.fillAmount = Mathf.Lerp(vHealth.fillAmount, currValue, Time.deltaTime * healthS);

    }

    // Maps the right min and max values for the Health bar 
    public float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    IEnumerator TakeDamageEffect()
    {
        for (int i = 0; i < thisRenderer.materials.Length; i++)
        {
            thisRenderer.materials[i].shader = damageShader;
            thisRenderer.materials[i].color = Color.white;
        }
        yield return new WaitForSeconds(damageFlashDuration);
        for (int i = 0; i < thisRenderer.materials.Length; i++)
        {
            thisRenderer.materials[i].shader = originalShaders[i];
            thisRenderer.materials[i].color = originalColors[i];
        }
        yield return null;
    }


    // On kill
    void Death() {
		// clean up for death *sad violin*
		deathSoundSource.Play();
		anim.SetTrigger ("Death");
        // Wait for audio to finish
        SceneManager.LoadScene(4);
        Destroy(this.gameObject);
		

	}
}

