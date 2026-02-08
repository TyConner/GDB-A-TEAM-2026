using System;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class EnemyAI : MonoBehaviour, iFootStep
{
    [Header("------Dependancies--------")]

    [Space(5)][SerializeField] GameObject[] Characters;

    [Space(2)][SerializeField] NavMeshAgent Agent;

    [Space(2)][SerializeField] Animation_State_Controller controller;
    [SerializeField] RigBuilder Ik_Rig;

    [Space(2)][SerializeField] Collider Head;

    [Space(2)][SerializeField] GameObject Gun;

    [Header("---------Audio Settings--------")]
    [Space(5)]
    [Space(2)][SerializeField] AudioSource audioSource;
    [Space(5)]
    [Space(2)][SerializeField] EnemyAudioConfig AudioConfig;
    [Header("---------Enemy Stats--------")]
    [Space(5)]
    [Space(2)][Range(1, 100)][SerializeField] int HP;
    [Space(2)][SerializeField] float Item_Drop_Height = 1.0f;
    [Range(1, 10)]
    [SerializeField] int AnimationTransSpeed = 8;
    [Header("AI Difficulty stats")]
    [Space(2)][SerializeField] EnemyStats Config;

    [Header("------Score and Team-------")]
    [Space(2)]
    [SerializeField] MyScore ScoreData;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Config != null)
        {
            AiLogic();
        }
    }

    void LocoAnim()
    {
        controller.SetSpeed(Agent.velocity.normalized.magnitude, AnimationTransSpeed);
    }
   void AiLogic()
    {

        LocoAnim();
        Agent.SetDestination(GameManager.instance.player.transform.position);
        
    }
}