using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    [SerializeField] private float _speed = 2.0f;
    [SerializeField] private Rigidbody _bulletRb;
    [SerializeField] private CameraBehavior _cameraBehavior;

    private void Update()
    {
        _bulletRb.AddForce(Vector3.ClampMagnitude(Vector3.forward, _speed)); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TriggerObject"))
        {
            gameObject.transform.DetachChildren();
            _cameraBehavior.CanMove = true; 
        }
    }
}
