﻿using System;
namespace Loader.MapperModels.SourceDataModels
{
    public class SourceDataVariable
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public bool IsDerived { get; set; }

        public SourceDataVariable()
        {
        }
    }
}

