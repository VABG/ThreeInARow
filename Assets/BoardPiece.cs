using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPiece : MonoBehaviour
{
    public int status = 0;
    private float rotationTargetAngle = 0;
    bool isErroring = false;
    Rigidbody rb;
    float rotationSpeedMult = .2f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rotationSpeedMult = Random.Range(.05f, .3f);
        rb.angularDrag = Random.Range(4, 12);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isErroring)
        {
            // Do something
        }

        //Rotate towards target rotation always
        float diff = GetNearestAngle(rb.transform.rotation.eulerAngles.y, rotationTargetAngle);

        rb.AddTorque(Vector3.up * diff * rotationSpeedMult);
    }

    public void SetPiece(int status)
    {
        this.status = status;
        if (status == 1)
        {
            rotationTargetAngle = -90;
        }
        else if (status == 2)
        {
            rotationTargetAngle = 90;
        }
        else if(status == 0)
        {
            rotationTargetAngle = 0;
        }
        if (rotationTargetAngle < 0) rotationTargetAngle = 360 + rotationTargetAngle;
    }


    public float GetNearestAngle(float from, float to)
    {
        float diff = to - from;
        if (Mathf.Abs(diff) > 180)
        {
            if (diff < 0) diff += 360;
            else diff -= 360;
        }
        return diff;
    }
    public void DoError()
    {

    }


}
