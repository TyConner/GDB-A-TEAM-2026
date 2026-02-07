using UnityEngine;

[CreateAssetMenu]
public class EnemyAudioConfig : ScriptableObject
{
   

    [Space(2)][SerializeField] AudioClip[] hurt, dying, laugh, footsteps;
    [Range(0f, 1f)][SerializeField] float hurt_Vol, dying_Vol, laugh_Vol, footsteps_Vol;

}
