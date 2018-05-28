using UnityEngine;
using System.Collections;

public class LookAtMouse : MonoBehaviour {
    public float depth = 10f;
    public Camera mainCam;
    private Vector3 position;
    private Ray gunRangeRay;
    private RaycastHit hit;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        gunRangeRay = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(gunRangeRay, out hit)) {
            if (hit.transform) {
                depth = 100; //depth = hit.distance;
            } else {
                depth = 100;
            }
        } else {
            depth = 100;
        }
        transform.LookAt(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth)));
    }
}
