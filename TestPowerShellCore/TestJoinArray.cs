// Copyright (c) 2024 Roger Brown.
// Licensed under the MIT License.

#if NETCOREAPP
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;

namespace RhubarbGeekNz.Joinery
{
    [TestClass]
    public class TestJoinArray
    {
        readonly InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
        public TestJoinArray()
        {
            foreach (Type t in new Type[] {
                typeof(JoinArray)
            })
            {
                CmdletAttribute ca = t.GetCustomAttribute<CmdletAttribute>();

                if (ca == null) throw new NullReferenceException();

                initialSessionState.Commands.Add(new SessionStateCmdletEntry($"{ca.VerbName}-{ca.NounName}", t, ca.HelpUri));
            }

            initialSessionState.Variables.Add(new SessionStateVariableEntry("ErrorActionPreference", ActionPreference.Stop, "Stop action"));
        }

        [TestMethod]
        public void TestByteArray()
        {
            byte[][] input =
            {
                Encoding.ASCII.GetBytes("Hello"),
                Encoding.ASCII.GetBytes(" "),
                Encoding.ASCII.GetBytes("World"),
            };

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                PSDataCollection<object> inputPipeline = new PSDataCollection<object>();

                foreach (var i in input)
                {
                    inputPipeline.Add(i);
                }

                powerShell.AddCommand("Join-Array").AddParameter("Type",typeof(byte));

                PSDataCollection<object> outputPipeline = powerShell.Invoke(inputPipeline);

                List<byte[]> result = new List<byte[]>();

                Assert.AreEqual(1, outputPipeline.Count);

                Append(result, outputPipeline);

                Assert.AreEqual(1, result.Count);

                Assert.AreEqual("Hello World", Encoding.ASCII.GetString(result[0]));
            }
        }

        [TestMethod]
        public void TestEmpty()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                PSDataCollection<object> inputPipeline = new PSDataCollection<object>();

                inputPipeline.Add(Array.Empty<byte>());

                powerShell.AddCommand("Join-Array").AddParameter("Type", typeof(byte));

                PSDataCollection<object> outputPipeline = powerShell.Invoke(inputPipeline);

                List<byte[]> result = new List<byte[]>();

                Assert.AreEqual(1, outputPipeline.Count);

                Append(result, outputPipeline);

                Assert.AreEqual(1, result.Count);

                Assert.AreEqual(0, result[0].Length);
            }
        }

        [TestMethod]
        public void TestNull()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                PSDataCollection<object> inputPipeline = new PSDataCollection<object>();

                powerShell.AddCommand("Join-Array").AddParameter("InputObject", null).AddParameter("Type", typeof(byte));

                PSDataCollection<object> outputPipeline = powerShell.Invoke();

                List<byte[]> result = new List<byte[]>();

                Assert.AreEqual(1, outputPipeline.Count);

                Append(result, outputPipeline);

                Assert.AreEqual(1, result.Count);

                Assert.AreEqual(0, result[0].Length);
            }
        }

        [TestMethod]
        public void TestArrayOfBytes()
        {
            byte[][] input =
            {
                Encoding.ASCII.GetBytes("Hello"),
                Encoding.ASCII.GetBytes(" "),
                Encoding.ASCII.GetBytes("World"),
            };

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                PSDataCollection<object> inputPipeline = new PSDataCollection<object>();

                foreach (var i in input)
                {
                    inputPipeline.Add(i);
                }

                powerShell.AddCommand("Join-Array").AddParameter("Type",typeof(byte));

                PSDataCollection<object> outputPipeline = powerShell.Invoke(inputPipeline);

                List<byte[]> result = new List<byte[]>();

                Assert.AreEqual(1, outputPipeline.Count);

                Append(result, outputPipeline);

                Assert.AreEqual(1, result.Count);

                Assert.AreEqual("Hello World", Encoding.ASCII.GetString(result[0]));
            }
        }

        [TestMethod]
        public void TestArrayScript()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Array -Type ([byte]) -InputObject ([System.Text.Encoding]::ASCII.GetBytes('Hello World'))");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);

                byte[] result = (byte[])outputPipeline[0].BaseObject;

                Assert.AreEqual("Hello World", Encoding.ASCII.GetString(result));
            }
        }

        [TestMethod]
        public void TestStringList()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Array -Type ([String]) -InputObject @('Hello', ' ', 'World')");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                string[] result = (string[])outputPipeline[0].BaseObject;
                Assert.AreEqual(3, result.Length);
                Assert.AreEqual("Hello", result[0]);
                Assert.AreEqual(" ", result[1]);
                Assert.AreEqual("World", result[2]);
            }
        }

        [TestMethod]
        public void TestCharList()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Array -Type ([Char]) -InputObject ('Hello'.ToCharArray())");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                char[] result = (char[])outputPipeline[0].BaseObject;
                Assert.AreEqual(5, result.Length);
                Assert.AreEqual("Hello", new String(result));
            }
        }


        [TestMethod]
        public void TestErrorList()
        {
            bool wasCaught = false;

            try
            {
                using (PowerShell powerShell = PowerShell.Create(initialSessionState))
                {
                    powerShell.AddScript("Join-Array -Type ([int64]) -InputObject @('Hello', ' ', 'World')");

                    var outputPipeline = powerShell.Invoke();

                    Assert.AreEqual(1, outputPipeline.Count);
                    string[] result = (string[])outputPipeline[0].BaseObject;
                    Assert.AreEqual(3, result.Length);
                    Assert.AreEqual("Hello", result[0]);
                    Assert.AreEqual(" ", result[1]);
                    Assert.AreEqual("World", result[2]);
                }
            }
            catch (ActionPreferenceStopException)
            {
                wasCaught = true;
            }

            Assert.IsTrue(wasCaught);
        }

        private void Append(List<byte[]> list, object o)
        {
            if (o is PSObject)
            {
                PSObject p = (PSObject)o;

                Append(list, p.BaseObject);
            }
            else
            {
                if (o is byte[])
                {
                    list.Add((byte[])o);
                }
                else
                {
                    IEnumerable<object> e = (IEnumerable<object>)o;

                    foreach (var i in e)
                    {
                        Append(list, i);
                    }
                }
            }
        }
    }
}
