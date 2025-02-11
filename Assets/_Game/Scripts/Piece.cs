using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    private void Start()
    {
      //  Invoke(nameof(BreakingPoint), 2f);
    }

    private void BreakingPoint()
    {
        gameObject.GetComponent<HingeJoint>().breakForce = 5400;
        gameObject.GetComponent<HingeJoint>().breakTorque = 5200;
    }

    private void Update()
    {
        if(gameObject.GetComponent<HingeJoint>() == null)
         {
            Debug.Log("Lose1");

        }
    }
}
