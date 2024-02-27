using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] public Item itemToSpawn;

    GameObject spawnedItem = null;
    SpriteRenderer previewRenderer;

    private void Start()
    {
        if (previewRenderer)
        {
            previewRenderer.sprite = null;
        }
    }

    public bool SpawnItem()
    {
        if (itemToSpawn && itemToSpawn.prefab && spawnedItem == null)
        {
            GameObject newObj = Instantiate(itemToSpawn.prefab);
            newObj.name = itemToSpawn.name;
            newObj.transform.position = transform.position;
            newObj.transform.rotation = transform.rotation;
            newObj.transform.localScale = transform.localScale;

            newObj.transform.parent = transform;

            ItemComponent itemComponent = newObj.GetComponent<ItemComponent>();
            itemComponent.itemAsset = itemToSpawn;

            SpriteRenderer spriteRenderer = newObj.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = itemToSpawn.icon;

            spawnedItem = newObj;

            return newObj != null;
        }

        return false;
    }

    private void OnValidate()
    {
        previewRenderer = GetComponent<SpriteRenderer>();
        if (itemToSpawn && previewRenderer)
        {
            previewRenderer.sprite = itemToSpawn.icon;
        }
    }
}
