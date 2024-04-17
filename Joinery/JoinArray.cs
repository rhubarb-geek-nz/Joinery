// Copyright (c) 2024 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace RhubarbGeekNz.Joinery
{
    internal interface IAppender : IDisposable
    {
        void ProcessRecord(IEnumerable record);
        IEnumerable EndProcessing();
    }

    sealed internal class GenericAppender<T> : IAppender
    {
        readonly private List<T> list = new List<T>();

        public void ProcessRecord(IEnumerable record)
        {
            list.AddRange(record is IEnumerable<T> enumerable ? enumerable : record.Cast<T>());
        }

        public IEnumerable EndProcessing()
        {
            return list.ToArray();
        }

        public void Dispose()
        {
            list.Clear();
        }
    }

    sealed internal class ByteAppender : IAppender
    {
        private readonly MemoryStream stream = new MemoryStream();

        public void ProcessRecord(IEnumerable record)
        {
            byte[] ba = record is byte[] a ? a : record.Cast<byte>().ToArray();
            stream.Write(ba, 0, ba.Length);
        }

        public IEnumerable EndProcessing()
        {
            stream.Close();
            return stream.ToArray();
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }

    sealed internal class CharAppender : IAppender
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();

        public void ProcessRecord(IEnumerable record)
        {
            stringBuilder.Append(record is char[] ca ? ca : record.Cast<char>().ToArray());
        }

        public IEnumerable EndProcessing()
        {
            char[] ca=new char[stringBuilder.Length];
            if (ca.Length > 0)
            {
                stringBuilder.CopyTo(0, ca, 0, ca.Length);
            }
            return ca;
        }

        public void Dispose()
        {
            stringBuilder.Clear();
        }
    }

    [Cmdlet(VerbsCommon.Join, "Array")]
    [OutputType(typeof(IEnumerable))]
    sealed public class JoinArray : PSCmdlet, IDisposable
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Array to be joined"), AllowNull, AllowEmptyCollection]
        public IEnumerable InputObject;

        [Parameter(Mandatory = true, HelpMessage = "Type of array element")]
        public Type Type;

        private IAppender appender;

        readonly private static Dictionary<Type, Func<IAppender>> factories = new Dictionary<Type, Func<IAppender>>()
        {
            { typeof(byte),() => new ByteAppender() },
            { typeof(char),() => new CharAppender() }
        };

        protected override void BeginProcessing()
        {
            if (factories.TryGetValue(Type, out var factory))
            {
                appender = factory();
            }
            else
            {
                Type genericType = typeof(GenericAppender<>).MakeGenericType(new Type[] { Type });

                appender = (IAppender)Activator.CreateInstance(genericType);
            }
        }

        protected override void ProcessRecord()
        {
            if (InputObject != null)
            {
                try
                {
                    appender.ProcessRecord(InputObject);
                }
                catch (InvalidCastException ex)
                {
                    WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidArgument, null));
                }
            }
        }

        protected override void EndProcessing()
        {
            WriteObject(appender.EndProcessing());
        }

        public void Dispose()
        {
            var appender = this.appender;
            this.appender = null;

            if (appender != null)
            {
                appender.Dispose();
            }
        }
    }
}
