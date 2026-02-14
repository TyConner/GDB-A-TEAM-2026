using UnityEngine;

public class FootSensor : MonoBehaviour
{
    [SerializeField] GameObject Parent;
    [SerializeField] bool bDebug = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.transform.IsChildOf(Parent.transform))
        {
            return;
        }
        iFootStep Instagator = transform.root.GetComponent<iFootStep>();
        if (other.CompareTag("Ground") || other.name == "Floor" && Instagator != null)
        {
            Instagator.onStepDetected(transform.position);
            if (bDebug)
            {
                Debug.Log("FootStepEvent");
            }
                
            return;
        }
        else if( other.CompareTag("Ground") != true && Instagator != null)
        {
            Debug.LogWarning("Try to Assign Tag: (Ground) to Floor GameObject." + '\n' + "Object Detected Name: " + other.name);
        }
        else if (Instagator == null)
        {
            Debug.LogWarning("No Parent found for " + gameObject.name);
            return;
        }

    }
}
