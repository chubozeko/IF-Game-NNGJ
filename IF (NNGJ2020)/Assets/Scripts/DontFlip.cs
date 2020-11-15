using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontFlip : MonoBehaviour
{
    private Transform tf;

    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*if (tf.parent.localScale.x < 0 && tf.localScale.x > 0) 
            tf.localScale = new Vector3(-tf.localScale.x, tf.localScale.y, tf.localScale.z);
        else if (tf.parent.localScale.x > 0 && tf.localScale.x < 0)
            tf.localScale = new Vector3(-tf.localScale.x, tf.localScale.y, tf.localScale.z);*/

        if (Mathf.Sign(tf.parent.localScale.x) != Mathf.Sign(tf.localScale.x)) {
            tf.localScale = new Vector3(-tf.localScale.x, tf.localScale.y, tf.localScale.z);
        }
    }
}
