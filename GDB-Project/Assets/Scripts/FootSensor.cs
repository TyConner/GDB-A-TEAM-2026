using UnityEngine;

public class FootSensor : MonoBehaviour
{
    [SerializeField] GameObject Parent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.transform.IsChildOf(Parent.transform))
        {
            return;
        }
        iFootStep Instagator = Parent.GetComponent<iFootStep>();
        if (other.CompareTag("Ground") || other.name == "Floor" && Instagator != null)
        {
            Instagator.onStepDetected(transform.position);
            Debug.Log("FootStepEvent");
            return;
        }
        else if( other.CompareTag("Ground") != true && Instagator != null)
        {
            Debug.LogWarning("Try to Assign Tag: (Ground) to Floor GameObject." + '\n' + "Object Detected Name: " + other.name);
        }
        else if (Instagator == null)
        {
            Debug.Log("No Parent found for " + gameObject.name);
            return;
        }

    }
}
