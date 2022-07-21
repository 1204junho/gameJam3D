using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vending_machine : MonoBehaviour
{
    public GameObject Item;
    public int price;
    public void DropItem()
    {
        Instantiate(Item,Vector3.forward+transform.position, transform.rotation);
    }
}
