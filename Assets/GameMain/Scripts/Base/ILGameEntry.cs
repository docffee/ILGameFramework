using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GameFramework;
using ILFramework;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

public class ILGameEntry : MonoBehaviour {

    #region AppDomain接口
    public AppDomain AppDomain
    {
        private set;
        get;
    }
    #endregion

    #region 属性
    [SerializeField]
    private TextAsset _dllAsset;
    [SerializeField]
    private TextAsset _pdbAsset;
    #endregion
    
    // Use this for initialization
    void Start () {
        AppDomain = new AppDomain();
#if UNITY_EDITOR
       OnHotFixLoaded(_dllAsset.bytes, _pdbAsset.bytes);
        LoadHotfixEntry();
#endif
    }

    // Update is called once per frame
    void Update () {
	    GameFrameworkEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
    }

    void OnDestroy()
    {
        GameFrameworkEntry.Shutdown();
    }

    void LoadHotfixEntry()
    {
        Type[] types = GetHotfixTypes();
        List<Type> _listType = new List<Type>();
        foreach (var item in types)
            _listType.Add(item);
        AppDomain.Invoke("Hotfix.GameEntry", "RegisterComponent", null, _listType);
    }

    #region 加载热更新
    void OnHotFixLoaded(byte[] dllBytes, byte[] pdbBytes)
    {
        if (pdbBytes == null)
        {
            using (System.IO.MemoryStream fs = new MemoryStream(dllBytes))
            {
                AppDomain.LoadAssembly(fs, null, new Mono.Cecil.Pdb.PdbReaderProvider());
            }
        }
        else
        {
            using (System.IO.MemoryStream fs = new MemoryStream(dllBytes))
            {
                using (System.IO.MemoryStream p = new MemoryStream(pdbBytes))
                {
                    AppDomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
                }
            }
        }
        
        InitializeILRuntime();
    }

    void InitializeILRuntime()
    {
        RegisterDelegate();
        RegisterAdaptor();
    }

    //注册委托
    private void RegisterDelegate()
    {
        AppDomain.DelegateManager.RegisterMethodDelegate<System.Object, GameFramework.Event.GameEventArgs>();
        AppDomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<GameFramework.Event.GameEventArgs>>((act) =>
        {
            return new System.EventHandler<GameFramework.Event.GameEventArgs>((sender, e) =>
            {
                ((System.Action<System.Object, GameFramework.Event.GameEventArgs>)act)(sender, e);
            });
        });

    }
    //注册继承适配器
    private void RegisterAdaptor()
    {
        //这里做一些ILRuntime的注册，HelloWorld示例暂时没有需要注册的
        AppDomain.RegisterCrossBindingAdaptor(new ProcedureBaseAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new UIFormLogicAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new IDataRowAdaptor());
    }

    /// <summary>
    /// 获取热更新的所有types
    /// </summary>
    /// <returns></returns>
    private Type[] GetHotfixTypes()
    {
        if (AppDomain == null)
            return new Type[0];
        return AppDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToArray();
    }

    #endregion

}
