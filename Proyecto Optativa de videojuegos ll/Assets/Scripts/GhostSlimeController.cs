using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSlimeController : MonoBehaviour
{
    private Animator _animator;
    private SlimeController _slimeController;
    private float _invisibleProbability;
    private bool _isInvisible;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _slimeController = GetComponent<SlimeController>();
        _invisibleProbability = 50;
        _isInvisible = false;
        InvokeRepeating("Invisibility", 0, 10);
    }

    void Invisibility()
    {
        if(!_isInvisible && Random.RandomRange(0, 100) <= _invisibleProbability)
        {
            _isInvisible = true;
        }
        else if(_isInvisible && Random.RandomRange(0, 100) <= 100 - _invisibleProbability)
        {
            _isInvisible = false;
        }
        _slimeController.SetInvincibility(_isInvisible);
        _animator.SetTrigger("Invisible");
    }
}
