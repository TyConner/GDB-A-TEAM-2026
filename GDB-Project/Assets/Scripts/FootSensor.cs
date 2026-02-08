using UnityEngine;

public class FootSensor : MonoBehaviour
{
    [SerializeField] GameObject Parent;

    private void OnTriggerEnter(Collider other)
    {
        iFootStep Instagator = Parent.GetComponent<iFootStep>();
        if (other.CompareTag("Ground") && Instagator != null)
        {
            Instagator.onStepDetected(transform.position);
        }

    }
}
