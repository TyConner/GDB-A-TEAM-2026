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
        if ((other.CompareTag("Ground") || other.CompareTag("Building") || other.CompareTag("Floor")) && Instagator != null)
        {
            Instagator.onStepDetected(transform.position);
            if (bDebug)
            {
                Debug.Log("FootStepEvent");
            }
                
            return;
        }
        else if( Instagator != null && !other.CompareTag("IgnoreStep"))
        {
            Debug.LogWarning("Try to Assign a valid Tag: (Ground, Floor, or Building ) to GameObject." + '\n' + "Object Detected Name: " + other.name);
        }
        else if (Instagator == null)
        {
            Debug.LogWarning("No Parent found for " + gameObject.name);
            return;
        }

    }
}
