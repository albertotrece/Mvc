// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace MvcTagHelpersWebSite.Models
{
    public class Employee
    {
        public int Number
        {
            get;
            set;
        }

        [Required]
        public string Name
        {
            get;
            set;
        }

        public string Phone
        {
            get;
            set;
        }

        public Gender Gender
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }

        public string OfficeNumber
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public bool Remote
        {
            get;
            set;
        }
    }
}