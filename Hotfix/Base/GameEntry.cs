using GameFramework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hotfix
{
    public static class GameEntry
    {
        private const string UnityGameFrameworkVersion = "3.0.6";
        private static readonly Dictionary<int, GameFrameworkComponent> _allGFComponents =
            new Dictionary<int, GameFrameworkComponent>();

        /// <summary>
        /// 游戏框架所在的场景编号。
        /// </summary>
        internal const int GameFrameworkSceneId = 0;

        /// <summary>
        /// 获取 Unity 游戏框架版本号。
        /// </summary>
        public static string Version
        {
            get
            {
                return UnityGameFrameworkVersion;
            }
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架组件类型。</typeparam>
        /// <returns>要获取的游戏框架组件。</returns>
        public static T GetComponent<T>() where T : GameFrameworkComponent
        {
            return (T)GetComponent(typeof(T));
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <param name="type">要获取的游戏框架组件类型。</param>
        /// <returns>要获取的游戏框架组件。</returns>
        public static GameFrameworkComponent GetComponent(Type type)
        {
            int hashCode = type.GetHashCode();
            if (_allGFComponents.ContainsKey(hashCode))
                return _allGFComponents[hashCode];
            return null;
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <param name="typeName">要获取的游戏框架组件类型名称。</param>
        /// <returns>要获取的游戏框架组件。</returns>
        public static GameFrameworkComponent GetComponent(int typeHashCode)
        {
            if (_allGFComponents.ContainsKey(typeHashCode))
                return _allGFComponents[typeHashCode];
            return null;
        }
        

        /// <summary>
        /// 关闭游戏框架。
        /// </summary>
        /// <param name="shutdownType">关闭游戏框架类型。</param>
        public static void Shutdown(ShutdownType shutdownType)
        {
            Log.Info("Shutdown Game Framework ({0})...", shutdownType.ToString());
            BaseComponent baseComponent = GetComponent<BaseComponent>();
            if (baseComponent != null)
            {
                baseComponent.Shutdown();
                baseComponent = null;
            }

            _allGFComponents.Clear();

            if (shutdownType == ShutdownType.None)
            {
                return;
            }

            if (shutdownType == ShutdownType.Restart)
            {
                SceneManager.LoadScene(GameFrameworkSceneId);
                return;
            }

            if (shutdownType == ShutdownType.Quit)
            {
                Application.Quit();
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
                return;
            }
        }

        /// <summary>
        /// 注册游戏框架组件。
        /// </summary>
        internal static void RegisterComponent()
        {
            Assembly assembly = typeof(GameEntry).Assembly;
            Type[] types = assembly.GetTypes();
            foreach (var item in types)
            {
                GFComponentAttribute gFComponentAttribute = item.GetCustomAttribute<GFComponentAttribute>();
                if (gFComponentAttribute == null|| item.IsAbstract)
                    continue;
                object obj = Activator.CreateInstance(item);
                if (!(obj is GameFrameworkComponent gfComponent))
                    Log.Error("Game Framework component is invalid." + item);
                else
                {
                    int hashCode = item.GetHashCode();
                    if (!_allGFComponents.ContainsKey(hashCode))
                        _allGFComponents.Add(hashCode, gfComponent);
                }
            }
        }
    }
}