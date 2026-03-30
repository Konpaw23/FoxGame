using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 5f;
    [Range(0.01f, 20.0f)][SerializeField] private float moveRange = 8.0f;

    private bool isMovingRight = false;
    private float startPositionX;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        startPositionX = this.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingRight)
        {
            if (this.transform.position.x < startPositionX + moveRange)
            {

                moveRight();
            }
            else
            {
                isMovingRight = false;
                moveLeft();
            }
        }
        else
        {
            if (this.transform.position.x > startPositionX - moveRange)
            {
                moveLeft();
            }
            else
            {
                isMovingRight = true;
                moveRight();
            }
        }
    }

    void moveRight()
    {
        transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }

    void moveLeft()
    {
        transform.Translate(moveSpeed * Time.deltaTime * -1, 0.0f, 0.0f, Space.World);
    }
}
