using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] Item itemToSpawn;

    private void Start()
    {
        SpawnItem(itemToSpawn);
    }

    public bool SpawnItem(Item item)
    {
        if (item.prefab)
        {
            GameObject newObj = Instantiate(item.prefab);
            newObj.name = item.name;
            newObj.transform.position = transform.position;
            return newObj != null;
        }

        return false;
    }
}
