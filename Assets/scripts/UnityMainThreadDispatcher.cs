using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    public static UnityMainThreadDispatcher instance;
    private static readonly Queue<Action> queue = new Queue<Action>();

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public static void Enqueue(Action action)
    {
        lock (queue) { queue.Enqueue(action); }
    }

    void Update()
    {
        lock (queue)
        {
            while (queue.Count > 0) queue.Dequeue()?.Invoke();
        }
    }
}