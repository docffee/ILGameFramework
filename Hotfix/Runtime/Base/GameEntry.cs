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
        private const string UnityGameFrameworkVersion = "3.0.7";
        private static readonly Dictionary<string, GameFrameworkComponent> _allGFComponents =
            new Dictionary<string, GameFrameworkComponent>();

        /// <summary>
        /// 热更新的所有类型
        /// </summary>
        public static Type[] HotfixTypes
        {
            get;
            private set;
        }

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
            string fullName = type.FullName;
            if (_allGFComponents.ContainsKey(fullName))
                return _allGFComponents[fullName];
            return null;
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <param name="typeName">要获取的游戏框架组件类型名称。</param>
        /// <returns>要获取的游戏框架组件。</returns>
        public static GameFrameworkComponent GetComponent(string typeName)
        {
            string fullName = typeName;
            //Type type = _assembly.GetType(typeName);
            //if (type != null)
            //    fullName = type.FullName;
            if (_allGFComponents.ContainsKey(fullName))
                return _allGFComponents[fullName];
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
        public static void RegisterComponent(List<Type> listTypes)
        {
            HotfixTypes = listTypes.ToArray();
            Utility.Assembly.AddTypes(listTypes);

            foreach (var item in listTypes)
            {
                if (item.IsAbstract)
                    continue;
                object[] objs = item.GetCustomAttributes(typeof(GfComponentAttribute),false);
                if (objs.Length == 0)
                    continue;
                
                object obj = Activator.CreateInstance(item);
                if (!(obj is GameFrameworkComponent gfComponent))
                    Debug.Log("Game Framework component is invalid." + item);
                else
                {
                    string fullName = item.FullName;
                    if (!string.IsNullOrEmpty(fullName) && !_allGFComponents.ContainsKey(fullName))
                        _allGFComponents.Add(fullName, gfComponent);
                    else
                        Debug.Log("添加失败,已有类型：" + item.FullName);
                }
             
            }
            Debug.Log("GFComponent count:" + _allGFComponents.Count);
        }
    }
}