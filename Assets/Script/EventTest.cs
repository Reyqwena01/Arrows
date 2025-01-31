using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EventTest : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image _image = null;
    [SerializeField] private float _maxHp = 100f;
    [SerializeField] private RectTransform _imageRectTransform = null;
    [SerializeField] private Color _startColor = Color.white;
    [SerializeField] private Color _endColor = Color.white;
    [SerializeField] private LayerMask _raycastLayer = 0;
    private float _currentHp = 100f;

    private Vector2 _startPos = Vector2.zero;
    private Vector2 _endPos = Vector2.zero;

    public float CurrentHp
    {
        get
        {
            return _currentHp;
        }
        set
        {
            _currentHp = Mathf.Clamp(value, 0f, _maxHp);
            if (_hpChange != null) 
            {
            _hpChange();
            }
        }
    }
    
    public float HpPercentage { get => _currentHp / _maxHp; }

    private event Action _hpChange = null;
    public event Action HpChange
    {
        add
        {
            HpChange -= value;
            HpChange += value;
        }
        remove
        {
            HpChange -= value;
        }
    }

    private float TakeDamage(float damage)
    {
        CurrentHp -= damage;
        return HpPercentage;
    }

    private void UpdateBar()
    {
        _imageRectTransform.localPosition = Vector2.Lerp(_startPos, _endPos, HpPercentage);
        _image.color = Color.Lerp(_endColor, _startColor, HpPercentage);
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentHp = _maxHp;
        _startPos.x = -_imageRectTransform.rect.width;
        _hpChange += UpdateBar; //Ajout d'une méthode à une Action
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            TakeDamage(10);
        }


    }
}
