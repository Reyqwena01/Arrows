using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;
    [SerializeField] private int _animationSelection = 0;
    
    [SerializeField] private Rigidbody _rb = null;
    [SerializeField] private bool _isTarget = false;
    [SerializeField] private bool _isDead = false;
    [SerializeField] private GameObject[] _bones = null;

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
        _animator.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator.SetInteger("SelectAnimation", _animationSelection);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
