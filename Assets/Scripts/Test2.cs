using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    public static int a;
    void Start()
    {
        a = 1;
        Test(a );
        Debug.Log("≤‚ ‘" + a);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Test(int a )
    {
        a++;
    }
}
