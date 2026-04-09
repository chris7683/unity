using UnityEngine;
using UnityEngine.EventSystems;

public class playerscript : MonoBehaviour
{
    public float Jumpforce;
    public bool isGrounded = false;
    Rigidbody2D RB;
    SpriteRenderer sr;

    public Color currentColor = Color.red;
    private Color[] colors = new Color[] { Color.red, Color.blue };
    private int colorIndex = 0;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.color = currentColor;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    Jump();
                }
            }
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            RB.AddForce(Vector2.up * Jumpforce);
            isGrounded = false;
        }
    }

    public void ChangeColor()
    {
        colorIndex = (colorIndex + 1) % colors.Length;
        currentColor = colors[colorIndex];
        sr.color = currentColor;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // Spike collision is fully handled by SpikeScript — nothing needed here
    }
}