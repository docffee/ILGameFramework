using System;
using System.Threading;
using GameFramework;
using GameFramework.Localization;
using GameFramework.Resource;
using UnityEngine;

namespace Hotfix
{
    public class BaseComponent:GameFrameworkComponent
    {
        private const int DefaultDpi = 96;  // default windows dpi

        private string m_GameVersion = string.Empty;
        private int m_InternalApplicationVersion = 0;
        private float m_GameSpeedBeforePause = 1f;
        
        private bool m_EditorResourceMode = true;
        
        private Language m_EditorLanguage = Language.Unspecified;
        

        private string m_LogHelperTypeName= "UnityGameFramework.Runtime.LogHelper";
        
        private string m_ZipHelperTypeName = "UnityGameFramework.Runtime.ZipHelper";
        
        private string m_JsonHelperTypeName = "UnityGameFramework.Runtime.JsonHelper";
        
        private string m_ProfilerHelperTypeName = "UnityGameFramework.Runtime.ProfilerHelper";
        
        private int m_FrameRate = 30;
        
        private float m_GameSpeed = 1f;
        
        private bool m_RunInBackground = true;
        
        private bool m_NeverSleep = true;

        /// <summary>
        /// 获取或设置游戏版本号。
        /// </summary>
        public string GameVersion
        {
            get
            {
                return m_GameVersion;
            }
            set
            {
                m_GameVersion = value;
            }
        }

        /// <summary>
        /// 获取或设置应用程序内部版本号。
        /// </summary>
        public int InternalApplicationVersion
        {
            get
            {
                return m_InternalApplicationVersion;
            }
            set
            {
                m_InternalApplicationVersion = value;
            }
        }

        /// <summary>
        /// 获取或设置是否使用编辑器资源模式（仅编辑器内有效）。
        /// </summary>
        public bool EditorResourceMode
        {
            get
            {
                return m_EditorResourceMode;
            }
            set
            {
                m_EditorResourceMode = value;
            }
        }

        /// <summary>
        /// 获取或设置编辑器语言（仅编辑器内有效）。
        /// </summary>
        public Language EditorLanguage
        {
            get
            {
                return m_EditorLanguage;
            }
            set
            {
                m_EditorLanguage = value;
            }
        }

        /// <summary>
        /// 获取或设置编辑器资源辅助器。
        /// </summary>
        public IResourceManager EditorResourceHelper
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置游戏帧率。
        /// </summary>
        public int FrameRate
        {
            get
            {
                return m_FrameRate;
            }
            set
            {
                Application.targetFrameRate = m_FrameRate = value;
            }
        }

        /// <summary>
        /// 获取或设置游戏速度。
        /// </summary>
        public float GameSpeed
        {
            get
            {
                return m_GameSpeed;
            }
            set
            {
                Time.timeScale = m_GameSpeed = (value >= 0f ? value : 0f);
            }
        }

        /// <summary>
        /// 获取游戏是否暂停。
        /// </summary>
        public bool IsGamePaused
        {
            get
            {
                return m_GameSpeed <= 0f;
            }
        }

        /// <summary>
        /// 获取是否正常游戏速度。
        /// </summary>
        public bool IsNormalGameSpeed
        {
            get
            {
                return m_GameSpeed == 1f;
            }
        }

        /// <summary>
        /// 获取或设置是否允许后台运行。
        /// </summary>
        public bool RunInBackground
        {
            get
            {
                return m_RunInBackground;
            }
            set
            {
                Application.runInBackground = m_RunInBackground = value;
            }
        }

        /// <summary>
        /// 获取或设置是否禁止休眠。
        /// </summary>
        public bool NeverSleep
        {
            get
            {
                return m_NeverSleep;
            }
            set
            {
                m_NeverSleep = value;
                Screen.sleepTimeout = value ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            }
        }

        /// <summary>
        /// 游戏框架组件初始化。--- 默认使用unity2017及以上版本
        /// </summary>
        public BaseComponent()
        {
            InitLogHelper();
            Log.Info("Game Framework version is {0}. Unity Game Framework version is {1}.", GameFrameworkEntry.Version, GameEntry.Version);
            
            InitZipHelper();
            InitJsonHelper();
            InitProfilerHelper();

            Utility.Converter.ScreenDpi = Screen.dpi;
            if (Utility.Converter.ScreenDpi <= 0)
            {
                Utility.Converter.ScreenDpi = DefaultDpi;
            }

            m_EditorResourceMode &= Application.isEditor;
            if (m_EditorResourceMode)
            {
                Log.Info("During this run, Game Framework will use editor resource files, which you should validate first.");
            }

            Application.targetFrameRate = m_FrameRate;
            Time.timeScale = m_GameSpeed;
            Application.runInBackground = m_RunInBackground;
            Screen.sleepTimeout = m_NeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            Application.lowMemory += OnLowMemory;
        }

        //***** 由游戏内调用Update
        private void Update()
        {
            GameFrameworkEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }
        

        /// <summary>
        /// 暂停游戏。
        /// </summary>
        public void PauseGame()
        {
            if (IsGamePaused)
            {
                return;
            }

            m_GameSpeedBeforePause = GameSpeed;
            GameSpeed = 0f;
        }

        /// <summary>
        /// 恢复游戏。
        /// </summary>
        public void ResumeGame()
        {
            if (!IsGamePaused)
            {
                return;
            }

            GameSpeed = m_GameSpeedBeforePause;
        }

        /// <summary>
        /// 重置为正常游戏速度。
        /// </summary>
        public void ResetNormalGameSpeed()
        {
            if (IsNormalGameSpeed)
            {
                return;
            }

            GameSpeed = 1f;
        }

        //销毁自己
        internal void Shutdown()
        {
            Application.lowMemory -= OnLowMemory;
            Dispose();
        }

        private void InitLogHelper()
        {
            if (string.IsNullOrEmpty(m_LogHelperTypeName))
            {
                return;
            }

            Type logHelperType = Utility.Assembly.GetTypeWithinLoadedAssemblies(m_LogHelperTypeName);
            if (logHelperType == null)
            {
                throw new GameFrameworkException(string.Format("Can not find log helper type '{0}'.", m_LogHelperTypeName));
            }

            Log.ILogHelper logHelper = (Log.ILogHelper)Activator.CreateInstance(logHelperType);
            if (logHelper == null)
            {
                throw new GameFrameworkException(string.Format("Can not create log helper instance '{0}'.", m_LogHelperTypeName));
            }

            Log.SetLogHelper(logHelper);
        }

        private void InitZipHelper()
        {
            if (string.IsNullOrEmpty(m_ZipHelperTypeName))
            {
                return;
            }

            Type zipHelperType = Utility.Assembly.GetTypeWithinLoadedAssemblies(m_ZipHelperTypeName);
            if (zipHelperType == null)
            {
                Log.Error("Can not find Zip helper type '{0}'.", m_ZipHelperTypeName);
                return;
            }

            Utility.Zip.IZipHelper zipHelper = (Utility.Zip.IZipHelper)Activator.CreateInstance(zipHelperType);
            if (zipHelper == null)
            {
                Log.Error("Can not create Zip helper instance '{0}'.", m_ZipHelperTypeName);
                return;
            }

            Utility.Zip.SetZipHelper(zipHelper);
        }

        private void InitJsonHelper()
        {
            if (string.IsNullOrEmpty(m_JsonHelperTypeName))
            {
                return;
            }

            Type jsonHelperType = Utility.Assembly.GetTypeWithinLoadedAssemblies(m_JsonHelperTypeName);
            if (jsonHelperType == null)
            {
                Log.Error("Can not find JSON helper type '{0}'.", m_JsonHelperTypeName);
                return;
            }

            Utility.Json.IJsonHelper jsonHelper = (Utility.Json.IJsonHelper)Activator.CreateInstance(jsonHelperType);
            if (jsonHelper == null)
            {
                Log.Error("Can not create JSON helper instance '{0}'.", m_JsonHelperTypeName);
                return;
            }

            Utility.Json.SetJsonHelper(jsonHelper);
        }

        private void InitProfilerHelper()
        {
            if (string.IsNullOrEmpty(m_ProfilerHelperTypeName))
            {
                return;
            }

            Type profilerHelperType = Utility.Assembly.GetTypeWithinLoadedAssemblies(m_ProfilerHelperTypeName);
            if (profilerHelperType == null)
            {
                Log.Error("Can not find profiler helper type '{0}'.", m_ProfilerHelperTypeName);
                return;
            }

            Utility.Profiler.IProfilerHelper profilerHelper = (Utility.Profiler.IProfilerHelper)Activator.CreateInstance(profilerHelperType, Thread.CurrentThread);
            if (profilerHelper == null)
            {
                Log.Error("Can not create profiler helper instance '{0}'.", m_ProfilerHelperTypeName);
                return;
            }

            Utility.Profiler.SetProfilerHelper(profilerHelper);
        }

        private void OnLowMemory()
        {
            Log.Info("Low memory reported...");

            //--------------暂时未实现ObjectPoolComponent 与 ResourceComponent


            //ObjectPoolComponent objectPoolComponent = GameEntry.GetComponent<ObjectPoolComponent>();
            //if (objectPoolComponent != null)
            //{
            //    objectPoolComponent.ReleaseAllUnused();
            //}

            //ResourceComponent resourceCompoent = GameEntry.GetComponent<ResourceComponent>();
            //if (resourceCompoent != null)
            //{
            //    resourceCompoent.ForceUnloadUnusedAssets(true);
            //}
        }
    }
}