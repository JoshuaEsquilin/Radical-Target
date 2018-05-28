using UnityEngine;
using System.Collections;

public class MouseCursorTextureModifier : MonoBehaviour {

    public bool UIMenu = false;
    public int textureSizeX;
    public int textureSizeY;
    public Texture2D cursorTexture;
    private Rect aimReticle;

    // Use this for initialization
    void Start() {
        if (!UIMenu) {
            LockCursor();
        } else {
            UnlockCursor();
        }

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonUp("Fire3")) {
            ToggleUI();
        }
    }

    void OnGUI() {
        aimReticle = new Rect(Event.current.mousePosition.x - textureSizeX / 2, Event.current.mousePosition.y - textureSizeY / 2, textureSizeX, textureSizeY);
        GUI.DrawTexture(aimReticle, cursorTexture);
    }

    // Toggles UI Elements on and off (Currently enables/disable visible mouse cursor)
    void ToggleUI() {
        if (Cursor.visible) {
            LockCursor();
        } else {
            UnlockCursor();
        }
    }

    void LockCursor() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void UnlockCursor() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
