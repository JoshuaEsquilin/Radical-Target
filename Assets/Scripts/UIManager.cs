using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public bool paused = false;
    public Canvas pauseMenu;
    public Canvas quitMenu;

    void Awake() {
        Time.timeScale = 1;
    }

    void Start() {
        if (pauseMenu) {
            pauseMenu = pauseMenu.GetComponent<Canvas>();
            pauseMenu.enabled = false;
        }
        if (quitMenu) {
            quitMenu.enabled = false;
        }
    }

    void Update() {
        if (Input.GetButtonDown("Cancel")) {
            Pause();
        }
    }

    void FixedUpdate() {
        //...
    }

    void LateUpdate() {
        //...
    }

    public void Pause() {
        if ((paused && pauseMenu.enabled == true) || (!paused && pauseMenu.enabled == false)) {
            paused = !paused;
        }

        if (paused) {
            Time.timeScale = 0;
            pauseMenu.enabled = true;
        } else {
            pauseMenu.enabled = false;
            Time.timeScale = 1;
        }
    }

    public void PauseQuit() {
        pauseMenu.enabled = false;
        quitMenu.enabled = true;
    }

    public void PauseReset() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NoQuit() {
        quitMenu.enabled = false;
        pauseMenu.enabled = true;
    }

    public void YesQuit() {
        SceneManager.LoadScene(0);
    }
}
