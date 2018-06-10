using System;
using System.Collections.Generic;
using System.Reflection;

namespace WhatDoBot.MvXForms.Core
{
    public class CreatableTypesContainer
    {
        public Func<Assembly, IEnumerable<Type>> CreatableTypes { get; }
        public CreatableTypesContainer(Func<Assembly, IEnumerable<Type>> creatableTypes) => CreatableTypes = creatableTypes;
    }
}
