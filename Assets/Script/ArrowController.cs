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

    #endregion

    #region "Properties"



    #endregion
}
