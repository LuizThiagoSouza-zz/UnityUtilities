using UnityEngine;

public abstract class PoolItemMonobehaviour : MonoBehaviour
{
    public Pool myPool;

    public void Despawn ()
    {
        if (myPool != null)
            myPool.RemovePoolItem (this);
    }

    public abstract void OnSpawn ();
    public abstract void OnDespawn ();
}