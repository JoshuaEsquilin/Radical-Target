using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Boss1Behavior : MonoBehaviour {

    [SerializeField]
    public Transform spawn;
	public Node myNode;
	public AudioClip attackSound;
	public AudioClip deathSound;
	public AudioSource attackSoundSource;
	public AudioSource deathSoundSource;
    public GameObject muzzleFlashBox;
    public Renderer thisRenderer;
    public Material damageSplashMat;
    public string damageShaderName = "Unlit/Color Shader";
    public float speed, health, damage, restTime, restTimeRange, coverTime, coverTimeRange, reloadTime, damageFlashDuration;
	public bool moving, rotating, active, usingCover;

	private Player player;
	private Movement playerMovement;
	private Vector3 enemyPos;
    private ParticleSystem muzzleFlash;
    private Shader[] originalShaders;
    private Shader damageShader;
    private Color[] originalColors;

    public int mHealth;             // The bosses max health
    public float healthS;           // The speed that the health goes down
    public Text healthT;            // The text for the health bar showing how much is left
    public Image vHealth;           // The visual health
    private float currValue;        // The health at the current time
    public Canvas canvas;			// The canvas for the healthbar

    public GameObject BossesHealth;
    public float minX=2f;
	public float maxX=3f;

	public float minY=2f;
	public float maxY=3f;

	public float minZ=2f;
	public float maxZ=3f;   

            void Awake() {
        active = false;
        enemyPos = transform.position;
		player = GameObject.Find("PlayerCarrier").GetComponent<Player>();
		playerMovement = GameObject.Find("PlayerCarrier").GetComponent<Movement>();
		active = false;
        muzzleFlash = muzzleFlashBox.GetComponent<ParticleSystem>();

        originalShaders = new Shader[thisRenderer.materials.Length];
        originalColors = new Color[thisRenderer.materials.Length];
        for (int i = 0; i < thisRenderer.materials.Length; i++)
        {
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
        if (attackSoundSource && attackSound) {
			attackSoundSource.clip = attackSound;
		}
		if (deathSoundSource && deathSound) {
			deathSoundSource.clip = deathSound;
		}

		myNode.AddEnemy(this.gameObject);
		transform.position = spawn.position;

		minX=transform.position.x;
		maxX=transform.position.x+3;

		minY=transform.position.y;
		maxY=transform.position.y+3;

		minZ=transform.position.z;
		maxZ=transform.position.z+3;
	}

	// Update is called once per frame
	void Update() {
        HHealthbar();
        if (health <= 0) {
			Death();
		}

		if (!active && playerMovement.currentNode == myNode) {
			active = true;
            BossesHealth.SetActive(true);
			StartCoroutine(AIRoutine());
		}

		//transform.position =new Vector3(Mathf.PingPong(Time.time*2,maxX-minX)+minX, transform.position.y, transform.position.z);
		transform.position =new Vector3(Mathf.PingPong(Time.time*2,maxX-minX)+minX, transform.position.y, Mathf.PingPong(Time.time*2,maxZ-minZ)+minZ);
	}
    // Enemy Attack
    void Attack()
    {
       // anim.SetBool("Firing", true);
        MuzzleFlash();
        bool hitConfirm = player.OnHit(damage); // Enemy only attacks player
        if (attackSoundSource && attackSoundSource.clip)
        {
            attackSoundSource.Play();
        }

        if (hitConfirm)
        {
            //anim.SetBool("Aiming", false);
            // React: Hitting Player
        }
        else
        {
            //anim.SetBool("Aiming", false);
            // React: Missing Player
        }
        //anim.SetBool("Firing", false);
    }

    void MuzzleFlash()
    {
        if (muzzleFlash.isPlaying)
        {
            muzzleFlash.Stop();
            muzzleFlash.Play();
        }
        else
        {
            muzzleFlash.Play();
        }
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

    // Other object call this to confirm they have 'hit' this object
    public void OnHit(float damage) {
        StartCoroutine(TakeDamageEffect());
        health -= damage;
	}

	// AI Loop
	IEnumerator AIRoutine() {
		//int cycle = 0;
		active = true;
		moving = true;
		while (active) {
			yield return new WaitForSeconds(restTime + Random.Range(-restTimeRange, restTimeRange));
			Attack();
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

    // On kill
    void Death() {
		// clean up for death *sad violin*
		//deathSoundSource.Play();
		// Wait for audio to finish
		SceneManager.LoadScene(2);
		Destroy(this.gameObject);
	}
}
