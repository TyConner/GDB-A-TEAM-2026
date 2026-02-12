using UnityEngine;

public interface iOwner
{
    public PlayerState OwningPlayer();

    public void SetOwningPlayer(PlayerState player);

}
