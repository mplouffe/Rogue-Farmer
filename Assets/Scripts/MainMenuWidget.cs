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

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var moveValue = context.action.ReadValue<Vector2>();
            if (moveValue.y > 0 && m_mainMenuState != MainMenuState.Start)
            {
                m_mainMenuState = MainMenuState.Start;
                m_mainMenuCursor.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, m_startYValue, m_mainMenuCursor.sizeDelta.x);
            }
            else if (moveValue.y < 0 && m_mainMenuState != MainMenuState.Quit)
            {
                m_mainMenuState = MainMenuState.Quit;
                m_mainMenuCursor.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, m_quitYValue, m_mainMenuCursor.sizeDelta.x);
            }
        }
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch(m_mainMenuState)
            {
                case MainMenuState.Start:
                    SceneManager.LoadScene(1);
                    break;
                case MainMenuState.Quit:
                    Application.Quit();
                    break;
            }
        }
    }
}

public enum MainMenuState
{
    Start,
    Quit
}
