using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private Transform _arrowContainer;
    [SerializeField] private GameObject _arrow; 

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Instantiate(_arrow, transform.position, Quaternion.identity, _arrowContainer); 
        }
    }

}
