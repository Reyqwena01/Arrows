using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewind : MonoBehaviour
{
    [SerializeField] private float _maxDuration = 5.0f;
    [SerializeField] private float _rewindSpeed = 2.0f;

    [SerializeField] private List<Transform> _multiplesLocations = new List<Transform>(); 

    private bool _isRewinding = false;


    private void Start()
    {
         
    }

    private void FixedUpdate()
    {
        
        if (_isRewinding != true)
        {
            RecordPosition();
        }
        else if (_isRewinding == true)
        {
            RewindTime();
        }

    }

    private void RecordPosition()
    {
        _multiplesLocations.Insert(0,  transform);
        if (_multiplesLocations.Count > Mathf.Round(_maxDuration / Time.fixedDeltaTime))
        {
            _multiplesLocations.RemoveAt(_multiplesLocations.Count - 1);
        }
    }

    private void RewindTime()
    {
        if (_multiplesLocations.Count > 0)
        {
            Transform locations = _multiplesLocations[0];
            transform.position = locations.position;    
            transform.rotation = locations.rotation;

            //_multiplesLocations.RemoveAt(0);
        }

        else
        {
            StopRewind();
        }

        
    }

    public void StartRewind()
    {
        _isRewinding = true;
        //Désactiver toutes les mécaniques du jeu (mettre en pause) 
    }

    private void StopRewind()
    {
        Debug.Log("STOOOOOP");
        _isRewinding = false;
    }

}
