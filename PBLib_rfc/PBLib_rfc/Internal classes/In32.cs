﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBLib
{

    internal class In32 : In
    {
        private readonly int mantSize = 21;
        private readonly int expSize = 8;

        public override int MantLength
        {
            get { return mantSize; }
        }

        public override int ExpLength
        {
            get { return expSize; }
        }
    }

}