using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;
    [SerializeField] private float _animationSelection = 0f;
    
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
        _animator.SetFloat("SelectAnimation", _animationSelection);
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
