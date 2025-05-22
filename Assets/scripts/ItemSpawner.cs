using System.Collections.Generic;
using UnityEngine;

public class ItemController: MonoBehaviour
{
    [System.Serializable]
    public struct Spawnable{
        public GameObject gameObject;
        public float weight;
    }
    public List<Spawnable> items = new List<Spawnable>();
    float totalWeight;
    void Awake()
    {
      totalWeight = 0;
        foreach (var item in items)
        {
            totalWeight += item.weight;
        }  
    }

    void Start()
    {
        float pick = UnityEngine.Random.value * totalWeight;	
        int chosenIndex = 0;
        float cumulatgiveWeight = items[0].weight;

        while (pick > cumulatgiveWeight && chosenIndex < items.Count - 1)
        {
            chosenIndex++;
            cumulatgiveWeight += items[chosenIndex].weight;
        }
        GameObject i = Instantiate(items[chosenIndex].gameObject, transform.position, Quaternion.identity) as GameObject;
    }
}