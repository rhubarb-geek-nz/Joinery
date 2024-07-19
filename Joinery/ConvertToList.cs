// Copyright (c) 2024 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace RhubarbGeekNz.Joinery
{
    [Cmdlet(VerbsData.ConvertTo, "List", DefaultParameterSetName = "type")]
    [OutputType(typeof(IList))]
    sealed public class ConvertToList : PSCmdlet
    {
        private IList list;
        private bool baseObject, passThru;

        [Parameter(ParameterSetName = "list", Mandatory = true, HelpMessage = "List to populate")]
        public IList List;

        [Parameter(ParameterSetName = "list", Mandatory = false, HelpMessage = "Write list to output")]
        public SwitchParameter PassThru
        {
            get
            {
                return passThru;
            }

            set
            {
                passThru = value;
            }
        }

        [Parameter(ParameterSetName = "type", Mandatory = false, HelpMessage = "Type of list element")]
        public Type Type;

        [Parameter(Mandatory = false, HelpMessage = "Use BaseObject from PSObject")]
        public SwitchParameter BaseObject
        {
            get
            {
                return baseObject;
            }

            set
            {
                baseObject = value;
            }
        }

        [Parameter(Mandatory = false, ValueFromPipeline = true, HelpMessage = "Entry to add to list")]
        public PSObject InputObject;

        protected override void BeginProcessing()
        {
            list = List == null ? Type == null ? new ArrayList() : (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { Type })) : List;
        }

        protected override void ProcessRecord()
        {
            try
            {
                list.Add(InputObject == null ? null : baseObject ? InputObject.BaseObject : InputObject);
            }
            catch (ArgumentException ex)
            {
                WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidData, null));
            }
        }

        protected override void EndProcessing()
        {
            if (passThru || List == null)
            {
                WriteObject(list);
            }
        }
    }
}
