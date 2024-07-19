// Copyright (c) 2024 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Management.Automation;

namespace RhubarbGeekNz.Joinery
{
    [Cmdlet(VerbsCommon.Join, "Dictionary", DefaultParameterSetName = "new")]
    [OutputType(typeof(IDictionary))]
    sealed public class JoinDictionary : PSCmdlet
    {
        private IDictionary dictionary;
        private bool dictionarySet;
        private bool passThru;

        [Parameter(ParameterSetName = "dict", Mandatory = true, ValueFromPipeline = false, HelpMessage = "Destination Dictionary")]
        public IDictionary Dictionary
        {
            get
            {
                return dictionary;
            }

            set
            {
                dictionarySet = true;
                dictionary = value;
            }
        }

        [Parameter(ParameterSetName = "dict", Mandatory = false, HelpMessage = "Write dictionary to output")]
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

        [Parameter(Mandatory = false, ValueFromPipeline = true, HelpMessage = "Entries to add to dictionary")]
        public PSObject InputObject;

        protected override void BeginProcessing()
        {
            if (dictionarySet)
            {
                if (null == dictionary)
                {
                    Exception ex = new NullReferenceException();
                    WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidArgument, null));
                }
            }
            else
            {
                dictionary = new Hashtable();
                passThru = true;
            }
        }

        protected override void ProcessRecord()
        {
            if (InputObject != null)
            {
                if (InputObject.BaseObject is DictionaryEntry entry)
                {
                    dictionary.Add(entry.Key, entry.Value);
                }
                else
                {
                    if (InputObject.BaseObject is IDictionary dict)
                    {
                        foreach (var key in dict.Keys)
                        {
                            dictionary.Add(key, dict[key]);
                        }
                    }
                    else
                    {
                        if (InputObject.BaseObject is PSNoteProperty noteProperty)
                        {
                            dictionary.Add(noteProperty.Name, noteProperty.Value);
                        }
                        else
                        {
                            if (InputObject.BaseObject is PSCustomObject)
                            {
                                foreach (var propertyInfo in InputObject.Properties)
                                {
                                    dictionary.Add(propertyInfo.Name, propertyInfo.Value);
                                }
                            }
                            else
                            {
                                Exception ex = new ParameterBindingException();
                                WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidArgument, InputObject.BaseObject));
                            }
                        }
                    }
                }
            }
        }

        protected override void EndProcessing()
        {
            if (passThru && null != dictionary)
            {
                WriteObject(dictionary);
            }
        }
    }
}
