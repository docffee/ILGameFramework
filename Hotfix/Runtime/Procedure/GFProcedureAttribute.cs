using System;
namespace Hotfix.Runtime
{
    [AttributeUsage( AttributeTargets.Class)]
    public class GfProcedureAttribute:Attribute
    {
        public readonly ProcedureType ProceType;

        public GfProcedureAttribute()
        {
            ProceType = ProcedureType.Normal;
        }

        public GfProcedureAttribute(ProcedureType procedureType)
        {
            ProceType = procedureType;
        }
    }
}