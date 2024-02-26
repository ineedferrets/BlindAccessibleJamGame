using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] Item itemToSpawn;

    GameObject spawnedItem = null;

    private void Start()
    {
        SpawnItem();
    }

    public bool SpawnItem()
    {
        if (itemToSpawn && itemToSpawn.prefab && spawnedItem != null)
        {
            GameObject newObj = Instantiate(itemToSpawn.prefab);
            newObj.name = itemToSpawn.name;
            newObj.transform.position = transform.position;

            ItemComponent itemComponent = newObj.GetComponent<ItemComponent>();
            itemComponent.itemAsset = itemToSpawn;

            SpriteRenderer spriteRenderer = newObj.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = itemToSpawn.icon;

            return newObj != null;
        }

        return false;
    }
}
