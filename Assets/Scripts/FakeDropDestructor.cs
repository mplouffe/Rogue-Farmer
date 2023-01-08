using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeDropDestructor : MonoBehaviour
{
    [SerializeField]
    private float m_dropDuration;

    private float m_currentDropDuration = 0;

    private void Update()
    {
        m_currentDropDuration += Time.deltaTime;
        if (m_currentDropDuration > m_dropDuration)
        {
            Destroy(gameObject);
        }
    }
}
