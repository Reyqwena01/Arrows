using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;
    // Start is called before the first frame update
    #region "Methode"
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateAngle();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        _rigidbody.simulated = true;
        _rigidbody.AddForce(Vector2.right * GameManager.Instance.ShootForce);
    }

    private void CalculateAngle()
    {
        Convert. = Input.mousePosition - transform.position;
    }

    #endregion

    #region "Properties"



    #endregion
}
