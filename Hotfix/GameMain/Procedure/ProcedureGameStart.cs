using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using ProcedureOwner= GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using Hotfix.Runtime;

namespace Hotfix
{
    [GfProcedure(ProcedureType.Start)]
    public class ProcedureGameStart:ProcedureBase
    {
        #region 重写函数
        
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Debug.Log("ProcedureGameStart" + "------" + "OnEnter");
        }
       
        protected internal override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            Debug.Log("ProcedureGameStart" + "------" + "OnInit");
        }
     
        protected internal override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            Debug.Log("ProcedureGameStart" + "------" + "OnLeave");
        }
       
        protected internal override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            Debug.Log("ProcedureGameStart" + "------" + "OnUpdate");
        }
        #endregion
    }
}