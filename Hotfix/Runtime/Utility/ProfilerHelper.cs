//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2017 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using System.Threading;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 性能分析辅助器。
    /// </summary>
    internal class ProfilerHelper : Utility.Profiler.IProfilerHelper
    {
        private Thread m_MainThread = null;
        
        public ProfilerHelper(Thread mainThread)
        {
            if (mainThread == null)
            {
                Log.Error("Main thread is invalid.");
                return;
            }

            m_MainThread = mainThread;
        }

        /// <summary>
        /// 开始采样。
        /// </summary>
        /// <param name="name">采样名称。</param>
        public void BeginSample(string name)
        {
            if (Thread.CurrentThread != m_MainThread)
            {
                return;
            }
            
            UnityEngine.Profiling.Profiler.BeginSample(name);
        }

        /// <summary>
        /// 结束采样。
        /// </summary>
        public void EndSample()
        {
            if (Thread.CurrentThread != m_MainThread)
            {
                return;
            }
            
            UnityEngine.Profiling.Profiler.EndSample();
        }
    }
}
