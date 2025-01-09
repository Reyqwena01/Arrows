using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrows : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _arrowRb = null;

    private Vector2 _lastVelocity; 
    
    private void Update()
    {
        _arrowRb.AddForce(Vector2.right * _speed);
        _lastVelocity = _arrowRb.velocity;  

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //float speed = _lastVelocity.magnitude;
        //Vector2 direction = Vector2.Reflect(_lastVelocity.normalized, collision.contacts[0].normal);
        //_arrowRb.velocity = direction * Mathf.Max(_speed, 0f);
        //Debug.Log("Touch");

    }

}
