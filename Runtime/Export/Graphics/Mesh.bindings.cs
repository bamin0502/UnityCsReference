// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine
{
    [UnityEngine.ExcludeFromPreset]
    [NativeHeader("Runtime/Graphics/Mesh/MeshScriptBindings.h")]
    public sealed partial class Mesh
    {
        [FreeFunction("MeshScripting::CreateMesh")] extern private static void Internal_Create([Writable] Mesh mono);

        [RequiredByNativeCode] // Used by IMGUI (even on empty projects, it draws development console & watermarks)
        public Mesh()
        {
            Internal_Create(this);
        }

        [FreeFunction("MeshScripting::MeshFromInstanceId")] extern internal static Mesh FromInstanceID(EntityId id);


        // triangles/indices

        extern public UnityEngine.Rendering.IndexFormat indexFormat { get; set; }

        extern internal UInt32 GetTotalIndexCount();

        [FreeFunction(Name = "MeshScripting::SetIndexBufferParams", HasExplicitThis = true, ThrowsException = true)]
        extern public void SetIndexBufferParams(int indexCount, UnityEngine.Rendering.IndexFormat format);

        [FreeFunction(Name = "MeshScripting::InternalSetIndexBufferData", HasExplicitThis = true, ThrowsException = true)]
        extern private void InternalSetIndexBufferData(IntPtr data, int dataStart, int meshBufferStart, int count, int elemSize, UnityEngine.Rendering.MeshUpdateFlags flags);
        [FreeFunction(Name = "MeshScripting::InternalSetIndexBufferDataFromArray", HasExplicitThis = true, ThrowsException = true)]
        extern private void InternalSetIndexBufferDataFromArray(System.Array data, int dataStart, int meshBufferStart, int count, int elemSize, UnityEngine.Rendering.MeshUpdateFlags flags);

        [FreeFunction(Name = "MeshScripting::SetVertexBufferParamsFromPtr", HasExplicitThis = true, ThrowsException = true)]
        extern private void SetVertexBufferParamsFromPtr(int vertexCount, IntPtr attributesPtr, int attributesCount);
        [FreeFunction(Name = "MeshScripting::SetVertexBufferParamsFromArray", HasExplicitThis = true, ThrowsException = true)]
        extern private void SetVertexBufferParamsFromArray(int vertexCount, params UnityEngine.Rendering.VertexAttributeDescriptor[] attributes);

        [FreeFunction(Name = "MeshScripting::InternalSetVertexBufferData", HasExplicitThis = true)]
        extern private void InternalSetVertexBufferData(int stream, IntPtr data, int dataStart, int meshBufferStart, int count, int elemSize, UnityEngine.Rendering.MeshUpdateFlags flags);
        [FreeFunction(Name = "MeshScripting::InternalSetVertexBufferDataFromArray", HasExplicitThis = true)]
        extern private void InternalSetVertexBufferDataFromArray(int stream, System.Array data, int dataStart, int meshBufferStart, int count, int elemSize, UnityEngine.Rendering.MeshUpdateFlags flags);

        [FreeFunction(Name = "MeshScripting::GetVertexAttributesAlloc", HasExplicitThis = true)]
        extern private System.Array GetVertexAttributesAlloc();
        [FreeFunction(Name = "MeshScripting::GetVertexAttributesArray", HasExplicitThis = true)]
        extern private int GetVertexAttributesArray([NotNull] UnityEngine.Rendering.VertexAttributeDescriptor[] attributes);
        [FreeFunction(Name = "MeshScripting::GetVertexAttributesList", HasExplicitThis = true)]
        extern private int GetVertexAttributesList([NotNull] System.Collections.Generic.List<UnityEngine.Rendering.VertexAttributeDescriptor> attributes);
        [FreeFunction(Name = "MeshScripting::GetVertexAttributesCount", HasExplicitThis = true)]
        extern private int GetVertexAttributeCountImpl();
        [FreeFunction(Name = "MeshScripting::GetVertexAttributeByIndex", HasExplicitThis = true, ThrowsException = true)]
        extern public UnityEngine.Rendering.VertexAttributeDescriptor GetVertexAttribute(int index);

        [FreeFunction(Name = "MeshScripting::GetIndexStart", HasExplicitThis = true)]
        extern private UInt32 GetIndexStartImpl(int submesh, int meshlod);

        [FreeFunction(Name = "MeshScripting::GetIndexCount", HasExplicitThis = true)]
        extern private UInt32 GetIndexCountImpl(int submesh, int meshlod);

        [FreeFunction(Name = "MeshScripting::GetTrianglesCount", HasExplicitThis = true)]
        extern private UInt32 GetTrianglesCountImpl(int submesh, int meshlod);

        [FreeFunction(Name = "MeshScripting::GetBaseVertex", HasExplicitThis = true)]
        extern private UInt32 GetBaseVertexImpl(int submesh);

        [FreeFunction(Name = "MeshScripting::GetTriangles", HasExplicitThis = true)]
        extern private int[] GetTrianglesImpl(int submesh, bool applyBaseVertex, int meshlod);

        [FreeFunction(Name = "MeshScripting::GetIndices", HasExplicitThis = true)]
        extern private int[] GetIndicesImpl(int submesh, bool applyBaseVertex, int meshlod);

        [FreeFunction(Name = "SetMeshIndicesFromScript", HasExplicitThis = true, ThrowsException = true)]
        extern private void SetIndicesImpl(int submesh, MeshTopology topology, UnityEngine.Rendering.IndexFormat indicesFormat, System.Array indices, int arrayStart, int arraySize, bool calculateBounds, int baseVertex, int meshlod);

        [FreeFunction(Name = "SetMeshIndicesFromNativeArray", HasExplicitThis = true, ThrowsException = true)]
        extern private void SetIndicesNativeArrayImpl(int submesh, MeshTopology topology, UnityEngine.Rendering.IndexFormat indicesFormat, IntPtr indices, int arrayStart, int arraySize, bool calculateBounds, int baseVertex, int meshlod);

        [FreeFunction(Name = "MeshScripting::ExtractTrianglesToArray", HasExplicitThis = true)]
        extern private void GetTrianglesNonAllocImpl([Out] int[] values, int submesh, bool applyBaseVertex, int meshlod);

        [FreeFunction(Name = "MeshScripting::ExtractTrianglesToArray16", HasExplicitThis = true)]
        extern private void GetTrianglesNonAllocImpl16([Out] ushort[] values, int submesh, bool applyBaseVertex, int meshlod);

        [FreeFunction(Name = "MeshScripting::ExtractIndicesToArray", HasExplicitThis = true)]
        extern private void GetIndicesNonAllocImpl([Out] int[] values, int submesh, bool applyBaseVertex, int meshlod);

        [FreeFunction(Name = "MeshScripting::ExtractIndicesToArray16", HasExplicitThis = true)]
        extern private void GetIndicesNonAllocImpl16([Out] ushort[] values, int submesh, bool applyBaseVertex, int meshlod);

        // component (channels) setters/getters helpers

        [FreeFunction(Name = "MeshScripting::PrintErrorCantAccessChannel", HasExplicitThis = true)]
        extern private void PrintErrorCantAccessChannel(VertexAttribute ch);

        [FreeFunction(Name = "MeshScripting::HasChannel", HasExplicitThis = true)]
        extern public bool HasVertexAttribute(VertexAttribute attr);
        [FreeFunction(Name = "MeshScripting::GetChannelDimension", HasExplicitThis = true)]
        extern public int GetVertexAttributeDimension(VertexAttribute attr);
        [FreeFunction(Name = "MeshScripting::GetChannelFormat", HasExplicitThis = true)]
        extern public VertexAttributeFormat GetVertexAttributeFormat(VertexAttribute attr);
        [FreeFunction(Name = "MeshScripting::GetChannelStream", HasExplicitThis = true)]
        public extern int GetVertexAttributeStream(VertexAttribute attr);
        [FreeFunction(Name = "MeshScripting::GetChannelOffset", HasExplicitThis = true)]
        public extern int GetVertexAttributeOffset(VertexAttribute attr);

        [FreeFunction(Name = "SetMeshComponentFromArrayFromScript", HasExplicitThis = true)]
        extern private void SetArrayForChannelImpl(VertexAttribute channel, VertexAttributeFormat format, int dim, System.Array values, int arraySize, int valuesStart, int valuesCount, UnityEngine.Rendering.MeshUpdateFlags flags);

        [FreeFunction(Name = "SetMeshComponentFromNativeArrayFromScript", HasExplicitThis = true)]
        extern private void SetNativeArrayForChannelImpl(VertexAttribute channel, VertexAttributeFormat format, int dim, IntPtr values, int arraySize, int valuesStart, int valuesCount, UnityEngine.Rendering.MeshUpdateFlags flags);

        [FreeFunction(Name = "AllocExtractMeshComponentFromScript", HasExplicitThis = true)]
        extern private System.Array GetAllocArrayFromChannelImpl(VertexAttribute channel, VertexAttributeFormat format, int dim);

        [FreeFunction(Name = "ExtractMeshComponentFromScript", HasExplicitThis = true)]
        extern private void GetArrayFromChannelImpl(VertexAttribute channel, VertexAttributeFormat format, int dim, System.Array values);

        // access to native underlying graphics API resources (mostly for native code plugins)

        extern public int vertexBufferCount
        {
            [FreeFunction(Name = "MeshScripting::GetVertexBufferCount", HasExplicitThis = true)] get;
        }
        [FreeFunction(Name = "MeshScripting::GetVertexBufferStride", HasExplicitThis = true)]
        public extern int GetVertexBufferStride(int stream);

        [NativeThrows]
        [FreeFunction(Name = "MeshScripting::GetNativeVertexBufferPtr", HasExplicitThis = true)]
        extern public IntPtr GetNativeVertexBufferPtr(int index);

        [FreeFunction(Name = "MeshScripting::GetNativeIndexBufferPtr", HasExplicitThis = true)]
        extern public IntPtr GetNativeIndexBufferPtr();

        [FreeFunction(Name = "MeshScripting::GetVertexBufferPtr", HasExplicitThis = true, ThrowsException = true)]
        extern GraphicsBuffer GetVertexBufferImpl(int index);
        [FreeFunction(Name = "MeshScripting::GetIndexBufferPtr", HasExplicitThis = true, ThrowsException = true)]
        extern GraphicsBuffer GetIndexBufferImpl();
        [FreeFunction(Name = "MeshScripting::GetBoneWeightBufferPtr", HasExplicitThis = true, ThrowsException = true)]
        extern GraphicsBuffer GetBoneWeightBufferImpl(int bonesPerVertex);

        [FreeFunction(Name = "MeshScripting::GetBlendShapeBufferPtr", HasExplicitThis = true, ThrowsException = true)]
        extern GraphicsBuffer GetBlendShapeBufferImpl(int layout);

        public extern GraphicsBuffer.Target vertexBufferTarget { get; set; }
        public extern GraphicsBuffer.Target indexBufferTarget { get; set; }

        // blend shapes

        extern public int blendShapeCount {[NativeMethod(Name = "GetBlendShapeChannelCount")] get; }

        [FreeFunction(Name = "MeshScripting::ClearBlendShapes", HasExplicitThis = true)]
        extern public void ClearBlendShapes();

        [FreeFunction(Name = "MeshScripting::GetBlendShapeName", HasExplicitThis = true, ThrowsException = true)]
        extern public string GetBlendShapeName(int shapeIndex);

        [FreeFunction(Name = "MeshScripting::GetBlendShapeIndex", HasExplicitThis = true, ThrowsException = true)]
        extern public int GetBlendShapeIndex(string blendShapeName);

        [FreeFunction(Name = "MeshScripting::GetBlendShapeFrameCount", HasExplicitThis = true, ThrowsException = true)]
        extern public int GetBlendShapeFrameCount(int shapeIndex);

        [FreeFunction(Name = "MeshScripting::GetBlendShapeFrameWeight", HasExplicitThis = true, ThrowsException = true)]
        extern public float GetBlendShapeFrameWeight(int shapeIndex, int frameIndex);

        [FreeFunction(Name = "GetBlendShapeFrameVerticesFromScript", HasExplicitThis = true, ThrowsException = true)]
        extern public void GetBlendShapeFrameVertices(int shapeIndex, int frameIndex, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);

        [FreeFunction(Name = "AddBlendShapeFrameFromScript", HasExplicitThis = true, ThrowsException = true)]
        extern public void AddBlendShapeFrame(string shapeName, float frameWeight, ReadOnlySpan<Vector3> deltaVertices, ReadOnlySpan<Vector3> deltaNormals, ReadOnlySpan<Vector3> deltaTangents);
        public void AddBlendShapeFrame(string shapeName, float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents)
            => AddBlendShapeFrame(shapeName, frameWeight, new ReadOnlySpan<Vector3>(deltaVertices), new ReadOnlySpan<Vector3>(deltaNormals), new ReadOnlySpan<Vector3>(deltaTangents));

        [FreeFunction(Name = "MeshScripting::GetBlendShapeOffset", HasExplicitThis = true)]
        extern private BlendShape GetBlendShapeOffsetInternal(int index);

        // skinning

        [NativeMethod("HasBoneWeights")]
        extern private bool HasBoneWeights();
        [FreeFunction(Name = "MeshScripting::GetBoneWeights", HasExplicitThis = true)]
        extern private BoneWeight[] GetBoneWeightsImpl();
        [FreeFunction(Name = "MeshScripting::SetBoneWeights", HasExplicitThis = true)]
        extern private void SetBoneWeightsImpl(BoneWeight[] weights);

        public unsafe void SetBoneWeights(NativeArray<byte> bonesPerVertex, NativeArray<BoneWeight1> weights)
        {
            InternalSetBoneWeights((IntPtr)bonesPerVertex.GetUnsafeReadOnlyPtr(), bonesPerVertex.Length, (IntPtr)weights.GetUnsafeReadOnlyPtr(), weights.Length);
        }

        [System.Security.SecurityCritical] // to prevent accidentally making this public in the future
        [FreeFunction(Name = "MeshScripting::SetBoneWeights", HasExplicitThis = true)]
        extern private void InternalSetBoneWeights(IntPtr bonesPerVertex, int bonesPerVertexSize, IntPtr weights, int weightsSize);

        public unsafe NativeArray<BoneWeight1> GetAllBoneWeights()
        {
            var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<BoneWeight1>((void*)GetAllBoneWeightsArray(), GetAllBoneWeightsArraySize(), Allocator.None);
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, GetReadOnlySafetyHandle(SafetyHandleIndex.BonesWeightsArray));
            return array;
        }

        public unsafe NativeArray<byte> GetBonesPerVertex()
        {
            int size = HasBoneWeights() ? vertexCount : 0;
            var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)GetBonesPerVertexArray(), size, Allocator.None);
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, GetReadOnlySafetyHandle(SafetyHandleIndex.BonesPerVertexArray));
            return array;
        }

        [FreeFunction(Name = "MeshScripting::GetAllBoneWeightsArraySize", HasExplicitThis = true)]
        extern private int GetAllBoneWeightsArraySize();

        [NativeMethod("GetBoneWeightBufferDimension")]
        extern int GetBoneWeightBufferLayoutInternal();

        [System.Security.SecurityCritical] // to prevent accidentally making this public in the future
        [FreeFunction(Name = "MeshScripting::GetAllBoneWeightsArray", HasExplicitThis = true)]
        extern private IntPtr GetAllBoneWeightsArray();

        [System.Security.SecurityCritical] // to prevent accidentally making this public in the future
        [FreeFunction(Name = "MeshScripting::GetBonesPerVertexArray", HasExplicitThis = true)]
        extern private IntPtr GetBonesPerVertexArray();

        extern public int bindposeCount { get; }
        [NativeName("BindPosesFromScript")] extern public Matrix4x4[] bindposes { get; set; }

        public unsafe NativeArray<Matrix4x4> GetBindposes()
        {
            var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>((void*)GetBindposesArray(), bindposeCount, Allocator.None);
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, GetReadOnlySafetyHandle(SafetyHandleIndex.BindposeArray));
            return array;
        }

        public unsafe void SetBindposes(NativeArray<Matrix4x4> poses)
        {
            if (!poses.IsCreated || poses.Length == 0)
                throw new ArgumentException("Cannot set bindposes as the native poses array is empty.", "poses");

            SetBindposesFromScript_NativeArray((IntPtr)poses.GetUnsafeReadOnlyPtr(), poses.Length);
        }

        [NativeMethod("SetBindposes")]
        extern private void SetBindposesFromScript_NativeArray(IntPtr posesPtr, int posesCount);

        [System.Security.SecurityCritical] // to prevent accidentally making this public in the future
        [FreeFunction(Name = "MeshScripting::GetBindposesArray", HasExplicitThis = true)]
        extern private IntPtr GetBindposesArray();

        [FreeFunction(Name = "MeshScripting::ExtractBoneWeightsIntoArray", HasExplicitThis = true)]
        extern private void GetBoneWeightsNonAllocImpl([Out] BoneWeight[] values);

        [FreeFunction(Name = "MeshScripting::ExtractBindPosesIntoArray", HasExplicitThis = true)]
        extern private void GetBindposesNonAllocImpl([Out] Matrix4x4[] values);

        private enum SafetyHandleIndex
        {
            // Keep in sync with C++ Mesh::SafetyHandleIndex class
            BonesPerVertexArray,
            BonesWeightsArray,
            BindposeArray,
        }

        [FreeFunction(Name = "MeshScripting::GetReadOnlySafetyHandle", HasExplicitThis = true)]
        extern private AtomicSafetyHandle GetReadOnlySafetyHandle(SafetyHandleIndex index);

        // random things

        extern public   bool isReadable {[NativeMethod("GetIsReadable")] get; }
        extern internal bool canAccess  {[NativeMethod("CanAccessFromScript")] get; }

        extern public int vertexCount   {[NativeMethod("GetVertexCount")] get; }
        extern public int subMeshCount
        {
            [NativeMethod(Name = "GetSubMeshCount")] get;
            [FreeFunction(Name = "MeshScripting::SetSubMeshCount", HasExplicitThis = true)] set;
        }

        [FreeFunction("MeshScripting::SetSubMesh", HasExplicitThis = true, ThrowsException = true)]
        extern public void SetSubMesh(int index, SubMeshDescriptor desc, UnityEngine.Rendering.MeshUpdateFlags flags = UnityEngine.Rendering.MeshUpdateFlags.Default);
        [FreeFunction("MeshScripting::GetSubMesh", HasExplicitThis = true, ThrowsException = true)]
        extern public SubMeshDescriptor GetSubMesh(int index);

        [FreeFunction("MeshScripting::SetAllSubMeshesAtOnceFromArray", HasExplicitThis = true, ThrowsException = true)]
        extern private void SetAllSubMeshesAtOnceFromArray(SubMeshDescriptor[] desc, int start, int count, UnityEngine.Rendering.MeshUpdateFlags flags = UnityEngine.Rendering.MeshUpdateFlags.Default);
        [FreeFunction("MeshScripting::SetAllSubMeshesAtOnceFromNativeArray", HasExplicitThis = true, ThrowsException = true)]
        extern private void SetAllSubMeshesAtOnceFromNativeArray(IntPtr desc, int start, int count, UnityEngine.Rendering.MeshUpdateFlags flags = UnityEngine.Rendering.MeshUpdateFlags.Default);

        [FreeFunction("MeshScripting::SetLodCount", HasExplicitThis = true, ThrowsException = true)]
        extern private void SetLodCount(int numLevels);

        [FreeFunction("MeshScripting::SetLodSelectionCurve",  HasExplicitThis = true, ThrowsException = true)]
        extern private void SetLodSelectionCurve(LodSelectionCurve lodSelectionCurve);

        [FreeFunction("MeshScripting::SetLods", HasExplicitThis = true, ThrowsException = true)]
        extern private void SetLodsFromArray(MeshLodRange[] levelRanges, int start, int count, int submesh, MeshUpdateFlags flags = UnityEngine.Rendering.MeshUpdateFlags.Default);

        [FreeFunction("MeshScripting::SetLodsFromNativeArray", HasExplicitThis = true, ThrowsException = true)]
        extern private void SetLodsFromNativeArray(IntPtr lodLevels, int count, int submesh, MeshUpdateFlags flags = UnityEngine.Rendering.MeshUpdateFlags.Default);

        [FreeFunction("MeshScripting::SetLod", HasExplicitThis = true, ThrowsException = true)]
        extern private void SetLodImpl(int subMeshIndex, int level, MeshLodRange levelRange, MeshUpdateFlags flags = UnityEngine.Rendering.MeshUpdateFlags.Default);

        [FreeFunction("MeshScripting::GetLods", HasExplicitThis = true, ThrowsException = true)]
        extern private MeshLodRange[] GetLodsAlloc(int subMeshIndex);

        [FreeFunction(Name = "MeshScripting::GetLodsNonAlloc", HasExplicitThis = true, ThrowsException = true)]
        extern private void GetLodsNonAlloc([Out] MeshLodRange[] levels, int subMeshIndex);

        [FreeFunction("MeshScripting::GetLodCount", HasExplicitThis = true)]
        extern private int GetLodCount();

        [FreeFunction("MeshScripting::GetLodSelectionCurve", HasExplicitThis = true)]
        extern private LodSelectionCurve GetLodSelectionCurve();

        [FreeFunction("MeshScripting::GetLod", HasExplicitThis = true, ThrowsException = true)]
        extern public MeshLodRange GetLod(int subMeshIndex, int levelIndex);

        extern public Bounds bounds { get; set; }

        [NativeMethod("Clear")]                 extern private void ClearImpl(bool keepVertexLayout);
        [NativeMethod("RecalculateBounds")]     extern private void RecalculateBoundsImpl(UnityEngine.Rendering.MeshUpdateFlags flags);
        [NativeMethod("RecalculateNormals")]    extern private void RecalculateNormalsImpl(UnityEngine.Rendering.MeshUpdateFlags flags);
        [NativeMethod("RecalculateTangents")]   extern private void RecalculateTangentsImpl(UnityEngine.Rendering.MeshUpdateFlags flags);
        [NativeMethod("MarkDynamic")]           extern private void MarkDynamicImpl();
        [NativeMethod("MarkModified")]          extern public  void MarkModified();
        [NativeMethod("UploadMeshData")]        extern private void UploadMeshDataImpl(bool markNoLongerReadable);

        [FreeFunction(Name = "MeshScripting::GetPrimitiveType", HasExplicitThis = true)]
        extern private MeshTopology GetTopologyImpl(int submesh);

        [NativeMethod("RecalculateMeshMetric")] extern private void RecalculateUVDistributionMetricImpl(int uvSetIndex, float uvAreaThreshold);
        [NativeMethod("RecalculateMeshMetrics")] extern private void RecalculateUVDistributionMetricsImpl(float uvAreaThreshold);
        [NativeMethod("GetMeshMetric")] extern public float GetUVDistributionMetric(int uvSetIndex);

        [NativeMethod(Name = "MeshScripting::CombineMeshes", IsFreeFunction = true, ThrowsException = true, HasExplicitThis = true)]
        extern private void CombineMeshesImpl(CombineInstance[] combine, bool mergeSubMeshes, bool useMatrices, bool hasLightmapData);

        [NativeMethod("Optimize")]                    extern private void OptimizeImpl();
        [NativeMethod("OptimizeIndexBuffers")]        extern private void OptimizeIndexBuffersImpl();
        [NativeMethod("OptimizeReorderVertexBuffer")] extern private void OptimizeReorderVertexBufferImpl();
    }

    [NativeHeader("Runtime/Graphics/Mesh/StaticBatching.h")]
    internal struct StaticBatchingHelper
    {
        [FreeFunction("StaticBatching::CombineMeshesForStaticBatching")]
        extern internal static void CombineMeshes(GameObject[] gos, GameObject staticBatchRoot);
    }
}
