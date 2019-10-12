using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmButton : MonoBehaviour
{
    public void DestroyObject(GameObject obj)
    {
        if(obj != null)
        {
            Destroy(obj);
        }
    }
}
