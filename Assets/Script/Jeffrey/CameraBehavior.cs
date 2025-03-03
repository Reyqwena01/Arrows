using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{

    [SerializeField] private Transform _globalLocation = null;
    [SerializeField] private RewindTime _rewindTime = null;
    [SerializeField] private Transform _bulletTransform = null; 
    
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

        if (Vector3.Distance(transform.position, _globalLocation.position) <= 2.5f)
        {
            _rewindTime.IsPlayingReverse = !_rewindTime.IsPlayingReverse;
            //SwtichCameraPosition();
            CanMove = false; 
        }

    }

    private void SwtichCameraPosition()
    {
        Vector3 offset = new Vector3(0, 5, -8);
        transform.position = _bulletTransform.position + offset;
        transform.rotation = Quaternion.Euler(-90, 0, 0);

    }


}
