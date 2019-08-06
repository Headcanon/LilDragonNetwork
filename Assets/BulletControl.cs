using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour, ICollidable
{
    [SerializeField] int damage;
    [SerializeField] public PlayerControl p;
    public PlayerControl Pc()
    {
        return p;
    }

    public int Value()
    {
        return -damage;
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
