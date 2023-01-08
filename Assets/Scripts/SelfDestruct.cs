using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField]
    private float m_timeUntilDestruction;

    private float m_currentExistnce;

    private void Update()
    {
        m_currentExistnce += Time.deltaTime;
        if (m_currentExistnce > m_timeUntilDestruction)
        {
            SceneManager.LoadScene(0);
        }
    }
}
