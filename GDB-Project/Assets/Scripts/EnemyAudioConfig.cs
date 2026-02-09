using UnityEngine;

[CreateAssetMenu]
public class EnemyAudioConfig : ScriptableObject
{


    public AudioClip[] hurt;
    public AudioClip[] dying;
    public AudioClip[] laugh;
    public AudioClip[] footsteps;
    public AudioClip[] gunshot;
    public float hurt_Vol;
    public float dying_Vol;
    public float laugh_Vol;
    public float footsteps_Vol;
    public float gunshot_Vol;
    

}
