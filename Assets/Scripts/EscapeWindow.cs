using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EscapeWindow : MonoBehaviour
{
    public InputAction m_escapeAction;

    public InputAction m_closeEscape;

    public InputAction m_finishEscape;

    [SerializeField]
    private CanvasGroup m_escapeWindowCanvasGroup;

    private float m_cooldownDuration;
    private float m_totalCooldown;
    private bool m_canAct;

    private void Awake()
    {
        m_escapeAction.performed += OnEscapePerformed;
        m_closeEscape.performed += OnCloseEscapePerformed;
        m_finishEscape.performed += OnFinishEscapePerformed;
        m_closeEscape.Disable();
        m_finishEscape.Disable();
        m_escapeAction.Enable();
    }

    private void OnEnable()
    {
        m_escapeWindowCanvasGroup = GetComponentInChildren<CanvasGroup>();
        m_closeEscape.Disable();
        m_finishEscape.Disable();
        m_escapeAction.Enable();
    }

    private void OnDisable()
    {
        m_closeEscape.Disable();
        m_finishEscape.Disable();
        m_escapeAction.Disable();
    }

    private void Update()
    {
        if (!m_canAct)
        {
            m_totalCooldown += Time.deltaTime;
            if (m_totalCooldown > m_cooldownDuration)
            {
                m_canAct = true;
                m_totalCooldown = 0f;
            }
        }
    }

    public void OnEscapePerformed(InputAction.CallbackContext context)
    {
        if (m_canAct)
        {
            m_escapeAction.Disable();
            m_escapeWindowCanvasGroup.alpha = 1;
            m_closeEscape.Enable();
            m_finishEscape.Enable();
            TakeAction();
        }
    }

    public void OnCloseEscapePerformed(InputAction.CallbackContext context)
    {
        if (m_canAct)
        {
            m_escapeWindowCanvasGroup.alpha = 0;
            m_closeEscape.Disable();
            m_escapeAction.Enable();
            m_finishEscape.Disable();
            TakeAction();
        }
    }

    private void TakeAction()
    {
        m_canAct = false;
        m_cooldownDuration = 0.3f;
        m_totalCooldown = 0;
    }

    public void OnFinishEscapePerformed(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(0);
    }
}
