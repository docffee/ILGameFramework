using System;
using System.Collections;
using System.Runtime.Serialization;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using JetBrains.Annotations;


    public class ExceptionTestAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return typeof(Exception); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        private string _message;
        private Exception _innerException;
        private SerializationInfo _info;
        private StreamingContext _context;
        private int type = 0;

        public ExceptionTestAdaptor()
        {
            type = 0;
        }

        public ExceptionTestAdaptor(string message)
        {
            _message = message;
            type = 1;
        }

        public ExceptionTestAdaptor(string message, Exception innerException)
        {
            _message = message;
            _innerException = innerException;
            type = 2;
        }

        public ExceptionTestAdaptor([NotNull] SerializationInfo info, StreamingContext context)
        {
            _info = info;
            _context = context;
            type = 3;
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain,
            ILTypeInstance instance)
        {
            Adaptor ad = null;
            switch (type)
            {
                case 0:
                    ad = new Adaptor();
                    break;
                case 1:
                    ad = new Adaptor(_message);
                    break;
                case 2:
                    ad = new Adaptor(_message, _innerException);
                    break;
                case 3:
                    ad = new Adaptor(_info, _context);
                    break;
                default:
                    break;
            }
            ad.SetAdaptor(appdomain, instance);
            return ad; //创建一个新的实例
        }

        internal class Adaptor : Exception, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public void SetAdaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance
            {
                get { return instance; }
            }

            public Adaptor()
            {
            }

            public Adaptor(string message) : base(message)
            {
            }

            public Adaptor(string message, Exception innerException) : base(message, innerException)
            {
            }

            public Adaptor([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

        }
    }