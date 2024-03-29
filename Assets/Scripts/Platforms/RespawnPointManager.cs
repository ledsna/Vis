using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPointManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetRespawnPosition()
    {
        // Assuming the GameObject is a cube and its pivot is at its center
        Vector3 scale = gameObject.transform.localScale;
        float height = scale.y;
        Vector3 topCenter = gameObject.transform.position + new Vector3(0, height / 2, 0);
        topCenter.y += 1f;

        return topCenter;
    }
}
