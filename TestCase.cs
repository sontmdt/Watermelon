using System.Collections;
using UnityEngine;

public class TestCase : MonoBehaviour
{
    [SerializeField] private int testCount = 100;
    private void Start()
    {
        StartCoroutine(RunTest());
    }

    private IEnumerator RunTest()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < testCount; i++)
        {
            GameManager.Instance.m_levelController.m_playerController.SpawnBall();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
