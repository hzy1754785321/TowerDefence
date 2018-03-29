using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirEnemy : Enemy
{
    public float flyspeed = 0;
    void Update()
    {
        RotateTo();
        MoveTo();
        Fly();
    }
    public void Fly()
    {
        flyspeed = 0;
        if (this.transform.position.y < 2.00)
            flyspeed = 1.0f;
        this.transform.Translate(new Vector3(0, flyspeed * Time.deltaTime, 0));
    }
}
