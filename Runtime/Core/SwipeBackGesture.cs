#region Using
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
#endregion

namespace NG.UINavigationSystem
{
    /// <summary>
    /// Edge swipe gesture to go back. It is updated by <see cref="UIPrefabListController"/> on iOS only.
    /// Swiping from the left edge to the right, or from the right edge to the left, works same as the device back key.
    /// It goes back on releasing the finger, if the finger moved enough or flicked.
    /// Edges can be enabled or disabled from <see cref="Configurations.SwipeBack"/>.
    /// </summary>
    internal class SwipeBackGesture
    {
        #region Constant Variables
        /// <summary>
        /// Distance in points the finger has to move from the edge to start the swipe.
        /// </summary>
        private const float START_DISTANCE = 10f;

        /// <summary>
        /// Distance as a fraction of the screen width, after which releasing the finger goes back.
        /// </summary>
        private const float COMPLETE_DISTANCE = 0.25f;

        /// <summary>
        /// Speed in points per second, after which releasing the finger is treated as a flick and goes back.
        /// </summary>
        private const float FLICK_SPEED = 400f;

        /// <summary>
        /// If the finger doesn't move for this time in seconds, its speed is treated as zero.
        /// </summary>
        private const float VELOCITY_TIMEOUT = 0.1f;

        /// <summary>
        /// Points per inch on iOS. It is used to convert points to pixels.
        /// </summary>
        private const float POINTS_PER_INCH = 163f;
        #endregion


        #region Enums
        /// <summary>
        /// States of the swipe gesture.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// Waiting for a touch on the edge.
            /// </summary>
            Idle,

            /// <summary>
            /// Touch started on the edge, waiting for it to move enough to start the swipe.
            /// </summary>
            Tracking,

            /// <summary>
            /// Swipe started, waiting for the finger to be released.
            /// </summary>
            Swiping
        }
        #endregion


        #region Variables
        /// <summary>
        /// Current state of the swipe gesture.
        /// </summary>
        private State state = State.Idle;

        /// <summary>
        /// Finger id of the touch of the swipe. It is used with the old Input Manager only.
        /// </summary>
        private int fingerId;

        /// <summary>
        /// Position in pixels where the touch started.
        /// </summary>
        private Vector2 startPosition;

        /// <summary>
        /// 1 for the left edge swipe which moves to the right, -1 for the right edge swipe which moves to the left.
        /// </summary>
        private int direction;

        /// <summary>
        /// Horizontal speed of the finger in pixels per second.
        /// </summary>
        private float velocity;

        /// <summary>
        /// Last horizontal position of the finger, used to calculate the speed.
        /// </summary>
        private float lastX;

        /// <summary>
        /// Last time the finger moved, used to calculate the speed.
        /// </summary>
        private float lastMoveTime;

        /// <summary>
        /// Invisible full screen blocker on top of all UIs during the swipe. It stops the touch from clicking the button under it on release.
        /// </summary>
        private GameObject blocker;
        #endregion


        #region Properties
        /// <summary>
        /// Pixels per point of this device.
        /// </summary>
        private static float PixelsPerPoint => UnityEngine.Screen.dpi > 0f ? UnityEngine.Screen.dpi / POINTS_PER_INCH : 1f;
        #endregion


        #region Internal Methods
        /// <summary>
        /// Updates the swipe gesture. It should be called every frame.
        /// </summary>
        /// <returns>Whether the swipe is completed in this frame and should go back</returns>
        internal bool Update()
        {
            switch (state)
            {
                case State.Tracking:
                    UpdateTracking();
                    return false;

                case State.Swiping:
                    return UpdateSwiping();

                default:
                    UpdateIdle();
                    return false;
            }
        }

        /// <summary>
        /// Destroys the objects created by the swipe gesture.
        /// </summary>
        internal void Dispose()
        {
            if (blocker != null)
                Object.Destroy(blocker);
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Waits for a touch on the enabled edges.
        /// </summary>
        private void UpdateIdle()
        {
            // Blocker is removed one frame after the swipe ends, so the UI processes the release of the touch while it's still blocked
            SetBlockerActive(false);

            bool allowLeftEdge = Configurations.SwipeBack.AllowLeftEdge;
            bool allowRightEdge = Configurations.SwipeBack.AllowRightEdge;
            if (!allowLeftEdge && !allowRightEdge)
                return;

            if (!ReadTouch(out Vector2 position, out bool began, out _, out _) || !began)
                return;

            float edgeWidth = Configurations.SwipeBack.EdgeWidth * PixelsPerPoint;
            if (allowLeftEdge && position.x <= edgeWidth)
                direction = 1;
            else if (allowRightEdge && position.x >= UnityEngine.Screen.width - edgeWidth)
                direction = -1;
            else
                return;

            startPosition = position;
            state = State.Tracking;
        }

        /// <summary>
        /// Waits for the touch to move enough from the edge to start the swipe.
        /// </summary>
        private void UpdateTracking()
        {
            if (!ReadTouch(out Vector2 position, out _, out bool ended, out _) || ended)
            {
                state = State.Idle;
                return;
            }

            Vector2 delta = position - startPosition;
            if (delta.magnitude < START_DISTANCE * PixelsPerPoint)
                return;

            // Swipe has to be horizontal and away from the edge, otherwise the touch is left for the UI, e.g. vertical scroll
            if (delta.x * direction <= Mathf.Abs(delta.y))
            {
                state = State.Idle;
                return;
            }

            // Block the UI input, so the touch doesn't click the button under it on release
            SetBlockerActive(true);

            lastX = position.x;
            lastMoveTime = Time.unscaledTime;
            velocity = 0f;
            state = State.Swiping;
        }

        /// <summary>
        /// Waits for the finger to be released, and checks if the swipe is completed.
        /// </summary>
        /// <returns>Whether the swipe is completed and should go back</returns>
        private bool UpdateSwiping()
        {
            bool hasTouch = ReadTouch(out Vector2 position, out _, out bool ended, out bool canceled);
            if (hasTouch)
                UpdateVelocity(position.x);

            if (hasTouch && !ended)
                return false;

            // Touch is released. Blocker is removed on the next frame, see UpdateIdle().
            state = State.Idle;
            return hasTouch && !canceled && IsCompleted(position.x);
        }

        /// <summary>
        /// Updates the horizontal speed of the finger.
        /// </summary>
        /// <param name="x">Horizontal position of the finger in pixels</param>
        private void UpdateVelocity(float x)
        {
            float time = Time.unscaledTime;
            if (x != lastX)
            {
                float deltaTime = time - lastMoveTime;
                if (deltaTime > 0f)
                    velocity = Mathf.Lerp(velocity, (x - lastX) / deltaTime, 0.5f);

                lastX = x;
                lastMoveTime = time;
            }
            else if (time - lastMoveTime > VELOCITY_TIMEOUT)
            {
                // Finger stopped
                velocity = 0f;
            }
        }

        /// <summary>
        /// Checks if the swipe is completed on releasing the finger. It's completed if the finger moved enough or flicked.
        /// </summary>
        /// <param name="x">Horizontal position of the finger in pixels, where it's released</param>
        /// <returns>Whether the swipe is completed</returns>
        private bool IsCompleted(float x)
        {
            float distance = (x - startPosition.x) * direction;
            float speed = velocity * direction / PixelsPerPoint;
            return speed > FLICK_SPEED || (distance > COMPLETE_DISTANCE * UnityEngine.Screen.width && speed > -FLICK_SPEED);
        }

        /// <summary>
        /// Reads the touch of the swipe. On Idle, it reads a new touch.
        /// </summary>
        /// <param name="position">Position of the touch in pixels</param>
        /// <param name="began">Whether the touch began in this frame</param>
        /// <param name="ended">Whether the touch ended or was canceled in this frame</param>
        /// <param name="canceled">Whether the touch was canceled in this frame, e.g. by the system</param>
        /// <returns>Whether the touch is found</returns>
        private bool ReadTouch(out Vector2 position, out bool began, out bool ended, out bool canceled)
        {
#if ENABLE_INPUT_SYSTEM
            UnityEngine.InputSystem.Touchscreen touchscreen = UnityEngine.InputSystem.Touchscreen.current;
            if (touchscreen != null)
            {
                UnityEngine.InputSystem.Controls.TouchControl touch = touchscreen.primaryTouch;
                if (touch.press.isPressed || touch.press.wasReleasedThisFrame)
                {
                    position = touch.position.ReadValue();
                    began = touch.press.wasPressedThisFrame;
                    ended = touch.press.wasReleasedThisFrame;
                    canceled = ended && touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled;
                    return true;
                }
            }
#else
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                // On Idle, look for a new touch. Otherwise, look for the touch of the swipe.
                if (state == State.Idle ? touch.phase != TouchPhase.Began : touch.fingerId != fingerId)
                    continue;

                fingerId = touch.fingerId;
                position = touch.position;
                began = touch.phase == TouchPhase.Began;
                ended = touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
                canceled = touch.phase == TouchPhase.Canceled;
                return true;
            }
#endif

            position = default;
            began = false;
            ended = false;
            canceled = false;
            return false;
        }

        /// <summary>
        /// Shows or hides the invisible blocker on top of all UIs.
        /// </summary>
        /// <param name="active">Whether to show the blocker</param>
        private void SetBlockerActive(bool active)
        {
            if (blocker == null)
            {
                if (!active)
                    return;

                blocker = new GameObject("[UINavigationSystem] Swipe Back Blocker", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster), typeof(Image));

                Canvas canvas = blocker.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = short.MaxValue;

                // Invisible, but still receives raycasts
                Image image = blocker.GetComponent<Image>();
                image.color = Color.clear;
                image.canvasRenderer.cullTransparentMesh = false;
            }

            if (blocker.activeSelf != active)
                blocker.SetActive(active);
        }
        #endregion
    }
}