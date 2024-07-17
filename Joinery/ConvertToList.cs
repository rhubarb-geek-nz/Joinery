// Copyright (c) 2024 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Management.Automation;

namespace RhubarbGeekNz.Joinery
{
    [Cmdlet(VerbsData.ConvertTo, "List")]
    [OutputType(typeof(ArrayList))]
    sealed public class ConvertToList : PSCmdlet
    {
        private IList list;
        private bool baseObject;

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
            list = new ArrayList();
        }

        protected override void ProcessRecord()
        {
            list.Add(InputObject == null ? null : baseObject ? InputObject.BaseObject : InputObject);
        }

        protected override void EndProcessing()
        {
            WriteObject(list);
        }
    }
}
