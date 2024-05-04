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

namespace RhubarbGeekNz.Joinery
{
    [TestClass]
    public class TestJoinDictionary
    {
        readonly InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
        public TestJoinDictionary()
        {
            foreach (Type t in new Type[] {
                typeof(JoinDictionary)
            })
            {
                CmdletAttribute ca = t.GetCustomAttribute<CmdletAttribute>();

                if (ca == null) throw new NullReferenceException();

                initialSessionState.Commands.Add(new SessionStateCmdletEntry($"{ca.VerbName}-{ca.NounName}", t, ca.HelpUri));
            }

            initialSessionState.Variables.Add(new SessionStateVariableEntry("ErrorActionPreference", ActionPreference.Stop, "Stop action"));
        }

        [TestMethod]
        public void TestDictionaryEntry()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript(
                    "$de = New-Object System.Collections.DictionaryEntry" + Environment.NewLine +
                    "$de.Name = 'foo'" + Environment.NewLine +
                    "$de.Value = 'bar'" + Environment.NewLine +
                    "$de | Join-Dictionary"
                    );

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                IDictionary dict = (IDictionary)outputPipeline[0].BaseObject;
                Assert.AreEqual(1, dict.Count);
                Assert.AreEqual("bar", dict["foo"]);
            }
        }

        [TestMethod]
        public void TestPSNoteProperty()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript(
                    "$de = New-Object System.Management.Automation.PSNoteProperty -ArgumentList ('foo','bar')" + Environment.NewLine +
                    "$de | Join-Dictionary"
                    );

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                IDictionary dict = (IDictionary)outputPipeline[0].BaseObject;
                Assert.AreEqual(1, dict.Count);
                Assert.AreEqual("bar", dict["foo"]);
            }
        }

        [TestMethod]
        public void TestHashtable()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript(
                    "$de = @{ foo = 'bar' }" + Environment.NewLine +
                    "$de | Join-Dictionary"
                    );

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                IDictionary dict = (IDictionary)outputPipeline[0].BaseObject;
                Assert.AreEqual(1, dict.Count);
                Assert.AreEqual("bar", dict["foo"]);
            }
        }

        [TestMethod]
        public void TestPSObject()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript(
                    "$de = [psobject]@{ foo = 'bar' }" + Environment.NewLine +
                    "$de | Join-Dictionary"
                    );

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                IDictionary dict = (IDictionary)outputPipeline[0].BaseObject;
                Assert.AreEqual(1, dict.Count);
                Assert.AreEqual("bar", dict["foo"]);
            }
        }

        [TestMethod]
        public void TestPSCustomObject()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript(
                    "$de = New-Object PSObject -Property @{ foo = 'bar' }" + Environment.NewLine +
                    "$de | Join-Dictionary"
                    );

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                IDictionary dict = (IDictionary)outputPipeline[0].BaseObject;
                Assert.AreEqual(1,dict.Count);
                Assert.AreEqual("bar", dict["foo"]);
            }
        }

        [TestMethod]
        public void TestVersionTable()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("$PSVersionTable | Join-Dictionary");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                IDictionary dict = (IDictionary)outputPipeline[0].BaseObject;
                Assert.IsNotNull(dict);
#if NETCOREAPP
                Assert.AreEqual("Core", dict["PSEdition"]);
#else
                Assert.AreEqual("Desktop", dict["PSEdition"]);
#endif
            }
        }

        [TestMethod]
        public void TestString()
        {
            bool wasCaught = false;
            string reason = null;

            try
            {
                using (PowerShell powerShell = PowerShell.Create(initialSessionState))
                {
                    powerShell.AddScript("'foo' | Join-Dictionary");

                    powerShell.Invoke();
                }
            }
            catch (ActionPreferenceStopException ex)
            {
                reason = ex.ErrorRecord.Exception.GetType().Name;
                wasCaught = ex.ErrorRecord.Exception is ParameterBindingException;
            }

            Assert.IsTrue(wasCaught, reason);
        }

        [TestMethod]
        public void TestNull()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("$null | Join-Dictionary");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                IDictionary dict = (IDictionary)outputPipeline[0].BaseObject;
                Assert.AreEqual(0, dict.Count);
            }
        }

        [TestMethod]
        public void TestPassThru()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                IDictionary dict2 = new Dictionary<string,string>();
                powerShell
                    .AddCommand("Join-Dictionary")
                    .AddParameter("PassThru")
                    .AddParameter("InputObject", new PSNoteProperty("foo","bar"))
                    .AddParameter("Dictionary",dict2);

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                IDictionary dict = (IDictionary)outputPipeline[0].BaseObject;
                Assert.AreSame(dict2,dict);
                Assert.AreEqual(1, dict.Count);
                Assert.AreEqual("bar", dict["foo"]);
            }
        }

        [TestMethod]
        public void TestNoPassThru()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                IDictionary dict = new Dictionary<string, string>();
                powerShell
                    .AddCommand("Join-Dictionary")
                    .AddParameter("InputObject", new PSNoteProperty("foo", "bar"))
                    .AddParameter("Dictionary", dict);

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(0, outputPipeline.Count);
                Assert.AreEqual(1, dict.Count);
                Assert.AreEqual("bar", dict["foo"]);
            }
        }
    }
}
