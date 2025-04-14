using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{

    [SerializeField] private Transform _globalLocation = null;
    [SerializeField] private RewindTime _rewindTime = null;
    
    private bool _canMove = false;
    public bool CanMove { get => _canMove; set => _canMove = value; }
    

    private void Start()
    {
      
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
        }

        if (Vector3.Distance(transform.position, _globalLocation.position) < 2.5f)
        {
            _rewindTime.IsPlayingReverse = !_rewindTime.IsPlayingReverse;
            CanMove = false; 
        }

    }



}
