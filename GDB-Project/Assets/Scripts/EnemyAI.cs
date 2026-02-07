using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Dependancies")]

    [Space(5)][SerializeField] GameObject[] Characters;

    [Space(2)][SerializeField] NavMeshAgent Agent;

    [Space(2)][SerializeField] AnimatorController anim;

    [Space(2)][SerializeField] Collider Head;

    [Space(2)][SerializeField] GameObject Gun;

    [Space(2)][SerializeField] AudioSource audioSource;

    [Space(2)][SerializeField] AudioClip[] hurt, dying, laugh, footsteps;
    [Range(0f, 1f)][SerializeField] float hurt_Vol, dying_Vol, laugh_Vol, footsteps_Vol;

    [Header("Enemy Stats")]

    [Space(2)][Range(1, 100)][SerializeField] int HP;
    [Space(2)][SerializeField] float Item_Drop_Height = 1.0f;
    [Header("AI Difficulty stats")]
    [Space(2)][Range(1, 90)][SerializeField] int FOV;
    [Range(20, 100)][SerializeField] int faceTargetSpeed = 50;
    [Range(1.1f, 3.0f)][SerializeField] float AgentSprintMod = 2f;
    [Range(10, 50)][SerializeField] int AgentAlertedSearchDistance = 10;
    [Range(5, 120)][SerializeField] int AgentAlertTime = 30;
    [Range(1, 10)][SerializeField] int AgentAlertPauseTime = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


}