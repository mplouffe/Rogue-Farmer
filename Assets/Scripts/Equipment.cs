using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public EquipmentId EquipmentId;
    public Sprite EquipmentSprite;
    public int carryHands;
    public int useHands;
    public int toolCooldown;
    public string toastMessage;
    public bool firstTimeEquipping = true;

}
