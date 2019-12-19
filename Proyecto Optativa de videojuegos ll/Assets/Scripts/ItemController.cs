using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private bool _isPotion;
    [SerializeField] private bool _arePoints;
    [SerializeField] private bool _isEvilPlant;

    void Start()
    {
        Destroy(gameObject, 15f);
    }

    public void ItemEffect(CharacterController _characterController)
    {
        if(_isPotion)
        {
            _characterController.SetStamina(5);
            if (_characterController.GetStamina() >= 20)
                _characterController.SetDefaultStamina();
            _characterController.GetGameController().SetHpText(_characterController.gameObject.name, _characterController.GetStamina());
        }
        else if(_arePoints)
        {
            _characterController.SetPoints(20);
            _characterController.GetGameController().SetPlayerPoints(_characterController.gameObject.name, 
                int.Parse(_characterController.GetPoints()));
        }
        else if(_isEvilPlant)
        {
            _characterController.SetStamina(-3);
        }
    }

    public string GetItemName()
    {
        if (_isPotion)
            return "Potion";
        else if (_arePoints)
            return "Points";
        else if (_isEvilPlant)
            return "EvilPlant";
        return "";
    }
}
