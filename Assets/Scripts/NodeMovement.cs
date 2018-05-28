using UnityEngine;
using System.Collections;

public class NodeMovement : MonoBehaviour {

    public Node currentNode;
    public float rotateSpeed = 45.0f;
    public float moveSpeed = 1.0f;
    public bool moving, rotating;
    private float startTime;
    private float journeyLength;

    void awake() {
        //this.transform.position = currentNode.transform.position;
    }

    void start() {
        moving = false;
        rotating = false;
        startTime = Time.time;
        //this.transform.position = currentNode.transform.position;
        //this.transform.LookAt(currentNode.lookAt.transform.position);
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, currentNode.transform.position, 0.03f);
        if (!currentNode.hasEnemies && !rotating && !moving && currentNode.nextNode) {
            StartCoroutine(GoToGoal(currentNode.nextNode));
        }
    }

    public void GoTo(Node aNode) {
        StartCoroutine(GoToGoal(aNode));
    }

    IEnumerator RotateTo(Transform aNode) {
        Quaternion goalRotation;
        rotating = true;

        goalRotation = Quaternion.LookRotation(aNode.transform.position - transform.position);

        while (true) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, goalRotation, rotateSpeed * Time.deltaTime);

            if (transform.rotation != goalRotation) {
                yield return null;
            } else {
                rotating = false;
                break;
            }
        }
    }

    IEnumerator GoToGoal(Node aNode) {
        moving = true;
        while (true) {
            //transform.position += transform.forward * moveSpeed * Time.deltaTime;
            float distCovered = (Time.time - startTime) * moveSpeed;
            journeyLength = Vector3.Distance(transform.position, aNode.transform.position);
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(transform.position, aNode.transform.position, fracJourney);
            if (Vector3.Dot(transform.forward, aNode.transform.position - transform.position) <= 0) {
                transform.position = aNode.transform.position;
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

    IEnumerator RotateAndMoveTo(Node aNode) {
        Quaternion goalRotation;
        rotating = true;
        moving = true;

        goalRotation = Quaternion.LookRotation(aNode.transform.position - transform.position);

        while (true) {
            if (rotating) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, goalRotation, rotateSpeed * Time.deltaTime);
                if (transform.rotation == goalRotation) {
                    rotating = false;
                }
            }
            if (moving) {
                //transform.position += transform.forward * moveSpeed * Time.deltaTime;
                float distCovered = (Time.time - startTime) * moveSpeed;
                journeyLength = Vector3.Distance(transform.position, aNode.transform.position);
                float fracJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(transform.position, aNode.transform.position, fracJourney);
                if (Vector3.Dot(transform.forward, aNode.transform.position - transform.position) <= 0) {
                    transform.position = aNode.transform.position;
                    currentNode = aNode;
                    moving = false;
                }
            }
            if (!rotating && !moving) {
                if (aNode.lookAt) {
                    StartCoroutine(RotateTo(aNode.lookAt));
                }
                break;
            } else {
                yield return null;
            }
        }
    }

    IEnumerator RotateThenMoveTo(Node aNode) {
        Quaternion goalRotation;
        rotating = true;

        goalRotation = Quaternion.LookRotation(aNode.transform.position - transform.position);

        while (true) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, goalRotation, rotateSpeed * Time.deltaTime);

            if (transform.rotation != goalRotation) {
                yield return null;
            } else {
                rotating = false;
                break;
            }
        }
        StartCoroutine(GoToGoal(aNode));
    }
}
