using UnityEngine;

public interface iDamage
{
    void takeDamage(int amount, PlayerState Instigator, bool Headshot = false);

};

