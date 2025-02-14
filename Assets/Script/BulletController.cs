using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody = null;
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private float _impulseForce = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Movement()
    {
        _rigidbody.velocity = Vector3.forward * _speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Head"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(_rigidbody.velocity.normalized * _impulseForce, ForceMode.Impulse);
        }

        if (collision.gameObject.CompareTag("Other"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(_rigidbody.velocity.normalized * _impulseForce, ForceMode.Impulse);
        }
    }
}
