using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ItemDataManager dataManager = ItemDataManager.GetItemDataManager();
        dataManager.saveFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
