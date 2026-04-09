using UnityEngine;

public class SpikeGenerator : MonoBehaviour
{
    public GameObject spike;
    
    public float currentSpeed;
    public float MaxSpeed;
    public float MinSpeed;
    public float spawnInterval = 3f;
    
    private float timer;
    public Color[] spikeColors = new Color[] { Color.red, Color.blue };

    void Awake()
    {
        currentSpeed = MinSpeed;
        generateSpike();
        timer = spawnInterval;
    }

    public void generateSpike()
    {
        GameObject SpikeIns = Instantiate(spike, transform.position, transform.rotation);
        SpikeScript spikeScript = SpikeIns.GetComponent<SpikeScript>();
        spikeScript.spikeGenerator = this;

        Color randomColor = spikeColors[Random.Range(0, spikeColors.Length)];
        spikeScript.SetColor(randomColor);
    }

    public void NextLevel()
    {
        spawnInterval = Mathf.Max(0.8f, spawnInterval - 0.5f);
        currentSpeed = Mathf.Min(MaxSpeed, currentSpeed + 0.5f);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            generateSpike();
            timer = spawnInterval;
        }
    }
}