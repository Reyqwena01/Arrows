using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private GameObject _arrowOrigine = null;
    [SerializeField] private float  _shootForce = 1.0f;

    private bool _isShoot = false;
    // Start is called before the first frame update
    #region "Methode"
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isShoot)
        {
            CalculateAngle();
        }
        
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
        if (_rigidbody.velocity != Vector2.zero)
            transform.right = -_rigidbody.velocity.normalized;
        //_lastVelocity = _arrowRb.velocity;  
    }

    private void Shoot()
    {
        //GameManager.Instance.ShootForce
        _isShoot = true;
        _rigidbody.simulated = true;
        
        _rigidbody.AddForce(-transform.right * _shootForce, ForceMode2D.Impulse);
        
    }

    private void CalculateAngle()
    {
      Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      Vector2 direction = mousePosition - (Vector2)_arrowOrigine.transform.position;

        transform.right = -direction;
        Debug.Log(transform.right);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //float speed = _lastVelocity.magnitude;
        //Vector2 direction = Vector2.Reflect(_lastVelocity.normalized, collision.contacts[0].normal);
        //_arrowRb.velocity = direction * Mathf.Max(_speed, 0f);
        //Debug.Log("Touch");

        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 wallNormal = collision.contacts[0].normal;
            Vector2 dir = Vector2.Reflect(_rigidbody.velocity, wallNormal).normalized;

            //_rigidbody.AddForce(dir * _rigidbody.velocity, ForceMode2D.Impulse);
            _rigidbody.velocity = dir * _shootForce;
            transform.right = - _rigidbody.velocity;
        }

    }

    #endregion

    #region "Properties"



    #endregion
}
