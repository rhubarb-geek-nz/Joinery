// Copyright (c) 2026 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Management.Automation;

namespace RhubarbGeekNz.Joinery
{
    [Cmdlet(VerbsCommon.Join, "Boolean")]
    [OutputType(typeof(bool))]
    [OutputType(typeof(bool[]))]
    sealed public class JoinBoolean : PSCmdlet
    {
        [Parameter(ParameterSetName = "input-passthru", Mandatory = true, ValueFromPipeline = true, HelpMessage = "Value to be output"), AllowEmptyCollection]
        [Parameter(ParameterSetName = "input-and", Mandatory = true, ValueFromPipeline = true, HelpMessage = "Value to be joined")]
        [Parameter(ParameterSetName = "input-or", Mandatory = true, ValueFromPipeline = true, HelpMessage = "Value to be joined")]
        [Parameter(ParameterSetName = "input-xor", Mandatory = true, ValueFromPipeline = true, HelpMessage = "Value to be joined")]
        public bool InputValue;

        [Parameter(ParameterSetName = "args-passthru", Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "Values to be passed through"), AllowEmptyCollection, AllowNull]
        [Parameter(ParameterSetName = "args-and", Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "Values to be joined")]
        [Parameter(ParameterSetName = "args-or", Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "Values to be joined")]
        [Parameter(ParameterSetName = "args-xor", Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "Values to be joined")]
        public bool[] InputList;

        [Parameter(ParameterSetName = "empty-and", Mandatory = true, ValueFromPipeline = false, HelpMessage = "Logical And Function")]
        [Parameter(ParameterSetName = "args-and", Mandatory = true, ValueFromPipeline = false, HelpMessage = "Logical And Function")]
        [Parameter(ParameterSetName = "input-and", Mandatory = true, ValueFromPipeline = false, HelpMessage = "Logical And Function")]
        public SwitchParameter And;

        [Parameter(ParameterSetName = "empty-or", Mandatory = true, ValueFromPipeline = false, HelpMessage = "Logical Or Function")]
        [Parameter(ParameterSetName = "args-or", Mandatory = true, ValueFromPipeline = false, HelpMessage = "Logical Or Function")]
        [Parameter(ParameterSetName = "input-or", Mandatory = true, ValueFromPipeline = false, HelpMessage = "Logical Or Function")]
        public SwitchParameter Or;

        [Parameter(ParameterSetName = "empty-xor", Mandatory = true, ValueFromPipeline = false, HelpMessage = "Logical Xor Function")]
        [Parameter(ParameterSetName = "args-xor", Mandatory = true, ValueFromPipeline = false, HelpMessage = "Logical Xor Function")]
        [Parameter(ParameterSetName = "input-xor", Mandatory = true, ValueFromPipeline = false, HelpMessage = "Logical Xor Function")]
        public SwitchParameter Xor;

        [Parameter(ParameterSetName = "input-passthru", Mandatory = true, ValueFromPipeline = false, HelpMessage = "Values to be passed through")]
        [Parameter(ParameterSetName = "args-passthru", Mandatory = true, ValueFromPipeline = false, HelpMessage = "Values to be passed through")]
        public SwitchParameter PassThru;

        private bool? resultValue;

        [Parameter(Mandatory = false, HelpMessage = "Result if no input provided")]
        public bool? DefaultValue;

        private bool invertResult, noEnumerate;

        [Parameter(Mandatory = false, HelpMessage = "Invert final result")]
        public SwitchParameter Not
        {
            get
            {
                return invertResult;
            }

            set
            {
                invertResult = value;
            }
        }

        [Parameter(ParameterSetName = "args-passthru", Mandatory = false, ValueFromPipeline = false, HelpMessage = "Prevent enumeration")]
        public SwitchParameter NoEnumerate
        {
            get
            {
                return noEnumerate;
            }

            set
            {
                noEnumerate = value;
            }
        }

        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case "input-and":
                    if (resultValue.HasValue)
                    {
                        resultValue &= InputValue;
                    }
                    else
                    {
                        resultValue = InputValue;
                    }
                    break;

                case "input-or":
                    if (resultValue.HasValue)
                    {
                        resultValue |= InputValue;
                    }
                    else
                    {
                        resultValue = InputValue;
                    }
                    break;

                case "input-xor":
                    if (resultValue.HasValue)
                    {
                        resultValue ^= InputValue;
                    }
                    else
                    {
                        resultValue = InputValue;
                    }
                    break;

                case "args-and":
                    if (InputList!=null && InputList.Length > 0)
                    {
                        int i = 0;
                        bool value = InputList[i++];
                        while (i < InputList.Length)
                        {
                            value &= InputList[i++];
                        }
                        WriteObject(value ^ invertResult);
                    }
                    else
                    {
                        if (DefaultValue.HasValue)
                        {
                            WriteObject(DefaultValue.Value ^ invertResult);
                        }
                    }
                    break;

                case "args-or":
                    if (InputList != null && InputList.Length > 0)
                    {
                        int i = 0;
                        bool value = InputList[i++];
                        while (i < InputList.Length)
                        {
                            value |= InputList[i++];
                        }
                        WriteObject(value ^ invertResult);
                    }
                    else
                    {
                        if (DefaultValue.HasValue)
                        {
                            WriteObject(DefaultValue.Value ^ invertResult);
                        }
                    }
                    break;

                case "args-xor":
                    if (InputList != null && InputList.Length > 0)
                    {
                        int i = 0;
                        bool value = InputList[i++];
                        while (i < InputList.Length)
                        {
                            value ^= InputList[i++];
                        }
                        WriteObject(value ^ invertResult);
                    }
                    else
                    {
                        if (DefaultValue.HasValue)
                        {
                            WriteObject(DefaultValue.Value ^ invertResult);
                        }
                    }
                    break;

                case "input-passthru":
                    WriteObject(InputValue ^ invertResult);
                    break;

                case "args-passthru":
                    if (InputList != null && InputList.Length > 0)
                    {
                        int i = InputList.Length;
                        bool [] result = new bool[i];

                        while (0 != i--)
                        {
                            result[i] = InputList[i] ^ invertResult;
                        }

                        WriteObject(result, !noEnumerate);
                    }
                    else
                    {
                        WriteObject(InputList);
                    }
                    break;

                case "empty-and":
                case "empty-or":
                case "empty-xor":
                    break;

                default:
                    Exception ex = new ParameterBindingException();
                    WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidArgument, ParameterSetName));
                    break;
            }
        }

        protected override void EndProcessing()
        {
            switch (ParameterSetName)
            {
                case "input-and":
                case "input-or":
                case "input-xor":
                case "empty-and":
                case "empty-or":
                case "empty-xor":
                    if (resultValue.HasValue)
                    {
                        WriteObject(resultValue.Value ^ invertResult);
                    }
                    else
                    {
                        if (DefaultValue.HasValue)
                        {
                            WriteObject(DefaultValue.Value ^ invertResult);
                        }
                    }
                    break;
            }
        }
    }
}
