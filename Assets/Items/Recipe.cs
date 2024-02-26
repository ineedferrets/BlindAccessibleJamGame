using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Item/Create New Recipe")]
public class Recipe : ScriptableObject
{
    public int id;
    public string recipeName;
    public List<Item> ingredients;
    public Item finalItem;
}
