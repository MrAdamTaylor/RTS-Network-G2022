using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
    private void Update()
    {
        if (isLocalPlayer)
        {
            
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");



            float high = 0;
            if (Input.GetKey(KeyCode.Space))
            {
                high = 1;
            }

            Vector3 movement = new Vector3(h * 0.25f, high * 0.25f, v * 0.25f);

            transform.position = transform.position + movement;
        }
    }
}
