// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine.Bindings;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

namespace UnityEditor
{
    internal struct PlatformIconStruct
    {
        [NativeName("m_Width")]
        public int Width;
        [NativeName("m_Height")]
        public int Height;
        [NativeName("m_Kind")]
        public int Kind;
        [NativeName("m_SubKind")]
        public string SubKind;
        [NativeName("m_Textures")]
        public Texture2D[] Textures;
    }

    internal struct LegacyPlatformIcon
    {
        [NativeName("m_Icon")]
        public Texture2D Icon;
        [NativeName("m_Width")]
        public int Width;
        [NativeName("m_Height")]
        public int Height;
        [NativeName("m_Kind")]
        public IconKind Kind;
    }

    internal struct LegacyBuildTargetIcons
    {
        [NativeName("m_BuildTarget")]
        public string BuildTarget;
        [NativeName("m_Icons")]
        public LegacyPlatformIcon[] Icons;
    }

    internal struct BuildTargetIcons
    {
        [NativeName("m_BuildTarget")]
        public string BuildTarget;
        [NativeName("m_Icons")]
        public PlatformIconStruct[] Icons;
    }

    public class PlatformIcon
    {
        internal List<Texture2D> m_Textures;
        private Texture2D[] m_PreviewTextures;

        public int layerCount
        {
            get
            {
                return m_Textures.Count;
            }
            set
            {
                value = Math.Clamp(value, minLayerCount, maxLayerCount);

                if (value < m_Textures.Count)
                    m_Textures.RemoveRange(value, m_Textures.Count - 1);
                else if (value > m_Textures.Count)
                    m_Textures.AddRange(new Texture2D[value - m_Textures.Count]);
            }
        }

        public int maxLayerCount  { get; private set; }
        public int minLayerCount  { get; private set; }
        internal string description  { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }
        internal bool draggable { get; set; }

        public PlatformIconKind kind { get; private set; }
        internal string iconSubKind { get; private set; }

        internal PlatformIconStruct GetPlatformIconStruct()
        {
            PlatformIconStruct platformIconStruct = new PlatformIconStruct();
            platformIconStruct.Textures = m_Textures.ToArray();
            platformIconStruct.Width = width;
            platformIconStruct.Height = height;
            platformIconStruct.Kind = kind.kind;
            platformIconStruct.SubKind = iconSubKind;

            return platformIconStruct;
        }

        internal bool IsEmpty()
        {
            return m_Textures.Count(t => t != null) == 0;
        }

        internal static PlatformIcon[] GetRequiredPlatformIconsByType(PlatformIconKind kind, IReadOnlyDictionary<PlatformIconKind, PlatformIcon[]> requiredIcons)
        {
            if (kind != PlatformIconKind.Any)
                return requiredIcons[kind];

            return requiredIcons.Values.SelectMany(i => i).ToArray();
        }

        internal PlatformIcon(int width, int height, int minLayerCount, int maxLayerCount, string iconSubKind, string description, PlatformIconKind kind, bool draggable = true)
        {
            this.width = width;
            this.height = height;
            this.iconSubKind = iconSubKind;
            this.description = description;
            this.minLayerCount = minLayerCount;
            this.maxLayerCount = maxLayerCount;
            this.kind = kind;
            this.draggable = draggable;

            m_Textures = new List<Texture2D>();
        }

        public Texture2D GetTexture(int layer = 0)
        {
            if (layer < 0 || layer >= maxLayerCount)
                throw new ArgumentOutOfRangeException($"Attempting to retrieve icon layer {layer}, while the icon only contains {layerCount} layers!");
            return layer < layerCount ? m_Textures[layer] : null;
        }

        public Texture2D[] GetTextures()
        {
            return m_Textures.ToArray();
        }

        internal void SetPreviewTextures(Texture2D[] textures)
        {
            m_PreviewTextures = textures;
        }

        internal Texture2D[] GetPreviewTextures()
        {
            return m_PreviewTextures;
        }

        public void SetTexture(Texture2D texture, int layer = 0)
        {
            if (layer < 0 || layer >= maxLayerCount)
            {
                throw new ArgumentOutOfRangeException($"Attempting to set icon layer {layer}, while icon only supports {maxLayerCount} layers!");
            }
            if (layer > m_Textures.Count - 1)
            {
                for (int i = m_Textures.Count; i <= layer; i++)
                    m_Textures.Add(null);
            }

            m_Textures[layer] = texture;
        }

        public void SetTextures(params Texture2D[] textures)
        {
            if (textures == null || textures.Length == 0 || textures.Count(t => t != null) == 0)
            {
                m_Textures.Clear();
                return;
            }
            else if (textures.Length > maxLayerCount || textures.Length < minLayerCount)
            {
                throw new InvalidOperationException($"Attempting to assign an incorrect amount of layers to an PlatformIcon, trying to assign {textures.Length} textures while the Icon requires atleast {minLayerCount} but no more than {maxLayerCount} layers");
            }

            m_Textures = textures.ToList();
        }

        internal int GetValidLayerCount()
        {
            var validLayerCount = m_Textures.Count(t => t != null);
            var previewTexturesCount = GetPreviewTextures().Count(t => t != null);

            return Math.Max(previewTexturesCount, validLayerCount);
        }

        public override string ToString() { return string.Format("({0}x{1}) {2}", width, height, description); }
    }

    public class PlatformIconKind
    {
        internal static readonly PlatformIconKind Any = new PlatformIconKind(-1, "Any", "", NamedBuildTarget.Unknown);

        internal int kind { get; private set; }
        internal string platform { get; private set; }
        internal string[] customLayerLabels { get; private set; }
        internal string description { get; set; }
        private string kindString { get; set; }

        internal PlatformIconKind(int kind, string kindString, string description, NamedBuildTarget platform, string[] customLayerLabels = null)
        {
            this.kind = kind;
            this.platform = platform.TargetName;
            this.kindString = kindString;
            this.customLayerLabels = customLayerLabels;
            this.description = description;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return kind == ((PlatformIconKind)obj).kind && platform == ((PlatformIconKind)obj).platform;
        }

        public override int GetHashCode()
        {
            return kind.GetHashCode();
        }

        public override string ToString() { return kindString; }
    }

    [NativeHeader("Runtime/Misc/PlayerSettings.h")]
    public partial class PlayerSettings : UnityEngine.Object
    {
        // TODO: once we move the icon stuff to a common editor plugin, move it there as well
        internal static void InitIconsStructs(NamedBuildTarget buildTarget)
        {
            // This forces icon structures to be initialized in PlayerSettings on Editor start if needed
            // If we don't do this, the following might happen:
            // * Create new project
            // * ProjectSettings.asset will have its m_BuildTargetPlatformIcons structure empty
            // * But once you build to Android/iOS/tvOS or any platform which uses IIconPlatformProperties, it will initialize m_BuildTargetPlatformIcons
            // * Causing ProjectSettings.asset to change after you clicked build
            // * This is bad for incremental builds, at least for the 2nd/sequential build, it will see that ProjectSettings.asset has changed and will start rebuilding resources
            if (!BuildTargetDiscovery.TryGetBuildTarget(BuildPipeline.GetBuildTargetByName(buildTarget.TargetName), out var iBuildTarget))
                return;
            var requiredIcons = iBuildTarget.IconPlatformProperties?.GetRequiredPlatformIcons();
            if (requiredIcons == null)
                return;

            foreach(var icon in requiredIcons.Keys)
            {
                // Init the internal structures with empty icons
                GetPlatformIcons(buildTarget, icon);
            }
        }

        internal static PlatformIcon[] GetPlatformIconsFromStruct(PlatformIcon[] icons, PlatformIconKind kind, PlatformIconStruct[] serializedIcons)
        {
            foreach (var icon in icons)
            {
                foreach (var serializedIcon in serializedIcons)
                {
                    var requiredKind = kind.Equals(PlatformIconKind.Any) ? (int)serializedIcon.Kind : kind.kind;
                    if (icon.kind.kind != requiredKind || icon.iconSubKind != serializedIcon.SubKind) continue;
                    if (icon.width != serializedIcon.Width || icon.height != serializedIcon.Height) continue;
                    var serializedTextures = serializedIcon.Textures.Take(icon.maxLayerCount).ToArray();
                    var textures = new Texture2D[serializedTextures.Length > icon.minLayerCount
                                                 ? serializedTextures.Length
                                                 : icon.minLayerCount];

                    for (int i = 0; i < serializedTextures.Length; i++)
                        textures[i] = serializedTextures[i];

                    icon.SetTextures(textures);
                    break;
                }
            }

            return icons;
        }

        [Obsolete("Use GetPlatformIcons(NamedBuildTarget , PlatformIconKind) instead")]
        public static PlatformIcon[] GetPlatformIcons(BuildTargetGroup platform, PlatformIconKind kind) =>
            GetPlatformIcons(NamedBuildTarget.FromBuildTargetGroup(platform), kind);

        // Loops through 'requiredIconSlots' and fills it with icons that are already serialized.
        public static PlatformIcon[] GetPlatformIcons(NamedBuildTarget buildTarget, PlatformIconKind kind)
        {
            if (!BuildTargetDiscovery.TryGetBuildTarget(BuildPipeline.GetBuildTargetByName(buildTarget.TargetName), out var iBuildTarget))
                return Array.Empty<PlatformIcon>();
            var requiredIcons = iBuildTarget.IconPlatformProperties?.GetRequiredPlatformIcons();
            if (requiredIcons == null)
                return Array.Empty<PlatformIcon>();

            var icons = PlatformIcon.GetRequiredPlatformIconsByType(kind, requiredIcons);
            var serializedIcons = GetPlatformIconsInternal(buildTarget.TargetName, kind.kind);

            if (serializedIcons.Length <= 0)
            {
                ImportLegacyIcons(buildTarget.TargetName, kind, icons);
                SetPlatformIcons(buildTarget, kind, icons);

                foreach (var icon in icons)
                    if (icon.IsEmpty())
                        icon.SetTextures(null);
            }
            else
            {
                icons = GetPlatformIconsFromStruct(icons, kind, serializedIcons);
            }
            return icons;
        }

        [Obsolete("Use SetPlatformIcons(NamedBuildTarget , PlatformIconKind) instead")]
        public static void SetPlatformIcons(BuildTargetGroup platform, PlatformIconKind kind, PlatformIcon[] icons) =>
            SetPlatformIcons(NamedBuildTarget.FromBuildTargetGroup(platform), kind, icons);

        public static void SetPlatformIcons(NamedBuildTarget buildTarget, PlatformIconKind kind, PlatformIcon[] icons)
        {
            if (!BuildTargetDiscovery.TryGetBuildTarget(BuildPipeline.GetBuildTargetByName(buildTarget.TargetName), out var iBuildTarget))
                return;

            var requiredIcons = iBuildTarget.IconPlatformProperties?.GetRequiredPlatformIcons();
            if (requiredIcons == null)
                return;

            var requiredIconCount = PlatformIcon.GetRequiredPlatformIconsByType(kind, requiredIcons).Length;

            PlatformIconStruct[] iconStructs;
            if (icons == null)
                iconStructs = new PlatformIconStruct[0];
            else if (requiredIconCount != icons.Length)
            {
                throw new InvalidOperationException($"Attempting to set an incorrect number of icons for {buildTarget.TargetName} {kind} kind, it requires {requiredIconCount} icons but trying to assign {icons.Length}.");
            }
            else
            {
                iconStructs = icons.Select(
                    i => i.GetPlatformIconStruct()
                    ).ToArray();
            }

            SetPlatformIconsInternal(buildTarget.TargetName, iconStructs, kind.kind);
        }

        [Obsolete("Use GetSupportedIconKinds(NamedBuildTarget) instead")]
        public static PlatformIconKind[] GetSupportedIconKindsForPlatform(BuildTargetGroup platform) =>
            GetSupportedIconKinds(NamedBuildTarget.FromBuildTargetGroup(platform));

        public static PlatformIconKind[] GetSupportedIconKinds(NamedBuildTarget buildTarget)
        {
            if (BuildTargetDiscovery.TryGetBuildTarget(BuildPipeline.GetBuildTargetByName(buildTarget.TargetName), out var iBuildTarget))
            {
                var requiredIcons = iBuildTarget.IconPlatformProperties?.GetRequiredPlatformIcons();
                if (requiredIcons != null)
                    return requiredIcons.Keys.ToArray();
            }
            return new PlatformIconKind[] { };
        }

        internal static int GetNonEmptyPlatformIconCount(PlatformIcon[] icons)
        {
            return icons.Count(i => !i.IsEmpty());
        }

        internal static int GetValidPlatformIconCount(PlatformIcon[] icons)
        {
            return icons.Count(
                i => i.GetTextures().Count(t => t != null) >= i.minLayerCount && i.layerCount <= i.maxLayerCount
            );
        }

        [StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
        [NativeMethod(Name = "SetIconsForPlatform")]
        extern internal static void SetPlatformIconsInternal(string platform, PlatformIconStruct[] icons, int kind);

        [StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
        extern internal static LegacyBuildTargetIcons[] SetPlatformIconsForTargetIcons(string platform, Texture2D[] icons, IconKind kind, LegacyBuildTargetIcons[] allIcons);

        [StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
        extern internal static BuildTargetIcons[] SetIconsForPlatformForTargetIcons(string platform, PlatformIconStruct[] icons, int kind, BuildTargetIcons[] allIcons);

        [StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
        [NativeMethod(Name = "GetIconsForPlatform")]
        extern internal static PlatformIconStruct[] GetPlatformIconsInternal(string platform, int kind);

        [StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
        extern internal static Texture2D[] GetPlatformIconsForTargetIcons(string platform, IconKind kind, LegacyBuildTargetIcons[] allIcons);

        [StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
        [NativeMethod(Name = "GetIconsForPlatformFromTargetIcons")]
        extern internal static PlatformIconStruct[] GetPlatformIconsFromTargetIcons(string platform, int kind, BuildTargetIcons[] allIcons);

        extern internal static Texture2D GetPlatformIconForSize(string platform, int width, int height, IconKind kind = IconKind.Application);

        // Get the texture that will be used as the display icon at a specified size for the specified platform.
        internal static extern Texture2D GetPlatformIconForSizeForTargetIcons(string platform, int width, int height, IconKind kind, LegacyBuildTargetIcons[] allIcons);

        // Get the texture that will be used as the display icon at a specified size for the specified platform.
        [StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
        extern internal static Texture2D GetPlatformIconAtSize(string platform, int width, int height, int kind, string subKind = "", int layer = 0);

        // Get the texture that will be used as the display icon at a specified size for the specified platform.
        [StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
        extern internal static Texture2D GetPlatformIconAtSizeForTargetIcons(string platform, int width, int height, BuildTargetIcons[] allIcons, int kind, string subKind = "", int layer = 0);

        internal static void ClearSetIconsForPlatform(NamedBuildTarget buildTarget)
        {
            SetPlatformIcons(buildTarget, PlatformIconKind.Any, null);
        }

        // Old API methods, will be made obsolete when the new API is implemented for all platforms,
        // currently it functions as a wrapper for the new API for all platforms that support it (iOS, Android & tvOS).
        [Obsolete("Use SetIcons(NamedBuildTarget, Texture2D[], IconKind) instead")]
        public static void SetIconsForTargetGroup(BuildTargetGroup platform, Texture2D[] icons, IconKind kind) =>
            SetIcons(NamedBuildTarget.FromBuildTargetGroup(platform), icons, kind);

        public static void SetIcons(NamedBuildTarget buildTarget, Texture2D[] icons, IconKind kind)
        {
            if (BuildTargetDiscovery.TryGetBuildTarget(BuildPipeline.GetBuildTargetByName(buildTarget.TargetName), out var iBuildTarget) &&
                iBuildTarget.IconPlatformProperties != null)
            {
                var platformIconKind = iBuildTarget.IconPlatformProperties.GetPlatformIconKindFromEnumValue(kind);
                if (platformIconKind != null)
                {
                    PlatformIcon[] platformIcons = GetPlatformIcons(buildTarget, platformIconKind);

                    for (var i = 0; i < icons.Length; i++)
                        platformIcons[i].SetTexture(icons[i], 0);

                    SetPlatformIcons(buildTarget, platformIconKind, platformIcons);
                }
            }
            else
                SetIconsForPlatform(buildTarget.TargetName, icons, kind);
        }

        [Obsolete("Use SetIcons(NamedBuildTarget, Texture2D[], IconKind) instead")]
        // Assign a list of icons for the specified platform.
        public static void SetIconsForTargetGroup(BuildTargetGroup platform, Texture2D[] icons) =>
            SetIcons(NamedBuildTarget.FromBuildTargetGroup(platform), icons, IconKind.Any);

        // Returns the list of assigned icons for the specified platform of a specific kind.
        [Obsolete("Use GetIcons(NamedBuildTarget, IconKind) instead")]
        public static Texture2D[] GetIconsForTargetGroup(BuildTargetGroup platform, IconKind kind) =>
            GetIcons(NamedBuildTarget.FromBuildTargetGroup(platform), kind);

        public static Texture2D[] GetIcons(NamedBuildTarget buildTarget, IconKind kind)
        {
            if (BuildTargetDiscovery.TryGetBuildTarget(BuildPipeline.GetBuildTargetByName(buildTarget.TargetName), out var iBuildTarget))
            {
                var platformIconKind = iBuildTarget.IconPlatformProperties?.GetPlatformIconKindFromEnumValue(kind);
                return platformIconKind == null ?
                    GetIconsForPlatform(iBuildTarget.TargetName, kind) :
                    GetPlatformIcons(buildTarget, platformIconKind).Select(t => t.GetTexture(0)).ToArray();
            }
            return PlayerSettings.GetIconsForPlatform(buildTarget.TargetName, kind);
        }

        internal static void ImportLegacyIcons(string platform, PlatformIconKind kind, PlatformIcon[] platformIcons)
        {
            if (!Enum.IsDefined(typeof(IconKind), kind.kind))
                return;

            IconKind iconKind = (IconKind)kind.kind;

            Texture2D[] legacyIcons = GetIconsForPlatform(platform, iconKind);
            int[] legacyIconWidths = GetIconWidthsForPlatform(platform, iconKind);
            int[] legacyIconHeights  = GetIconHeightsForPlatform(platform, iconKind);

            for (var i = 0; i < legacyIcons.Length; i++)
            {
                foreach (var icon in platformIcons)
                {
                    if (icon.width == legacyIconWidths[i] && icon.height == legacyIconHeights[i])
                    {
                        icon.SetTextures(legacyIcons[i]);
                    }
                }
            }
        }

        internal static void ImportLegacyIcons(BuildTargetGroup platform, PlatformIconKind kind, PlatformIcon[] platformIcons)
        {
            ImportLegacyIcons(GetPlatformName(platform), kind, platformIcons);
        }

        // Returns the list of assigned icons for the specified platform.
        [Obsolete("Use GetIcons(NamedBuildTarget, IconKind) instead")]
        public static Texture2D[] GetIconsForTargetGroup(BuildTargetGroup platform) =>
            GetIcons(NamedBuildTarget.FromBuildTargetGroup(platform), IconKind.Any);

        // Returns a list of icon sizes for the specified platform of a specific kind.
        [Obsolete("Use GetIconSizes(NamedBuildTarget, IconKind) instead")]
        public static int[] GetIconSizesForTargetGroup(BuildTargetGroup platform, IconKind kind) =>
            GetIconSizes(NamedBuildTarget.FromBuildTargetGroup(platform), kind);

        public static int[] GetIconSizes(NamedBuildTarget buildTarget, IconKind kind)
        {
            if (BuildTargetDiscovery.TryGetBuildTarget(BuildPipeline.GetBuildTargetByName(buildTarget.TargetName), out var iBuildTarget))
            {
                var platformIconKind = iBuildTarget.IconPlatformProperties?.GetPlatformIconKindFromEnumValue(kind);
                return platformIconKind == null ?
                    GetIconWidthsForPlatform(iBuildTarget.TargetName, kind) :
                    GetPlatformIcons(buildTarget, platformIconKind).Select(s => s.width).ToArray();
            }
            return GetIconWidthsForPlatform(buildTarget.TargetName, kind);
        }

        // Returns a list of icon sizes for the specified platform.
        [Obsolete("Use GetIconSizes(NamedBuildTarget) instead")]
        public static int[] GetIconSizesForTargetGroup(BuildTargetGroup platform) =>
            GetIconSizes(NamedBuildTarget.FromBuildTargetGroup(platform), IconKind.Any);
    }
}
