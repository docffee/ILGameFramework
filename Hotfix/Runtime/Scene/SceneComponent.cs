using GameFramework.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hotfix.Runtime
{
    [GfComponent]
    public class SceneComponent:GameFrameworkComponent
    {
        private ISceneManager m_SceneManager = null;
        private EventComponent m_EventComponent = null;
        private Camera m_MainCamera = null;
        private Scene m_GameFrameworkScene = default(Scene);
        
        private bool m_EnableLoadSceneSuccessEvent = true;
        
        private bool m_EnableLoadSceneFailureEvent = true;
        
        private bool m_EnableLoadSceneUpdateEvent = true;
        
        private bool m_EnableLoadSceneDependencyAssetEvent = true;
        
        private bool m_EnableUnloadSceneSuccessEvent = true;
        
        private bool m_EnableUnloadSceneFailureEvent = true;
    }
}