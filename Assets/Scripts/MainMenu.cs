using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public Canvas mainMenu;
    public Canvas levelSelectMenu;
    public Canvas quitMenu;

    void Start() {
        if (mainMenu) {
            mainMenu.enabled = true;
        }
        if (quitMenu) {
            quitMenu.enabled = false;
        }
        if (levelSelectMenu) {
            levelSelectMenu.enabled = false;
        }
    }

    public void StartButton() {
        SceneManager.LoadScene(1);
    }

    public void LevelSelectButton() {
        mainMenu.enabled = false;
        levelSelectMenu.enabled = true;
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene(0);
    }

    public void levelSelectStage(int stageNo) {
        SceneManager.LoadScene(stageNo);
    }

    public void loadSecond()
    {
        SceneManager.LoadScene(3);
    }

    public void loadFirst()
    {
        SceneManager.LoadScene(1);
    }

    public void levelSelectBack() {
        levelSelectMenu.enabled = false;
        mainMenu.enabled = true;
    }

    public void ExitButton() {
        mainMenu.enabled = false;
        quitMenu.enabled = true;
    }

    public void NoQuit() {
        quitMenu.enabled = false;
        mainMenu.enabled = true;
    }

    public void YesQuit() {
        Application.Quit();
    }
}
