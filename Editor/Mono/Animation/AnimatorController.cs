// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using RequiredByNativeCodeAttribute = UnityEngine.Scripting.RequiredByNativeCodeAttribute;

namespace UnityEditor.Animations
{
    [NativeClass(null)]
    public sealed partial class AnimatorController : RuntimeAnimatorController
    {
        internal System.Action OnAnimatorControllerDirty;

        internal static AnimatorController lastActiveController = null;
        internal  static int lastActiveLayerIndex = 0;
        private const string kControllerExtension = "controller";

        internal PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);
        internal bool pushUndo { set { undoHandler.pushUndo = value; } }

        internal string GetDefaultBlendTreeParameter()
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].type == AnimatorControllerParameterType.Float)
                    return parameters[i].name;
            }
            AddParameter("Blend", AnimatorControllerParameterType.Float);
            return "Blend";
        }

        [RequiredByNativeCode]
        internal static void OnInvalidateAnimatorController(AnimatorController controller)
        {
            if (controller.OnAnimatorControllerDirty != null)
                controller.OnAnimatorControllerDirty();
        }

        internal AnimatorStateMachine FindEffectiveRootStateMachine(int layerIndex)
        {
            AnimatorControllerLayer currentLayer = layers[layerIndex];
            while (currentLayer.syncedLayerIndex != -1)
            {
                currentLayer = layers[currentLayer.syncedLayerIndex];
            }
            return currentLayer.stateMachine;
        }

        public void AddLayer(string name)
        {
            AnimatorControllerLayer newLayer = new AnimatorControllerLayer();
            newLayer.name = MakeUniqueLayerName(name);
            newLayer.stateMachine = new AnimatorStateMachine();
            newLayer.stateMachine.name = newLayer.name;
            newLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;

            if (AssetDatabase.GetAssetPath(this) != "")
                AssetDatabase.AddObjectToAsset(newLayer.stateMachine, AssetDatabase.GetAssetPath(this));

            undoHandler.DoUndoCreated(newLayer.stateMachine, "Layer added");

            AddLayer(newLayer);
        }

        public void AddLayer(AnimatorControllerLayer layer)
        {
            undoHandler.DoUndo(this, "Layer added");

            AnimatorControllerLayer[] layerVector = layers;
            ArrayUtility.Add(ref layerVector, layer);
            layers = layerVector;
        }

        internal void RemoveLayers(List<int> layerIndexes)
        {
            undoHandler.DoUndo(this, "Layers removed");

            AnimatorControllerLayer[] layerVector = this.layers;
            foreach (var layerIndex in layerIndexes)
            {
                for (var i = 0; i < layerVector.Length; ++i)
                {
                    var syncedLayerIndex = layerVector[i].syncedLayerIndex;
                    if (syncedLayerIndex > layerIndex)
                    {
                        // synced layer is after the layer being removed, so it's going to be shifted upon removal
                        layerVector[i].syncedLayerIndex = syncedLayerIndex - 1;
                    }
                }

                RemoveLayerInternal(layerIndex, ref layerVector);
            }
            this.layers = layerVector;
        }

        private void RemoveLayerInternal(int index, ref AnimatorControllerLayer[] layerVector)
        {
            if (layerVector[index].syncedLayerIndex == -1 && layerVector[index].stateMachine != null)
            {
                undoHandler.DoUndo(layerVector[index].stateMachine, "Layer removed");
                layerVector[index].stateMachine.Clear();
                if (MecanimUtilities.AreSameAsset(this, layerVector[index].stateMachine))
                    Undo.DestroyObjectImmediate(layerVector[index].stateMachine);
            }

            ArrayUtility.Remove(ref layerVector, layerVector[index]);
        }

        public void RemoveLayer(int index)
        {
            undoHandler.DoUndo(this, "Layer removed");

            AnimatorControllerLayer[] layerVector = layers;
            RemoveLayerInternal(index, ref layerVector);
            layers = layerVector;
        }

        internal bool IsStateInLayer(AnimatorState state, int layerIndex)
        {
            if (layerIndex >= layers.Length)
            {
                return false;
            }

            return layers[layerIndex].stateMachine.HasState(state);
        }

        internal int GetStateLayer(AnimatorState state)
        {
            for (int i = 0; i < layers.Length; ++i)
            {
                if (layers[i].stateMachine.HasState(state))
                {
                    return i;
                }
            }

            return -1;
        }

        public void AddParameter(string name, AnimatorControllerParameterType type)
        {
            AnimatorControllerParameter newParameter = new AnimatorControllerParameter();
            newParameter.name = MakeUniqueParameterName(name);
            newParameter.type = type;

            AddParameter(newParameter);
        }

        public void AddParameter(AnimatorControllerParameter paramater)
        {
            undoHandler.DoUndo(this, "Parameter added");
            AnimatorControllerParameter[] parameterVector = parameters;
            ArrayUtility.Add(ref parameterVector, paramater);
            parameters = parameterVector;
        }

        public void RemoveParameter(int index)
        {
            undoHandler.DoUndo(this, "Parameter removed");
            AnimatorControllerParameter[] parameterVector = parameters;
            ArrayUtility.Remove(ref parameterVector, parameterVector[index]);
            parameters = parameterVector;
        }

        public void RemoveParameter(AnimatorControllerParameter parameter)
        {
            undoHandler.DoUndo(this, "Parameter removed");
            AnimatorControllerParameter[] parameterVector = parameters;
            ArrayUtility.Remove(ref parameterVector, parameter);
            parameters = parameterVector;
        }

        // We cannot call AddMotion from native code, because there are multiple signatures of that method, creating ambiguity.
        [RequiredByNativeCode]
        private AnimatorState AddMotionInternal(Motion motion)
        {
            return AddMotion(motion);
        }

        public AnimatorState AddMotion(Motion motion)
        {
            return AddMotion(motion, 0);
        }

        public AnimatorState AddMotion(Motion motion, int layerIndex)
        {
            AnimatorState state = layers[layerIndex].stateMachine.AddState(motion.name);
            state.motion = motion;
            return state;
        }

        public AnimatorState CreateBlendTreeInController(string name, out BlendTree tree)
        {
            return CreateBlendTreeInController(name, out tree, 0);
        }

        public AnimatorState CreateBlendTreeInController(string name, out BlendTree tree, int layerIndex)
        {
            tree = new BlendTree();
            tree.name = name;
            tree.blendParameter = tree.blendParameterY = GetDefaultBlendTreeParameter();

            if (AssetDatabase.GetAssetPath(this) != "")
                AssetDatabase.AddObjectToAsset(tree, AssetDatabase.GetAssetPath(this));

            undoHandler.DoUndoCreated(tree, "Blend Tree Created");

            AnimatorState state = layers[layerIndex].stateMachine.AddState(tree.name);
            state.motion = tree;
            return state;
        }

        public static AnimatorController CreateAnimatorControllerAtPath(string path)
        {
            AnimatorController controller = new AnimatorController();

            controller.name = Path.GetFileName(path);
            AssetDatabase.CreateAsset(controller, path);

            controller.pushUndo = false;
            controller.AddLayer("Base Layer");
            controller.pushUndo = true;

            return controller;
        }

        public static AnimationClip AllocateAnimatorClip(string name)
        {
            var clip = UnityEditorInternal.AnimationWindowUtility.AllocateAndSetupClip(true);
            clip.name = name;
            return clip;
        }

        [RequiredByNativeCode]
        internal static AnimatorController CreateAnimatorControllerForClip(AnimationClip clip, GameObject animatedObject)
        {
            string path = AssetDatabase.GetAssetPath(clip);

            if (string.IsNullOrEmpty(path))
                return null;

            string name = System.Text.RegularExpressions.Regex.Replace(animatedObject.name, @"[\\\./]", "_");

            path = Path.Combine(FileUtil.DeleteLastPathNameComponent(path), name + "." + kControllerExtension);
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            if (string.IsNullOrEmpty(path))
                return null;


            return CreateAnimatorControllerAtPathWithClip(path, clip);
        }

        public static AnimatorController CreateAnimatorControllerAtPathWithClip(string path, AnimationClip clip)
        {
            AnimatorController controller = CreateAnimatorControllerAtPath(path);

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            stateMachine.pushUndo = false;
            var state = stateMachine.AddState(clip.name);
            state.motion = clip;
            stateMachine.pushUndo = true;

            return controller;
        }

        public void SetStateEffectiveMotion(AnimatorState state, Motion motion)
        {
            SetStateEffectiveMotion(state, motion, 0);
        }

        public void SetStateEffectiveMotion(AnimatorState state, Motion motion, int layerIndex)
        {
            //delete existing nested blend tree asset
            Motion selectedMotion = GetStateEffectiveMotion(state, layerIndex);
            BlendTree blendTree = selectedMotion as BlendTree;

            if (blendTree != null && !AssetDatabase.IsMainAsset(blendTree))
            {
                MecanimUtilities.DestroyBlendTreeRecursive(blendTree);
            }

            if (layers[layerIndex].syncedLayerIndex == -1)
            {
                undoHandler.DoUndo(state, "Set Motion");
                state.motion = motion;
            }
            else
            {
                undoHandler.DoUndo(this, "Set Motion");
                AnimatorControllerLayer[] layerArray = layers;
                layerArray[layerIndex].SetOverrideMotion(state, motion);
                layers = layerArray;
            }
        }

        public Motion GetStateEffectiveMotion(AnimatorState state)
        {
            return GetStateEffectiveMotion(state, 0);
        }

        public Motion GetStateEffectiveMotion(AnimatorState state, int layerIndex)
        {
            if (layerIndex < 0 || layerIndex > layers.Length)
                return null;

            return layers[layerIndex].syncedLayerIndex == -1 ? state.motion : layers[layerIndex].GetOverrideMotion(state);
        }

        public void SetStateEffectiveBehaviours(AnimatorState state, int layerIndex, StateMachineBehaviour[] behaviours)
        {
            if (layers[layerIndex].syncedLayerIndex == -1)
            {
                undoHandler.DoUndo(state, "Set Behaviours");
                state.behaviours = behaviours;
            }
            else
            {
                undoHandler.DoUndo(this, "Set Behaviours");
                Internal_SetEffectiveBehaviours(state, layerIndex, behaviours);
            }
        }

        public StateMachineBehaviour[] GetStateEffectiveBehaviours(AnimatorState state, int layerIndex)
        {
            if (layerIndex < 0 || layerIndex > layers.Length)
                return Array.Empty<StateMachineBehaviour>();

            return Internal_GetEffectiveBehaviours(state, layerIndex) as StateMachineBehaviour[];
        }

        [System.Obsolete("parameterCount is obsolete. Use parameters.Length instead.", true)]
        int parameterCount
        {
            get { return 0; }
        }

        [System.Obsolete("layerCount is obsolete. Use layers.Length instead.", true)]
        int layerCount
        {
            get { return 0; }
        }
    }
}
