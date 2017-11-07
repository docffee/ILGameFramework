using System;
namespace Hotfix
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