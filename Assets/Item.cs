using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new Item", menuName = "Item", order = 0)]
public class Item : ScriptableObject
{
    public string itemType;
    public float amount;
    public float duration;
}
