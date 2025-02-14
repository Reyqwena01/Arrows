using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb = null;
    [SerializeField] private bool _isTarget = false;

    private void OnCollisionEnter(Collision collision)
    {
        //BulletController bullet = collision.gameObject.GetComponent<BulletController>();

        //if (bullet != null)
        //{
        //    Die();
        //    bullet.Kill(transform);
        //}
    }

    public void Die()
    {
        //_rb.constraints = RigidbodyConstraints.FreezePosition;
        if (_isTarget)
        {

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
