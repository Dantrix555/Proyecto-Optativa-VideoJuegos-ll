using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialController : MonoBehaviour
{
    [SerializeField] private float _speed;

    void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.gameObject.tag == "Enemy")
        {
            //Set the same of the game controller to the slime
            _other.gameObject.GetComponent<SlimeController>().SetGameControllerColor();
        }
    }

    public void SetSpecialVelocity(float _xLocalScale)
    {
        transform.localScale = new Vector3(-_xLocalScale, transform.localScale.y, transform.localScale.z);
        GetComponent<Rigidbody2D>().velocity = new Vector2(-_xLocalScale * _speed, 0);
    }
}
