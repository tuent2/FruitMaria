﻿using UnityEngine;

namespace dotmob.Scripts.System.Orientation
{
    /// <summary>
    /// Holds game panels for appropriate orientation
    /// </summary>
    public class OrientationPanels : MonoBehaviour
    {

        public RectTransform[] panels;
        public Transform movesTransform;

        private void OnEnable()
        {
        }
    }
}
