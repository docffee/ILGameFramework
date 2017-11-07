using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;

namespace Hotfix.Runtime
{
    [GfComponent]
    public class ProcedureComponent:GameFrameworkComponent
    {
        private IProcedureManager m_ProcedureManager = null;
        private ProcedureBase m_EntranceProcedure = null;
        /// <summary>
        /// 获取当前流程。
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                return (ProcedureBase)m_ProcedureManager.CurrentProcedure;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ProcedureComponent()
        {
            m_ProcedureManager = GameFrameworkEntry.GetModule<IProcedureManager>();
            if (m_ProcedureManager == null)
            {
                Log.Fatal("Procedure manager is invalid.");
                return;
            }

            //初始化  所有的流程
            List<ProcedureBase> procedures = new List<ProcedureBase>();
            Type[] types = GameEntry.HotfixTypes;
            foreach (var item in types)
            {
                if (item.IsAbstract)
                    continue;
                object[] objs = item.GetCustomAttributes(typeof(GfProcedureAttribute), false);
                if (objs.Length == 0)
                    continue;
                Debug.Log("创建物体："+objs[0]+"----"+item.FullName);
                GfProcedureAttribute obj = (GfProcedureAttribute) objs[0];
                if (obj.ProceType == ProcedureType.Ignore)
                    continue;
                ProcedureBase procedure = (ProcedureBase) Activator.CreateInstance(item);
                if (procedure != null)
                {
                    procedures.Add(procedure);
                    if (obj.ProceType == ProcedureType.Start)
                        m_EntranceProcedure = procedure;
                }
                else
                    Log.Error("流程没有继承:ProcedureBase----" + item);
            }
            Debug.Log("初始流程Type：" + m_EntranceProcedure.GetType().FullName);
            m_ProcedureManager.Initialize(GameFrameworkEntry.GetModule<IFsmManager>(), procedures.ToArray());
           // 开始流程
               m_ProcedureManager.StartProcedure(m_EntranceProcedure.GetType());

            //   WaitToRunProcedure();
        }

        async void WaitToRunProcedure()
        {
            //延迟300毫秒执行
            await Task.Run(() =>
                {
                    Thread.Sleep(300);
                }
            );
        
        }

    }
}