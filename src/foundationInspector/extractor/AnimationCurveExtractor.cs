﻿using UnityEngine;
using UnityEditor;


namespace foundationEditor
{
    public class AnimationCurveExtractor : EditorWindow
    {
        public void Init(SerializedProperty inTargetProperty)
        {
            //keep the iterator in its current state...
            _PopupTargetAnimationCurveProperty = inTargetProperty;
        }

        void OnGUI()
        {
            AnimationClip anim = EditorGUILayout.ObjectField("Source Animation", _SourceAnimationClip, typeof(AnimationClip), false) as AnimationClip;

            if (anim != _SourceAnimationClip)
            {
                _SourceAnimationClip = anim;
                if (anim != null)
                {
                    _Curves = AnimationUtility.GetCurveBindings(anim);
                    _SelectedCurveIndex = 0;
                    int curveCount = _Curves.Length;
                    _CurveNames = new string[curveCount];
                    for (int i = 0; i < curveCount; ++i)
                    {
                        if (_Curves[i].path == "")
                        {
                            _CurveNames[i] = _Curves[i].propertyName;
                        }
                        else
                        {
                            _CurveNames[i] = _Curves[i].path + "/" + _Curves[i].propertyName;
                        }
                    }
                }
                else
                {
                    _Curves = null;
                    _CurveNames = null;
                }
            }

            if (_Curves != null && _Curves.Length > 0)
            {
                _SelectedCurveIndex = EditorGUILayout.Popup("Source Curve", _SelectedCurveIndex, _CurveNames);

                EditorGUILayout.CurveField("Data", AnimationUtility.GetEditorCurve(_SourceAnimationClip, _Curves[_SelectedCurveIndex]));

                if (GUILayout.Button("Extract!"))
                {
                    Extract();
                    Close();
                }
            }

        }

        private void Extract()
        {
            AnimationCurve sourceCurve = AnimationUtility.GetEditorCurve(_SourceAnimationClip, _Curves[_SelectedCurveIndex]);

            _PopupTargetAnimationCurveProperty.animationCurveValue = AnimationCurveCopier.CreateCopy(sourceCurve);
            _PopupTargetAnimationCurveProperty.serializedObject.ApplyModifiedProperties();
        }

        private SerializedProperty _PopupTargetAnimationCurveProperty;

        private AnimationClip _SourceAnimationClip;

        private EditorCurveBinding[] _Curves;

        private string[] _CurveNames;
        private int _SelectedCurveIndex;
    }


    public static class AnimationCurveCopier
    {
        public static void Copy(AnimationCurve inSource, AnimationCurve inDest)
        {
            inDest.keys = inSource.keys;
            inDest.preWrapMode = inSource.preWrapMode;
            inDest.postWrapMode = inSource.postWrapMode;
        }

        public static AnimationCurve CreateCopy(AnimationCurve inSource)
        {
            AnimationCurve newCurve = new AnimationCurve();
            Copy(inSource, newCurve);
            return newCurve;
        }
    }

}