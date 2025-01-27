using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        BulletController bullet = collision.gameObject.GetComponent<BulletController>();

        if (bullet != null)
        {
            Die();
            bullet.Kill();
        }
    }

    private void Die()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
