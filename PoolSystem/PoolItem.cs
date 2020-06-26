using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoolItem : PoolItemMonobehaviour
{
    [SerializeField] private UnityEvent onDespawn;
    [SerializeField] private UnityEvent onSpawn;

    public override void OnDespawn ()
    {
        onDespawn?.Invoke ();
    }

    public override void OnSpawn ()
    {
        onSpawn?.Invoke ();
    }
}