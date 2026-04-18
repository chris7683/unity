using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    public SpikeGenerator spikeGenerator;
    public Color spikeColor;
    private SpriteRenderer sr;
    private bool passedPlayer = false;
    private Transform playerTransform;
    private playerscript player;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            player = playerObj.GetComponent<playerscript>();
        }
    }

    public void SetColor(Color color)
    {
        spikeColor = color;
        sr.color = color;
    }

    void Update()
    {
        if (spikeGenerator != null)
            transform.Translate(Vector2.left * spikeGenerator.currentSpeed * Time.deltaTime);

        if (!passedPlayer && playerTransform != null)
        {
            if (transform.position.x < playerTransform.position.x)
            {
                passedPlayer = true;

                if (player != null && !ColorsMatch(player.currentColor, spikeColor))
                    GameManager.instance.GameOver();
                else
                    GameManager.instance.AddScore(1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("nextline"))
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            GameManager.instance.GameOver();
    }

    bool ColorsMatch(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < 0.01f &&
               Mathf.Abs(a.g - b.g) < 0.01f &&
               Mathf.Abs(a.b - b.b) < 0.01f;
    }
}