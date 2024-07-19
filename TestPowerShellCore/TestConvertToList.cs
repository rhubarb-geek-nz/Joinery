// Copyright (c) 2024 Roger Brown.
// Licensed under the MIT License.

#if NETCOREAPP
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Threading.Tasks;

namespace RhubarbGeekNz.Joinery
{
    [TestClass]
    public class TestConvertToList
    {
        readonly InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
        public TestConvertToList()
        {
            foreach (Type t in new Type[] {
                typeof(ConvertToList)
            })
            {
                CmdletAttribute ca = t.GetCustomAttribute<CmdletAttribute>();

                if (ca == null) throw new NullReferenceException();

                initialSessionState.Commands.Add(new SessionStateCmdletEntry($"{ca.VerbName}-{ca.NounName}", t, ca.HelpUri));
            }

            initialSessionState.Variables.Add(new SessionStateVariableEntry("ErrorActionPreference", ActionPreference.Stop, "Stop action"));
        }

        [TestMethod]
        public async Task TestEmpty()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddCommand("ConvertTo-List").AddParameter("BaseObject");

                var inputPipeline = new PSDataCollection<PSObject>();
                var outputPipeline = new PSDataCollection<PSObject>();

                var task = Task.Factory.FromAsync(
                    powerShell.BeginInvoke(inputPipeline, outputPipeline),
                    t => powerShell.EndInvoke(t));

                inputPipeline.Complete();

                await task;

                Assert.AreEqual(1, outputPipeline.Count);

                object result = outputPipeline[0].BaseObject;

                AssertOutputType(result);

                IList list = (IList)result;

                Assert.AreEqual(0, list.Count);
            }
        }

        [TestMethod]
        public void TestBaseObject()
        {
            string[] input = { "foo", "bar" };

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                var inputPipeline = new PSDataCollection<object>();

                foreach (var i in input)
                {
                    inputPipeline.Add(i);
                }

                powerShell.AddCommand("ConvertTo-List").AddParameter("BaseObject");

                var outputPipeline = powerShell.Invoke(inputPipeline);

                Assert.AreEqual(1, outputPipeline.Count);

                object result = outputPipeline[0].BaseObject;

                AssertOutputType(result);

                IList list = (IList)result;

                Assert.AreEqual(input.Length, list.Count);

                for (int i = 0; i < input.Length; i++)
                {
                    var value = list[i];

                    Assert.AreEqual(input[i], value);
                }
            }
        }

        [TestMethod]
        public void TestPSObject()
        {
            string[] input = { "foo", "bar" };

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                var inputPipeline = new PSDataCollection<object>();

                foreach (var i in input)
                {
                    inputPipeline.Add(i);
                }

                powerShell.AddCommand("ConvertTo-List");

                var outputPipeline = powerShell.Invoke(inputPipeline);

                Assert.AreEqual(1, outputPipeline.Count);

                object result = outputPipeline[0].BaseObject;

                AssertOutputType(result);

                IList list = (IList)result;

                Assert.AreEqual(input.Length, list.Count);

                for (int i = 0; i < input.Length; i++)
                {
                    var value = list[i];

                    Assert.IsInstanceOfType(value, typeof(PSObject));

                    PSObject psobj = (PSObject)value;

                    Assert.AreEqual(input[i], psobj.BaseObject);
                }
            }
        }

        [TestMethod]
        public void TestString()
        {
            string[] input = { "foo", "bar" };

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                var inputPipeline = new PSDataCollection<object>();

                foreach (var i in input)
                {
                    inputPipeline.Add(i);
                }

                powerShell.AddCommand("ConvertTo-List").AddParameter("BaseObject").AddParameter("Type", typeof(string));

                var outputPipeline = powerShell.Invoke(inputPipeline);

                Assert.AreEqual(1, outputPipeline.Count);

                object result = outputPipeline[0].BaseObject;

                AssertOutputType(result, typeof(List<string>));

                IList<string> list = (IList<string>)result;

                Assert.AreEqual(input.Length, list.Count);

                for (int i = 0; i < input.Length; i++)
                {
                    var value = list[i];

                    Assert.AreEqual(input[i], value);
                }
            }
        }

        [TestMethod]
        public void TestGenericPSObject()
        {
            string[] input = { "foo", "bar" };

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                var inputPipeline = new PSDataCollection<object>();

                foreach (var i in input)
                {
                    inputPipeline.Add(i);
                }

                powerShell.AddCommand("ConvertTo-List").AddParameter("Type", typeof(PSObject));

                var outputPipeline = powerShell.Invoke(inputPipeline);

                Assert.AreEqual(1, outputPipeline.Count);

                object result = outputPipeline[0].BaseObject;

                AssertOutputType(result, typeof(List<PSObject>));

                IList<PSObject> list = (IList<PSObject>)result;

                Assert.AreEqual(input.Length, list.Count);

                for (int i = 0; i < input.Length; i++)
                {
                    PSObject psobj = list[i];

                    Assert.AreEqual(input[i], psobj.BaseObject);
                }
            }
        }

        [TestMethod]
        public void TestCastError()
        {
            bool wasCaught = false;
            string exName = null;

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddCommand("ConvertTo-List").AddParameter("BaseObject").AddParameter("Type", typeof(int));

                try
                {
                    string[] input = { "foo", "bar" };
                    powerShell.Invoke(input);
                }
                catch (ActionPreferenceStopException ex)
                {
                    wasCaught = true;
                    exName = ex.ErrorRecord.Exception.GetType().Name;
                }
            }

            Assert.IsTrue(wasCaught);
            Assert.AreEqual("ArgumentException", exName);
        }

        [TestMethod]
        public void TestListString()
        {
            string[] input = { "foo", "bar" };

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                var inputPipeline = new PSDataCollection<object>();

                foreach (var i in input)
                {
                    inputPipeline.Add(i);
                }

                IList list = new List<string>();

                powerShell.AddCommand("ConvertTo-List").AddParameter("BaseObject").AddParameter("List", list);

                var outputPipeline = powerShell.Invoke(inputPipeline);

                Assert.AreEqual(0, outputPipeline.Count);

                Assert.AreEqual(input.Length, list.Count);

                for (int i = 0; i < input.Length; i++)
                {
                    var value = list[i];

                    Assert.AreEqual(input[i], value);
                }
            }
        }

        [TestMethod]
        public void TestPSDataCollection()
        {
            string[] input = { "foo", "bar" };

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                var inputPipeline = new PSDataCollection<object>();

                foreach (var i in input)
                {
                    inputPipeline.Add(i);
                }

                var list = new PSDataCollection<PSObject>();

                powerShell.AddCommand("ConvertTo-List").AddParameter("List", list);

                var outputPipeline = powerShell.Invoke(inputPipeline);

                Assert.AreEqual(0, outputPipeline.Count);

                Assert.AreEqual(input.Length, list.Count);

                for (int i = 0; i < input.Length; i++)
                {
                    var value = list[i];

                    Assert.AreEqual(input[i], value.BaseObject);
                }
            }
        }

        [TestMethod]
        public void TestPassThru()
        {
            string[] input = { "foo", "bar" };

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                var inputPipeline = new PSDataCollection<object>();

                foreach (var i in input)
                {
                    inputPipeline.Add(i);
                }

                var list = new List<PSObject>();

                powerShell.AddCommand("ConvertTo-List").AddParameter("List", list).AddParameter("PassThru");

                var outputPipeline = powerShell.Invoke(inputPipeline);

                Assert.AreEqual(1, outputPipeline.Count);

                Assert.AreSame(list, outputPipeline[0].BaseObject);

                Assert.AreEqual(input.Length, list.Count);

                for (int i = 0; i < input.Length; i++)
                {
                    var value = list[i];

                    Assert.AreEqual(input[i], value.BaseObject);
                }
            }
        }

        [TestMethod]
        public void TestNullList()
        {
            bool wasCaught = false;
            string parameterName = null;

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddCommand("ConvertTo-List").AddParameter("List", null);

                try
                {
                    string[] input = { "foo", "bar" };
                    powerShell.Invoke(input);
                }
                catch (ParameterBindingException ex)
                {
                    wasCaught = true;
                    parameterName = ex.ParameterName;
                }
            }

            Assert.IsTrue(wasCaught);
            Assert.AreEqual("List", parameterName);
        }

        void AssertOutputType(object output, Type listType = null)
        {
            OutputTypeAttribute ca = typeof(ConvertToList).GetCustomAttribute<OutputTypeAttribute>();

            foreach (var type in ca.Type)
            {
                Assert.IsInstanceOfType(output, type.Type);
            }

            Assert.IsInstanceOfType(output, listType == null ? typeof(ArrayList) : listType);
        }
    }
}
