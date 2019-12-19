using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.gameObject.tag == "Enemy")
        {
            _other.gameObject.GetComponent<SlimeController>().IncreaseHP();
            Destroy(gameObject);
        }
    }

    public void SetFireVelocityAndDirection(float _localScaleX, float _speed)
    {
        transform.localScale = new Vector3(-_localScaleX, transform.localScale.y, transform.localScale.z);
        GetComponent<Rigidbody2D>().velocity = new Vector2(_speed * -_localScaleX, 0);
        Destroy(gameObject, 3);
    }
}
