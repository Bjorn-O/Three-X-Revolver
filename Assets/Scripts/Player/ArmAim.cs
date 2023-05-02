using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class ArmAim : MonoBehaviour
{
    public void LookAtCrosshair(Vector3 pos)
    {
        var target = pos;
        var objectPos = transform.position;
        
        pos.z = 0;

        target.x -= objectPos.x;
        target.y -= objectPos.y;

        var angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(new Vector3(0,0,angle + 90));
    }
}
