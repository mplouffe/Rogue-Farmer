using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialScreenInputListener : MonoBehaviour
{
    public InputAction m_exitAction;

    private void Awake()
    {
        m_exitAction.performed += OnExit;
    }

    private void OnEnable()
    {
        m_exitAction.Enable();
    }

    private void OnDisable()
    {
        m_exitAction.Disable();
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(0);
    }
}
