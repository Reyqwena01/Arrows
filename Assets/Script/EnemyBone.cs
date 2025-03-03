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
            _enemy.Die();
        }
        else
        {
            bullet.Drop();
        }
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
