using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb = null;
    [SerializeField] private Camera _camera = null;

    private void OnCollisionEnter(Collision collision)
    {
        BulletController bullet = collision.gameObject.GetComponent<BulletController>();

        if (bullet != null)
        {
            Die();
            bullet.Kill(_camera);
        }
    }

    private void Die()
    {
        _rb.constraints = RigidbodyConstraints.FreezeAll;
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
