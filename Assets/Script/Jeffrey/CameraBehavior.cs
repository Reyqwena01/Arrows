using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{

    [SerializeField] private Transform _globalLocation = null; 
    
    private bool _canMove = false;
    private static event Action _callActions; 
    public bool CanMove { get => _canMove; set => _canMove = value; }
    
    public event Action CallActions
    {
        add
        {
            _callActions -= value; 
            _callActions += value;
        }

        remove
        {
            _callActions -= value;  
        }
      
    }

    private void Start()
    {
        //_callActions();
        //CallActions += MoveCamera; 
    }

    private void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (CanMove) 
        { 
            transform.position = Vector3.Lerp(transform.position, _globalLocation.position, Time.deltaTime);
            transform.rotation = Quaternion.Euler(90, 0, 0); 
        }

    }


}
