using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    public Node currentNode;             // Node at which we are currently located
    public float rotateSpeed, moveSpeed; // You know what this does. I mean, really.
    public bool moving, rotating, stare; // moving, rotating need to be read-only
    public Transform stareAtThis;        // Object to stare at

    // initialization
    void Start() {
        moving = false;
        rotating = false;
        stareAtThis = currentNode.lookAt;
    }

    // Update is called once per frame
    void Update() {
        if (stare) {
            transform.LookAt(stareAtThis);
        }
        
        if (!currentNode.hasEnemies && !moving && !rotating && currentNode.nextNode) {
            StartCoroutine(RotateToThenMoveTo(currentNode.nextNode)); // Rotate to face next node, then Move to that node
            stareAtThis = currentNode.lookAt;
        } else if (!moving && !rotating) {
            transform.position = currentNode.transform.position; // If not supposed to move, don't move
        }
    }

    // Forces player came to look at a fixed point for a certain amount of time
    IEnumerator LookAtThisForTime(Transform toLookAt, float timeInSec) {
        stare = true;
        stareAtThis = toLookAt;
        yield return new WaitForSeconds(timeInSec); // Length of stare-time
        stare = false;
    }

    // Moves transform of this gameObject to a target position
    IEnumerator MoveTo(Node aNode) {
        moving = true;
        while (true) {
            transform.position = Vector3.MoveTowards(transform.position, aNode.transform.position, moveSpeed * Time.deltaTime);
            if (transform.position == aNode.transform.position) { // reached target location
                moving = false;
                currentNode = aNode; // update currentNode
                break;
            }
            yield return null;
        }
    }

    // Rotates this gameObject to face a target object
    IEnumerator RotateTo(Transform aNode) {
        Quaternion targetRotation;

        if (stare) { 
            yield break;
        }

        rotating = true;
        targetRotation = Quaternion.LookRotation(aNode.transform.position - transform.position);

        while (true) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            if (transform.rotation != targetRotation) {
                yield return null;
            } else {
                rotating = false;
                break;
            }
        }
    }

    // Moves this transform to a target position, then calls RotateTo()
    IEnumerator MoveToThenRotate(Node aNode) {
        moving = true;

        while (true) {
            transform.position = Vector3.MoveTowards(transform.position, aNode.transform.position, moveSpeed * Time.deltaTime);
            if (transform.position == aNode.transform.position) {
                moving = false;
                currentNode = aNode;
                if (aNode.lookAt) {
                    StartCoroutine(RotateTo(aNode.lookAt));
                }
                break;
            }
            yield return null;
        }
    }

    // Rotates this gameObject to face a target object, then calls MoveToThenRotate()
    IEnumerator RotateToThenMoveTo (Node aNode) {
        Quaternion targetRotation;

        if (stare) {
            StartCoroutine(MoveTo(aNode));
            yield break;
        }

        rotating = true;
        targetRotation = Quaternion.LookRotation(aNode.transform.position - transform.position);

        while (true) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            if (transform.rotation != targetRotation) {
                yield return null;
            } else {
                rotating = false;
                StartCoroutine(MoveToThenRotate(aNode));
                break;
            }
        }
    }

    // Simultaneously calls MoveTo() and RotateTo()
    IEnumerator MoveToWhileRotating (Node aNode) {
        if (!moving) {
            StartCoroutine(MoveTo(aNode));
        }

        if (!stare && !rotating) {
            StartCoroutine(RotateTo(aNode.transform));
        }
        yield break;
    }
}
