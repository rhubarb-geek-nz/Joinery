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
using System.Text;

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

                ArrayList list = (ArrayList)result;

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

                ArrayList list = (ArrayList)result;

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

        void AssertOutputType(object output)
        {
            OutputTypeAttribute ca = typeof(ConvertToList).GetCustomAttribute<OutputTypeAttribute>();

            foreach (var type in ca.Type)
            {
                Assert.IsInstanceOfType(output, type.Type);
            }

            Assert.IsInstanceOfType(output, typeof(ArrayList));
        }
    }
}
