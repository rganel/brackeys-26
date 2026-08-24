using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-1000)]
public class TickManager : MonoBehaviour
{
    [SerializeField] private float TickUpdateInterval;
    [SerializeField] private int UpdatesPerTick;

    public UnityEvent<float> TickEvent;

    public static TickManager Instance { get; private set; }

    private float m_tick;
    private Coroutine m_tickCoroutine;

    private void Awake()
    {
        Debug.Assert(Instance == null);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        Debug.Assert(TickUpdateInterval > 0.0f);
        Debug.Assert(UpdatesPerTick > 0);
    }

    public void TickOnce()
    {
        m_tick = Mathf.Floor(m_tick + 1);
        TickEvent?.Invoke(m_tick);
    }

    public void BeginTravel()
    {
        if (m_tickCoroutine != null)
        {
            StopCoroutine(m_tickCoroutine);
        }

        m_tickCoroutine = StartCoroutine(TickHandler());
    }

    public void PauseTravel()
    {
        if (m_tickCoroutine != null)
        {
            StopCoroutine(m_tickCoroutine);
            m_tickCoroutine = null;
        }
    }

    private IEnumerator TickHandler()
    {
        while (true)
        {
            yield return new WaitForSeconds(TickUpdateInterval);

            m_tick += (1 / (float)UpdatesPerTick);

            TickEvent?.Invoke(m_tick);
        }
    }
}