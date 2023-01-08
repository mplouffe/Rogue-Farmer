using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuWidget : MonoBehaviour
{
    private MainMenuState m_mainMenuState = MainMenuState.Start;

    [SerializeField]
    private RectTransform m_mainMenuCursor;

    [SerializeField]
    private int m_startYValue;

    [SerializeField]
    private int m_quitYValue;

    [SerializeField]
    private int m_infoYValue;

    private float m_actionCooldown;
    private float m_durationSinceLastAction;
    private bool m_canAct;

    private void Update()
    {
        if (!m_canAct)
        {
            m_durationSinceLastAction += Time.deltaTime;
            if (m_durationSinceLastAction > m_actionCooldown)
            {
                m_canAct = true;
            }
        }
    }

    private void TakeAction()
    {
        m_actionCooldown = 0.2f;
        m_durationSinceLastAction = 0;
        m_canAct = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed && m_canAct)
        {
            var moveValue = context.action.ReadValue<Vector2>();
            if (moveValue.y > 0)
            {
                switch (m_mainMenuState)
                {
                    case MainMenuState.Quit:
                        m_mainMenuState = MainMenuState.Info;
                        m_mainMenuCursor.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, m_infoYValue, m_mainMenuCursor.sizeDelta.x);
                        break;
                    case MainMenuState.Info:
                        m_mainMenuState = MainMenuState.Start;
                        m_mainMenuCursor.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, m_startYValue, m_mainMenuCursor.sizeDelta.x);
                        break;

                }
                TakeAction();
            }
            else if (moveValue.y < 0)
            {
                switch (m_mainMenuState)
                {
                    case MainMenuState.Start:
                        m_mainMenuState = MainMenuState.Info;
                        m_mainMenuCursor.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, m_infoYValue, m_mainMenuCursor.sizeDelta.x);
                        break;
                    case MainMenuState.Info:
                        m_mainMenuState = MainMenuState.Quit;
                        m_mainMenuCursor.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, m_quitYValue, m_mainMenuCursor.sizeDelta.x);
                        break;
                }
                TakeAction();
            }
        }
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (context.performed && m_canAct)
        {
            switch(m_mainMenuState)
            {
                case MainMenuState.Start:
                    SceneManager.LoadScene(1);
                    break;
                case MainMenuState.Quit:
                    Application.Quit();
                    break;
                case MainMenuState.Info:
                    SceneManager.LoadScene(3);
                    break;
            }
        }
    }
}

public enum MainMenuState
{
    Start,
    Quit,
    Info
}
