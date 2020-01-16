using Backtrace.Unity;
using RedRunner.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner
{
    public enum UIScreenInfo
    {
        LOADING_SCREEN,
        START_SCREEN,
        END_SCREEN,
        PAUSE_SCREEN,
        IN_GAME_SCREEN
    }

    public class UIManager : MonoBehaviour
    {

        private static UIManager m_Singleton;

        public static UIManager Singleton
        {
            get
            {
                return m_Singleton;
            }
        }

        [SerializeField]
        private List<UIScreen> m_Screens;
        private UIScreen m_ActiveScreen;
        public BacktraceClient BacktraceClient;
        private UIWindow m_ActiveWindow;
        [SerializeField]
        private readonly Texture2D m_CursorDefaultTexture;
        [SerializeField]
        private readonly Texture2D m_CursorClickTexture;
        [SerializeField]
        private readonly float m_CursorHideDelay = 1f;

        public List<UIScreen> UISCREENS
        {
            get
            {
                return m_Screens;
            }
        }

        public UIScreen GetUIScreen(UIScreenInfo screenInfo)
        {
            return m_Screens.Find(el => el.ScreenInfo == screenInfo);
        }

        private void Awake()
        {
            if (m_Singleton != null)
            {
                Destroy(gameObject);
                return;
            }
            BacktraceClient = GameObject.Find("Backtrace").GetComponent<BacktraceClient>();
            m_Singleton = this;
            Cursor.SetCursor(m_CursorDefaultTexture, Vector2.zero, CursorMode.Auto);
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            var loadingScreen = GetUIScreen(UIScreenInfo.LOADING_SCREEN);
            OpenScreen(loadingScreen);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                //Added enumeration to store screen info, aka type, so it will be easier to understand it
                var pauseScreen = GetUIScreen(UIScreenInfo.PAUSE_SCREEN);
                var ingameScreen = GetUIScreen(UIScreenInfo.IN_GAME_SCREEN);

                //If the pause screen is not open : open it otherwise close it
                if (!pauseScreen.IsOpen)
                {
                    if (m_ActiveScreen == ingameScreen)
                    {
                        if (IsAsScreenOpen())
                        {
                            CloseAllScreens();
                        }
                        try
                        {
                            OpenScreen(pauseScreen, true);
                        }
                        catch(Exception e)
                        {
                            BacktraceClient.Send(e);
                        }

                        GameManager.Singleton.StopGame();
                    }
                }
                else
                {
                    if (m_ActiveScreen == pauseScreen)
                    {
                        CloseScreen(pauseScreen);
                        OpenScreen(ingameScreen);
                        ////We are sure that we want to resume the game when we close a screen
                        GameManager.Singleton.ResumeGame();
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Cursor.SetCursor(m_CursorClickTexture, Vector2.zero, CursorMode.Auto);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Cursor.SetCursor(m_CursorDefaultTexture, Vector2.zero, CursorMode.Auto);
            }
        }

        public void OpenWindow(UIWindow window)
        {
            window.Open();
            m_ActiveWindow = window;
        }

        public void CloseWindow(UIWindow window)
        {
            if (m_ActiveWindow == window)
            {
                m_ActiveWindow = null;
            }
            window.Close();
        }

        public void CloseActiveWindow()
        {
            if (m_ActiveWindow != null)
            {
                CloseWindow(m_ActiveWindow);
            }
        }

        public void OpenScreen(UIScreen screen, bool validate = false)
        {
            CloseAllScreens();
            screen.UpdateScreenStatus(true);
            if (validate)
            {
                m_ActiveScreen = null;
                m_ActiveScreen.BroadcastMessage("invoke Store");
            }
            m_ActiveScreen = screen;
        }

        public void CloseScreen(UIScreen screen)
        {
            if (m_ActiveScreen == screen)
            {
                m_ActiveScreen = null;
            }
            screen.UpdateScreenStatus(false);
        }

        public void CloseAllScreens()
        {
            foreach (var screen in m_Screens)
            {
                CloseScreen(screen);
            }
        }

        private bool IsAsScreenOpen()
        {
            foreach (var screen in m_Screens)
            {
                if (screen.IsOpen)
                {
                    return true;
                }
            }

            return false;
        }
    }

}