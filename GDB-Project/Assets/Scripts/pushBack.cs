using UnityEngine;

public class pushBack : MonoBehaviour
{

    [SerializeField] float pushBackAmount;
    [SerializeField] bool pull;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Vector3 pushDir = (other.transform.position - transform.position).normalized;
    //    Vector3 pullDir = (transform.position - other.transform.position).normalized;

    //    IPush push = other.GetComponent<IPush>();

    //    if (push != null)
    //    {
    //        if (pull)
    //        {
    //            push.getPushVel(pullDir * pushBackAmount);
    //        }
    //        else
    //        {
    //            push.getPushVel(pushDir * pushBackAmount);
    //        }
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    Vector3 pushDir = (other.transform.position - transform.position).normalized;
    //    Vector3 pullDir = (transform.position - other.transform.position).normalized;

    //    IPush push = other.GetComponent<IPush>();

    //    if (push != null)
    //    {
    //        if (pull)
    //        {
    //            push.getPushVel(pullDir * pushBackAmount);
    //        }
    //        else
    //        {
    //            push.getPushVel(pushDir * pushBackAmount);
    //        }
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        Vector3 pushDir = (other.transform.position - transform.position).normalized;
        Vector3 pullDir = (transform.position - other.transform.position).normalized;

        IPush push = other.GetComponent<IPush>();

        if (push != null)
        {
            push.getPushVel(transform.forward * pushBackAmount);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
