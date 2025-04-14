using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBone : MonoBehaviour
{
    [SerializeField] private EnemyController _enemy = null;

    private void OnCollisionEnter(Collision collision)
    {
        BulletController bullet = collision.gameObject.GetComponent<BulletController>();

        if (bullet != null && !_enemy.IsDead)
        {
            _enemy.Die((bullet.transform.position - transform.position).normalized, 50f);
        }
        else if (bullet != null && _enemy.IsDead)
        {
            bullet.Drop();
        }
    }

    private void OnTriggerEnter(Collider other)
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
