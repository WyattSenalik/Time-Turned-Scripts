using System.Collections.Generic;
using UnityEngine;

using Helpers.Extensions;
// Original Authors - Wyatt

namespace Helpers
{
    /// <summary>
    /// Handles creating a dotted line to preview a path.
    /// </summary>
    public class DottedLine
    {
        // Constants
        // If the dots are closer than this, they are culled from the line
        private const float END_POS_TOO_CLOSE_SQR_DIST = 2.0f;

        // Active dots that are currently in use
        private Stack<DotVisual> m_activeVisuals = new Stack<DotVisual>();
        // Dots that are not currently being used and are hidden
        private Stack<DotVisual> m_inactiveVisuals = new Stack<DotVisual>();

        // Visual for the end of the line
        private GameObject m_lineEndVisual = null;
        // Prefab for spawning new dots
        private GameObject m_dotPrefab = null;
        // Parent of all the dots
        private Transform m_dotParent = null;

        // If the dotted line is active or inactive (renderers on or off)
        private bool m_isActive = false;

        private int m_layer = 0;
        /// <summary>
        /// Constructs and initializes the dotted line.
        /// Spawns a new gameobject to serve as the parent of the dots.
        /// </summary>
        /// <param name="endObj">Instance of the visual
        /// to display at the end of the line. NOT a prefab.</param>
        /// <param name="dotPrefab">Prefab to spawn the dots based on.</param>
        public DottedLine(GameObject endObj, GameObject dotPrefab, int layer)
        {
#if UNITY_EDITOR
            // If it is a prefab, print an error
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(endObj))
            {
                Debug.LogError($"{GetType().Name}'s Constructor was " +
                    $"given an {nameof(endObj)} that was a prefab and not " +
                    $"an instance of an object, which is expected.");
            }
#endif
            m_lineEndVisual = endObj;
            m_lineEndVisual.SetLayerRecursively(layer);

            m_dotPrefab = dotPrefab;
            m_dotParent = new GameObject("Dot Parent").transform;
            m_dotParent.gameObject.layer = layer;
            m_layer = layer;
        }
        /// <summary>
        /// Changes the line to display dots along the given points with the specified spacing.
        /// </summary>
        /// <param name="points">Path to display the dots along.</param>
        /// <param name="spacing">Distance to space the dots from each other.</param>
        public void CreateLine(Vector3[] points, float spacing, float yOffset = 0.1f)
        {
            // Add the y offset to the points.
            for (int i = 0; i < points.Length; ++i)
            {
                points[i] += new Vector3(0, yOffset, 0);
            }

            // Confirm that there are points
            if (points.Length < 1)
            {
                return;
            }

            // Show all the dots if they have turned off
            SetDotsActive(true);
            // Deactivate all the previous dots so they can be reused
            DeactivateAllDots();
            // Create a beginning dot at the start
            ActivateDot(points[0]);
            // Create the majority of the dots from the path
            ActivateDotsFromPath(points, spacing);
            // Potentially remove some of the ending points if they are too close to the final point
            RemoveLastDotsIfCloseToEnd(points[points.Length - 1]);

            // Move the ending visual to the end
            m_lineEndVisual.SetActive(true);
            m_lineEndVisual.transform.position = points[points.Length - 1];
        }
        /// <summary>
        /// Turn off the renderers for the line.
        /// </summary>
        public void HideLine()
        {
            m_lineEndVisual.SetActive(false);
            DeactivateAllDots();
            // Turn off the renderers for all the dots to save performance later
            SetDotsActive(false);
        }
        /// <summary>
        /// Has each point look at then next point on the line
        /// </summary>
        public void Point()
        {
            if (m_activeVisuals.Count <= 0) { return; }

            DotVisual above = null;
            // Foreach iterates from top to bottom in a stack
            foreach (DotVisual cur in m_activeVisuals)
            {
                // Skip the first one since there is no previous
                if (above == null)
                {
                    above = cur;
                    continue;
                }
                cur.LookAt(above.GetPosition());
                above = cur;
            }
            // Have the top one look at the end
            DotVisual top = m_activeVisuals.Peek();
            top.LookAt(m_lineEndVisual.transform.position);
        }

        /// <summary>
        /// Helper function for CreateLine. Activates dots along the given path.
        /// </summary>
        /// <param name="path">Path to display the dots along.</param>
        /// <param name="spacing">Distance to space the dots from each other.</param>
        private void ActivateDotsFromPath(Vector3[] path, float spacing)
        {
            // Keep track of how much distance we have travelled from the last dot
            float curDist = 0.0f;
            for (int i = 1; i < path.Length; ++i)
            {
                // Get the current and last point
                Vector3 lastPoint = path[i - 1];
                Vector3 curPoint = path[i];
                // Calculate some variables from the difference between the two points
                Vector3 difference = curPoint - lastPoint;
                float distance = difference.magnitude;
                Vector3 direction = difference.normalized;
                // Find out the distance needed to increase the curDist to be equal to spacing
                float distToNextDot = spacing - curDist;
                // Increment the current distance
                curDist += distance;
                // If the current distance is larger than the spacing, we need to keep placing dots
                // until that distance is shorter than the desired spacing
                while (curDist >= spacing)
                {
                    // Place the next dot the distance to the next dot away from the last point
                    Vector3 newDotPos = lastPoint + distToNextDot * direction;
                    ActivateDot(newDotPos);
                    curDist -= spacing;
                    // Update the last point to be where the last dot was
                    // so that the next dot isn't place ontop of the last dot
                    lastPoint = newDotPos;
                    // Change the distance to the next dot to be spacing so that
                    // the next dot is placed an accurate amount away
                    distToNextDot = spacing;
                }
            }
        }
        /// <summary>
        /// Hides dots if they are too close to the end point.
        /// </summary>
        /// <param name="endPoint">Last point in the path.</param>
        private void RemoveLastDotsIfCloseToEnd(Vector3 endPoint)
        {
            Vector3 difference = Vector3.zero;
            float sqrDist = difference.sqrMagnitude;
            // Remove points until we reach the end of the points or until
            // we find a point that is far enough away from the end
            while (m_activeVisuals.Count > 0 && sqrDist < END_POS_TOO_CLOSE_SQR_DIST)
            {
                DotVisual lastDot = m_activeVisuals.Peek();
                difference = endPoint - lastDot.GetPosition();
                sqrDist = difference.sqrMagnitude;

                if (sqrDist < END_POS_TOO_CLOSE_SQR_DIST)
                {
                    DeactivateLastDot();
                }
            }
        }
        /// <summary>
        /// Activates a dot and places it at the given position.
        /// If there are no more inactive dots to activate, it instantiates a new dot.
        /// </summary>
        /// <param name="pos">Position to place the next dot at.</param>
        private void ActivateDot(Vector3 pos)
        {
            if (m_inactiveVisuals.Count < 1)
            {
                m_inactiveVisuals.Push(CreateNewDot());
            }

            DotVisual dotToActivate = m_inactiveVisuals.Pop();
            dotToActivate.ActivateVisual(pos);
            m_activeVisuals.Push(dotToActivate);
        }
        /// <summary>
        /// Deactivates all dots.
        /// </summary>
        private void DeactivateAllDots()
        {
            while (m_activeVisuals.Count > 0)
            {
                DeactivateLastDot();
            }
        }
        /// <summary>
        /// Deactivates the last dot in the active visuals and adds it to the inactive visuals.
        /// </summary>
        private void DeactivateLastDot()
        {
            DotVisual dotToDeactivate = m_activeVisuals.Pop();
            dotToDeactivate.DeactivateVisual();
            m_inactiveVisuals.Push(dotToDeactivate);
        }
        /// <summary>
        /// Instantiates a new dot and returns it.
        /// </summary>
        /// <returns>Instantiated dot.</returns>
        private DotVisual CreateNewDot()
        {
            GameObject dotObj = Object.Instantiate(m_dotPrefab, m_dotParent);
            dotObj.layer = m_layer;
            SetLayers(dotObj);
            return new DotVisual(dotObj);
        }

        private void SetLayers(GameObject go)
        {
            foreach (Transform t in go.transform)
            {
                t.gameObject.layer = m_layer;
                SetLayers(t.gameObject);
            }
        }
        /// <summary>
        /// Toggles if the visuals of each dot are active or inactive.
        /// </summary>
        /// <param name="value">True - enables the visuals. False - disables the visuals.</param>
        private void SetDotsActive(bool value)
        {
            if (m_isActive != value)
            {
                foreach (DotVisual dot in m_inactiveVisuals)
                {
                    dot.SetActive(value);
                }

                m_isActive = value;
            }
        }


        /// <summary>
        /// Represents a dot that is being displayed in the dotted line.
        /// </summary>
        class DotVisual
        {
            // Constants
            // Position to place the dot when it is deactivated.
            private static readonly Vector3 DEACTIVATED_POSITION = new Vector3(0, -100, 0);


            // Reference to the game object of the dot
            private GameObject m_gameObject = null;


            /// <summary>
            /// Constructs a dot with the given object as the dot.
            /// </summary>
            /// <param name="obj">GameObject that is the visual dot in the world.</param>
            public DotVisual(GameObject obj)
            {
                m_gameObject = obj;
            }
            /// <summary>
            /// Constructs a dot with the given object at the given position.
            /// </summary>
            /// <param name="obj">GameObject that is the visual dot in the world.</param>
            /// <param name="pos">Position to place the dot at.</param>
            public DotVisual(GameObject obj, Vector3 pos) : this(obj)
            {
                ActivateVisual(pos);
            }


            /// <summary>
            /// Activates the dot by moving it to the given position.
            /// </summary>
            /// <param name="pos">Position to place the dot at.</param>
            public void ActivateVisual(Vector3 pos)
            {
                m_gameObject.transform.position = pos;
            }
            /// <summary>
            /// Deactivates the dot by moving it to an unseeable position.
            /// </summary>
            public void DeactivateVisual()
            {
                m_gameObject.transform.position = DEACTIVATED_POSITION;
            }
            /// <summary>
            /// Gets the position of the dot in the world.
            /// </summary>
            /// <returns>World Position of the dot.</returns>
            public Vector3 GetPosition()
            {
                return m_gameObject.transform.position;
            }
            /// <summary>
            /// Sets the gameobject visuals of the dot active/inactive.
            /// </summary>
            /// <param name="value">True - activate. False - deactivate.</param>
            public void SetActive(bool value)
            {
                m_gameObject.SetActive(value);
            }

            public void LookAt(Vector3 target)
            {
                m_gameObject.transform.LookAt(target);
            }

            public void SetRotation(Quaternion target)
            {
                m_gameObject.transform.rotation = target;
            }
            public Quaternion GetRotation()
            {
                return m_gameObject.transform.rotation;
            }
        }
    }
}
