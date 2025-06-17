using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapPlayer : MonoBehaviour
{
	public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void LateUpdate()
	{
		transform.rotation = Quaternion.Euler(0f, 0f, 90f-player.eulerAngles.y);
	}
}
