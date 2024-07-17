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
            string[] input = { "foo" };

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                PSDataCollection<object> inputPipeline = new PSDataCollection<object>();

                foreach (var i in input)
                {
                    inputPipeline.Add(i);
                }

                powerShell.AddCommand("ConvertTo-List").AddParameter("BaseObject");

                PSDataCollection<object> outputPipeline = powerShell.Invoke(inputPipeline);

                Assert.AreEqual(1, outputPipeline.Count);

                object result = outputPipeline[0];

                Assert.IsInstanceOfType(result, typeof(IList));

                IList list = (IList)result;

                Assert.AreEqual(1, list.Count);

                Assert.AreEqual(input[0], list[0].ToString());
            }
        }

        [TestMethod]
        public void TestObject()
        {
            string[] input = { "bar" };

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                PSDataCollection<object> inputPipeline = new PSDataCollection<object>();

                foreach (var i in input)
                {
                    inputPipeline.Add(i);
                }

                powerShell.AddCommand("ConvertTo-List");

                PSDataCollection<object> outputPipeline = powerShell.Invoke(inputPipeline);

                Assert.AreEqual(1, outputPipeline.Count);

                object result = outputPipeline[0];

                Assert.IsInstanceOfType(result, typeof(IList));

                IList list = (IList)result;

                Assert.AreEqual(1, list.Count);

                Assert.AreEqual(input[0], list[0].ToString());
            }
        }
    }
}
