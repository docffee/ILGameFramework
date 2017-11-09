using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using ILRuntime.CLR.Method;

public class IDisposableAdaptor : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(IDisposable);
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);//创建一个新的实例
    }

    internal class Adaptor : IDisposable, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor()
        { }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }
        public ILTypeInstance ILInstance
        {
            get
            {
                return instance;
            }
        }
        
        IMethod _mDispose;
        bool _isDispose = false;
        
        public void Dispose()
        {
            if (!_isDispose)
            {
                _mDispose = instance.Type.GetMethod("Dispose", 0);
                _isDispose = true;
            }
            if (_mDispose != null)
            {
                appdomain.Invoke(this._mDispose, instance,null);
            }
        }
        
    }
}
