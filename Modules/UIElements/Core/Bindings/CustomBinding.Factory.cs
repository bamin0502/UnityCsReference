// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

﻿using System;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
    public abstract partial class CustomBinding
    {
        [ExcludeFromDocs, Serializable]
        public new abstract class UxmlSerializedData : Binding.UxmlSerializedData
        {
        }
    }
}
