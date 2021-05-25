using System;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Bullet Bullet { get; set; }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.GetComponent<BulletController>())
        {
            Bullet.UpdateCollision(other);   
            Destroy(gameObject);
        }
        else if(Bullet.BigDamageable)
        {
            Bullet.AddForce();
            Destroy(other.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
