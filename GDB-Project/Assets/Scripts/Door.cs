using UnityEditor.Search;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator doorAnimator;

    bool playerInTrigger;

    private void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerInTrigger && Input.GetButtonDown("Interact") && CheckAnimationState("Closed"))
        {
            doorAnimator.SetTrigger("Open");
        }
        else if (playerInTrigger && Input.GetButtonDown("Interact") && CheckAnimationState("Open")) {
            doorAnimator.SetTrigger("Closed");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) { 
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }


    private bool CheckAnimationState(string animation) {
        AnimatorStateInfo stateInfo = doorAnimator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.IsName(animation);
    }
}
