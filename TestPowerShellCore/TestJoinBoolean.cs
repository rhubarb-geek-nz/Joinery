// Copyright (c) 2026 Roger Brown.
// Licensed under the MIT License.

#if NETCOREAPP
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using System;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace RhubarbGeekNz.Joinery
{
    [TestClass]
    public class TestJoinBoolean
    {
        readonly InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
        public TestJoinBoolean()
        {
            foreach (Type t in new Type[] {
                typeof(JoinBoolean)
            })
            {
                CmdletAttribute ca = t.GetCustomAttribute<CmdletAttribute>();

                if (ca == null) throw new NullReferenceException();

                initialSessionState.Commands.Add(new SessionStateCmdletEntry($"{ca.VerbName}-{ca.NounName}", t, ca.HelpUri));
            }

            initialSessionState.Variables.Add(new SessionStateVariableEntry("ErrorActionPreference", ActionPreference.Stop, "Stop action"));
        }

        [TestMethod]
        public void TestAndArgs1()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true -And");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsTrue((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestAndArgs11()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true, $true -And");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsTrue((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestAndArgs111()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true, $true, $true -And");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsTrue((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestAndArgs101()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true, $false, $true -And");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsFalse((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestAndArgs0()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $false -And");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsFalse((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestOrArgs0()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $false -Or");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsFalse((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestOrArgs1()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true -Or");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsTrue((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestOrArgs101()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true, $false, $true -Or");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsTrue((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestXorArgs101()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true, $false, $true -Xor");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsFalse((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestXorArgs1010()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true, $false, $true, $false -Xor");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsFalse((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestXorArgs10101()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true, $false, $true, $false, $true -Xor");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsTrue((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestAndInputEmpty()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("1 | ForEach-Object {} | Join-Boolean -And");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(0, outputPipeline.Count);
            }
        }

        [TestMethod]
        public void TestAndInputEmptyDefaultTrue()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("1 | ForEach-Object {} | Join-Boolean -And -DefaultValue $true");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsTrue((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestOrInputEmpty()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("1 | ForEach-Object {} | Join-Boolean -Or");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(0, outputPipeline.Count);
            }
        }

        [TestMethod]
        public void TestOrInputEmptyDefaultTrue()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("1 | ForEach-Object {} | Join-Boolean -Or -DefaultValue $true");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsTrue((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestXorInputEmpty()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("1 | ForEach-Object {} | Join-Boolean -Xor");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(0, outputPipeline.Count);
            }
        }

        [TestMethod]
        public void TestXorInputEmptyDefaultTrue()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("1 | ForEach-Object {} | Join-Boolean -Xor -DefaultValue $true");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsTrue((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestXorInputEmptyDefaultTrueNot()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("1 | ForEach-Object {} | Join-Boolean -Xor -DefaultValue $true -Not");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsFalse((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestAndInput111()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("$true, $true, $true | Join-Boolean -And");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsTrue((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestAndInput101()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("$true, $false, $true | Join-Boolean -And");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsFalse((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestPassThruInput1010()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("$true, $false, $true, $false | Join-Boolean -PassThru");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(4, outputPipeline.Count);
                Assert.IsTrue((bool)outputPipeline[0].BaseObject);
                Assert.IsFalse((bool)outputPipeline[1].BaseObject);
                Assert.IsTrue((bool)outputPipeline[2].BaseObject);
                Assert.IsFalse((bool)outputPipeline[3].BaseObject);
            }
        }

        [TestMethod]
        public void TestPassThruArgs1010()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true, $false, $true, $false -PassThru");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(4, outputPipeline.Count);
                Assert.IsTrue((bool)outputPipeline[0].BaseObject);
                Assert.IsFalse((bool)outputPipeline[1].BaseObject);
                Assert.IsTrue((bool)outputPipeline[2].BaseObject);
                Assert.IsFalse((bool)outputPipeline[3].BaseObject);
            }
        }

        [TestMethod]
        public void TestPassThruArgs1010NoEnumerate()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true, $false, $true, $false -PassThru -NoEnumerate");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                bool[] list = (bool[])outputPipeline[0].BaseObject;
                Assert.AreEqual(4, list.Length);
                Assert.IsTrue(list[0]);
                Assert.IsFalse(list[1]);
                Assert.IsTrue(list[2]);
                Assert.IsFalse(list[3]);
            }
        }

        [TestMethod]
        public void TestPassThruArgs1010Not()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true, $false, $true, $false -PassThru -Not");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(4, outputPipeline.Count);
                Assert.IsFalse((bool)outputPipeline[0].BaseObject);
                Assert.IsTrue((bool)outputPipeline[1].BaseObject);
                Assert.IsFalse((bool)outputPipeline[2].BaseObject);
                Assert.IsTrue((bool)outputPipeline[3].BaseObject);
            }
        }

        [TestMethod]
        public void TestAndEmpty()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean -And");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(0, outputPipeline.Count);
            }
        }

        [TestMethod]
        public void TestOrEmpty()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean -Or");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(0, outputPipeline.Count);
            }
        }

        [TestMethod]
        public void TestXorEmpty()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean -Xor");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(0, outputPipeline.Count);
            }
        }

        [TestMethod]
        public void TestNandArgs11()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("Join-Boolean $true, $true -And -Not");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsFalse((bool)outputPipeline[0].BaseObject);
            }
        }

        [TestMethod]
        public void TestNandInput11()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("$true, $true | Join-Boolean -And -Not");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsFalse((bool)outputPipeline[0].BaseObject);
            }
        }

        private void iterate(string[] flags, Func<bool[], bool> expectedResult)
        {
            for (int bits = 1; bits <= 4; bits++)
            {
                int combinations = 1 << bits;
                int j = 0;
                bool[] list = new bool[bits];

                while (j < combinations)
                {
                    for (int i = 0; i < bits; i++)
                    {
                        list[i] = 0 != (j & (1 << i));
                    }

                    String[] args = list.Select(b => b.ToString()).ToArray();
                    string testCase = $"Validate {bits} {combinations} {j} " + String.Join(",", flags) + " " + String.Join(",", args);

                    using (PowerShell powerShell = PowerShell.Create(initialSessionState))
                    {
                        powerShell.AddCommand("Join-Boolean");

                        powerShell.AddParameter("InputList", list);

                        foreach (string flag in flags)
                        {
                            powerShell.AddParameter(flag);
                        }

                        var outputPipeline = powerShell.Invoke();

                        Assert.AreEqual(1, outputPipeline.Count);
                        bool actualResult = (bool)outputPipeline[0].BaseObject;
                        Assert.AreEqual(expectedResult(list), actualResult, testCase);
                    }

                    using (PowerShell powerShell = PowerShell.Create(initialSessionState))
                    {
                        powerShell.AddCommand("Join-Boolean");

                        foreach (string flag in flags)
                        {
                            powerShell.AddParameter(flag);
                        }

                        PSDataCollection<bool> inputList = new PSDataCollection<bool>();

                        foreach (bool b in list)
                        {
                            inputList.Add(b);
                        }

                        var outputPipeline = powerShell.Invoke(inputList);

                        Assert.AreEqual(1, outputPipeline.Count);
                        bool actualResult = (bool)outputPipeline[0].BaseObject;
                        Assert.AreEqual(expectedResult(list), actualResult, testCase);
                    }

                    j++;
                }
            }
        }

        private void noInput(string[] flags, bool? defaultValue, bool? expectedResult)
        {
            string testCase = $"Validate empty {defaultValue} {expectedResult} " + String.Join(",", flags);

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddCommand("Join-Boolean");

                foreach (string flag in flags)
                {
                    powerShell.AddParameter(flag);
                }

                if (defaultValue.HasValue)
                {
                    powerShell.AddParameter("DefaultValue", defaultValue.Value);
                }

                var outputPipeline = powerShell.Invoke();

                if (expectedResult.HasValue)
                {
                    Assert.AreEqual(1, outputPipeline.Count);
                    bool actualResult = (bool)outputPipeline[0].BaseObject;
                    Assert.AreEqual(expectedResult.Value, actualResult, testCase);
                }
                else
                {
                    Assert.AreEqual(0, outputPipeline.Count);
                }
            }

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddCommand("Join-Boolean");

                powerShell.AddParameter("InputList", null);

                foreach (string flag in flags)
                {
                    powerShell.AddParameter(flag);
                }

                if (defaultValue.HasValue)
                {
                    powerShell.AddParameter("DefaultValue", defaultValue.Value);
                }

                var outputPipeline = powerShell.Invoke();

                if (expectedResult.HasValue)
                {
                    Assert.AreEqual(1, outputPipeline.Count);
                    bool actualResult = (bool)outputPipeline[0].BaseObject;
                    Assert.AreEqual(expectedResult.Value, actualResult, testCase);
                }
                else
                {
                    Assert.AreEqual(0, outputPipeline.Count);
                }
            }

            bool[] list = new bool[0];

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddCommand("Join-Boolean");

                powerShell.AddParameter("InputList", list);

                foreach (string flag in flags)
                {
                    powerShell.AddParameter(flag);
                }

                if (defaultValue.HasValue)
                {
                    powerShell.AddParameter("DefaultValue", defaultValue.Value);
                }

                var outputPipeline = powerShell.Invoke();

                if (expectedResult.HasValue)
                {
                    Assert.AreEqual(1, outputPipeline.Count);
                    bool actualResult = (bool)outputPipeline[0].BaseObject;
                    Assert.AreEqual(expectedResult.Value, actualResult, testCase);
                }
                else
                {
                    Assert.AreEqual(0, outputPipeline.Count);
                }
            }

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddCommand("Join-Boolean");

                foreach (string flag in flags)
                {
                    powerShell.AddParameter(flag);
                }

                if (defaultValue.HasValue)
                {
                    powerShell.AddParameter("DefaultValue", defaultValue.Value);
                }

                PSDataCollection<bool> inputList = new PSDataCollection<bool>();

                var outputPipeline = powerShell.Invoke(inputList);

                if (expectedResult.HasValue)
                {
                    Assert.AreEqual(1, outputPipeline.Count);
                    bool actualResult = (bool)outputPipeline[0].BaseObject;
                    Assert.AreEqual(expectedResult.Value, actualResult, testCase);
                }
                else
                {
                    Assert.AreEqual(0, outputPipeline.Count);
                }
            }
        }

        [TestMethod]
        public void TestCombinations()
        {
            iterate(new string[] { "And" }, b =>
            {
                bool result = true;

                foreach (bool c in b)
                {
                    result &= c;
                }

                return result;
            });

            iterate(new string[] { "And", "Not" }, b =>
            {
                bool result = true;

                foreach (bool c in b)
                {
                    result &= c;
                }

                return !result;
            });

            iterate(new string[] { "Or" }, b =>
            {
                bool result = false;

                foreach (bool c in b)
                {
                    result |= c;
                }

                return result;
            });

            iterate(new string[] { "Or", "Not" }, b =>
            {
                bool result = false;

                foreach (bool c in b)
                {
                    result |= c;
                }

                return !result;
            });

            iterate(new string[] { "Xor" }, b =>
            {
                bool result = false;

                foreach (bool c in b)
                {
                    result ^= c;
                }

                return result;
            });

            iterate(new string[] { "Xor", "Not" }, b =>
            {
                bool result = false;

                foreach (bool c in b)
                {
                    result ^= c;
                }

                return !result;
            });
        }

        [TestMethod]
        public void TestNoInput()
        {
            noInput(new string[] { "And" }, null, null);
            noInput(new string[] { "And", "Not" }, null, null);
            noInput(new string[] { "Or" }, null, null);
            noInput(new string[] { "Or", "Not" }, null, null);
            noInput(new string[] { "Xor" }, null, null);
            noInput(new string[] { "Xor", "Not" }, null, null);

            noInput(new string[] { "And" }, true, true);
            noInput(new string[] { "And", "Not" }, true, false);
            noInput(new string[] { "Or" }, true, true);
            noInput(new string[] { "Or", "Not" }, true, false);
            noInput(new string[] { "Xor" }, true, true);
            noInput(new string[] { "Xor", "Not" }, true, false);

            noInput(new string[] { "And" }, false, false);
            noInput(new string[] { "And", "Not" }, false, true);
            noInput(new string[] { "Or" }, false, false);
            noInput(new string[] { "Or", "Not" }, false, true);
            noInput(new string[] { "Xor" }, false, false);
            noInput(new string[] { "Xor", "Not" }, false, true);
        }
    }
}
