using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    private float moveSpeed = 0.1f;
    private Vector2 position;
    private bool levitateUp = true;
    
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    void FixedUpdate()
    {
        if(transform.position.y < position.y - 0.4)
        {
            levitateUp = true;
        }
        else if(transform.position.y > position.y + 0.4)
        {
            levitateUp = false;
        }
        
        if(levitateUp)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        transform.Translate(0.0f, moveSpeed * Time.deltaTime, 0.0f, Space.World);
    }

    void MoveDown()
    {
        transform.Translate(0.0f, moveSpeed * Time.deltaTime * -1, 0.0f, Space.World);
    }

    public Vector2 GetPosition()
    {
        return position;
    }

    public void Activate()
    {
        spriteRenderer.sprite = activeSprite;
    }

    public void Deactivate()
    {
        spriteRenderer.sprite = inactiveSprite;
    }
}
