using UnityEngine;
using System.Collections;

	public class EnemyBehaviour : MonoBehaviour {

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
		private Animator anim;
		private ParticleSystem muzzleFlash;
		private Shader[] originalShaders;
		private Shader damageShader;
		private Color[] originalColors;
		private bool firstActive;

		void Awake() {
			active = false;
			myNode.AddEnemy(this.gameObject);
			enemyPos = transform.position;
			player = GameObject.Find("PlayerCarrier").GetComponent<Player>();
			playerMovement = GameObject.Find("PlayerCarrier").GetComponent<Movement>();
			anim = GetComponent<Animator>();
			muzzleFlash = muzzleFlashBox.GetComponent<ParticleSystem>();

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
		void Start() {
			moving = false;
			if (attackSoundSource && attackSound) {
				attackSoundSource.clip = attackSound;
			}
			if (deathSoundSource && deathSound) {
				deathSoundSource.clip = deathSound;
			}

			transform.position = spawn.position;
		}

		// Update is called once per frame
		void Update() {
			if (health <= 0) {
				Death();
			}

			if (!active && playerMovement.currentNode == myNode) {
				active = true;
				MoveTowards(enemyPos);
				StartCoroutine(AIRoutine());
			}

			if (active && transform.position != enemyPos) {
				MoveTowards(enemyPos);
			}
		}
		// Enemy Attack
		void Attack() {
			anim.SetBool("Firing", true);
			MuzzleFlash();
			bool hitConfirm = player.OnHit(damage); // Enemy only attacks player
			if (attackSoundSource && attackSoundSource.clip) {
				attackSoundSource.Play();
			}

			if (hitConfirm) {
				anim.SetBool("Aiming", false);
				// React: Hitting Player
			} else {
				anim.SetBool("Aiming", false);
				// React: Missing Player
			}
			anim.SetBool("Firing", false);
		}

		void MuzzleFlash() {
			if (muzzleFlash.isPlaying) {
				muzzleFlash.Stop();
				muzzleFlash.Play();
			} else {
				muzzleFlash.Play();
			}
		}

		// Other objects call this to confirm they have 'hit' this object
		public void OnHit(float damage) {
			StartCoroutine(TakeDamageEffect());
			health -= damage;
		}

		IEnumerator TakeDamageEffect() {
			for (int i = 0; i < thisRenderer.materials.Length; i++) {
				thisRenderer.materials[i].shader = damageShader;
				thisRenderer.materials[i].color = Color.white;
			}
			yield return new WaitForSeconds(damageFlashDuration);
			for (int i = 0; i < thisRenderer.materials.Length; i++) {
				thisRenderer.materials[i].shader = originalShaders[i];
				thisRenderer.materials[i].color = originalColors[i];
			}
			yield return null;
		}

		// AI Loop
		IEnumerator AIRoutine() {
			//int cycle = 0;

			while (active) {
				while (moving) {
					yield return new WaitForEndOfFrame();
				}
				yield return new WaitForSeconds(restTime + Random.Range(-restTimeRange, restTimeRange));
				if (anim.GetBool("Aiming") != true) {
					anim.SetBool("Aiming", true);
					yield return new WaitForSeconds(0.4f);
				}
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

		// Enemy Movement
		void MoveTowards(Vector3 target) {
			moving = true;

			transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
			transform.LookAt(player.transform, Vector3.up);

			anim.SetInteger("Horizontal", Mathf.CeilToInt(transform.position.x - target.x));
			anim.SetInteger("Vertical", Mathf.CeilToInt(transform.position.y - target.y));

			if (transform.position == target) {
				moving = false;
				anim.SetInteger("Horizontal", 0);
				anim.SetInteger("Vertical", 0);
			}
		}


		// On kill
		void Death() {
			// clean up for death *sad violin*
			anim.SetBool("Alive", false);
			if (deathSoundSource && deathSoundSource.clip) {
				deathSoundSource.Play();
			}
			// Wait for audio to finish
			Destroy(this.gameObject);
		}
	}
