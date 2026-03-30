using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEagleController : EnemyController
{
    void OnMouseDown()
    {
        Kill();
    }
}
