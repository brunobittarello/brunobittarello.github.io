using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Utilities.Debug;

namespace com.ootii.Input
{
    /// <summary>
    /// Wraps game input so that we can process it
    /// before applying it to game objects.
    /// </summary>
    public class InputManager
    {
        /// <summary>
        /// Create the stub at startup and tie it into the Unity update path
        /// </summary>
        public static InputManagerCore Core;

        /// <summary>
        /// Set by an external object, it tracks the angle of the
        /// user input compared to the camera's forward direction
        /// Note that this info isn't reliable as objects using it 
        /// before it's set it will get float.NaN.
        /// </summary>
        private static float mInputFromCameraAngle = 0f;
        public static float InputFromCameraAngle
        {
            get { return mInputFromCameraAngle; }
            set { mInputFromCameraAngle = value; }
        }

        /// <summary>
        /// Set by an external object, it tracks the angle of the
        /// user input compared to the avatars's forward direction
        /// Note that this info isn't reliable as objects using it 
        /// before it's set it will get float.NaN.
        /// </summary>
        private static float mInputFromAvatarAngle = 0f;
        public static float InputFromAvatarAngle
        {
            get { return mInputFromAvatarAngle; }
            set { mInputFromAvatarAngle = value; }
        }

        /// <summary>
        /// Determines how quickly the mouse movement updates
        /// </summary>
        private static float mMouseSensativity = 2f;
        public static float MouseSensativity
        {
            get { return mMouseSensativity; }
            set { mMouseSensativity = value; }
        }

        /// <summary>
        /// Determines if we're going to use the xbox controller or not
        /// </summary>
        public static bool UseXboxController
        {
            get { return mIsXboxControllerEnabled; }
            set { mIsXboxControllerEnabled = value; }
        }

        /// <summary>
        /// Matches the call in the Motion Controller's input manager
        /// </summary>
        private static bool mIsXboxControllerEnabled = false;
        public static bool IsXboxControllerEnabled
        {
            get { return mIsXboxControllerEnabled; }
            set { mIsXboxControllerEnabled = value; }
        }

        /// <summary>
        /// Key or button used to allow movement to be activated
        /// 0 = none
        /// 1 = left mouse button
        /// 2 = right mouse button
        /// </summary>
        private static int mMoveActivator = 0;
        public static int MoveActivator
        {
            get { return mMoveActivator; }
            set { mMoveActivator = value; }
        }

        /// <summary>
        /// Key or button used to allow view to be activated
        /// 0 = none
        /// 1 = left mouse button
        /// 2 = right mouse button
        /// </summary>
        private static int mViewActivator = 0;
        public static int ViewActivator
        {
            get { return mViewActivator; }
            set { mViewActivator = value; }
        }

        /// <summary>
        /// Keep track of the old values for smoothing
        /// </summary>
        private static float mViewX = 0f;
        private static float mViewY = 0f;

        //private static float mOldViewX = 0f;
        //private static float mOldViewY = 0f;

        private static float mTargetViewX = 0f;
        private static float mTargetViewY = 0f;

        private static float mVSyncTimer = 0f;

        /// <summary>
        /// Determines if it's time to change the player's stance
        /// </summary>
        /// <value><c>true</c> if is changing stance; otherwise, <c>false</c>.</value>
        public static bool IsChangingStance
        {
            get
            {
                // T
                return UnityEngine.Input.GetKeyDown(KeyCode.T);
            }
        }

        /// <summary>
        /// Determine if the player is activating the aiming feature
        /// </summary>
        /// <value><c>true</c> if is aiming; otherwise, <c>false</c>.</value>
        public static bool IsAiming
        {
            get
            {
                bool lIsAiming = false;

                if (mIsXboxControllerEnabled) { lIsAiming = (UnityEngine.Input.GetAxis("LeftTrigger") == 1); }
                if (!lIsAiming) { lIsAiming = UnityEngine.Input.GetMouseButton(1); }

                return lIsAiming;
            }
        }

        /// <summary>
        /// Determines if we're allows to free view with the mouse or right-stick
        /// </summary>
        /// <value><c>true</c> if is free viewing; otherwise, <c>false</c>.</value>
        public static bool IsFreeViewing
        {
            get { return true; }
        }

        /// <summary>
        /// Speed of movement in the range of -1 (full backwards) to 1 (full forward)
        /// </summary>
        /// <value>The speed in the range of -1 to 1</value>
        public static float Speed
        {
            get
            {
                float lMovementX = MovementX;
                float lMovementY = MovementY;
                return Mathf.Sqrt((lMovementX * lMovementX) + (lMovementY * lMovementY));
            }
        }

        /// <summary>
        /// Horizontal movement in the range of -1 (left) to 1 (right)
        /// </summary>
        /// <value>The movement in the range of -1 to 1</value>
        public static float MovementX
        {
            get
            {
                if ((mMoveActivator == 0) ||
                    (mMoveActivator == 1 && UnityEngine.Input.GetMouseButton(0)) ||
                    (mMoveActivator == 2 && UnityEngine.Input.GetMouseButton(1)))
                {
                    return UnityEngine.Input.GetAxis("Horizontal");
                }

                return 0f;
            }
        }

        /// <summary>
        /// Vertical movement in the range of -1 (down) to 1 (up)
        /// </summary>
        /// <value>The movement in the range of -1 to 1</value>
        public static float MovementY
        {
            get
            {
                if ((mMoveActivator == 0) ||
                    (mMoveActivator == 1 && UnityEngine.Input.GetMouseButton(0)) ||
                    (mMoveActivator == 2 && UnityEngine.Input.GetMouseButton(1)))
                {
                    return UnityEngine.Input.GetAxis("Vertical");
                }

                return 0f;
            }
        }

        /// <summary>
        /// Horizontal view change in the range of -1 (left) to 1 (right)
        /// </summary>
        /// <value>The view in the range of -1 to 1</value>
        public static float ViewX
        {
            get
            {
                //float lView = 0f;

                //if (mUseXboxController)
                //{
                //    lView = UnityEngine.Input.GetAxisRaw("RightStickX");
                //}

                //if (lView == 0f)
                //{
                //    lView = UnityEngine.Input.GetAxis("Mouse X") * mMouseSensativity;
                //    if (lView < -mMouseSensativity) { lView = -mMouseSensativity; }
                //    else if (lView > mMouseSensativity) { lView = mMouseSensativity; }
                //}

                //lView = Mathf.Lerp(mOldViewX, lView, 0.9f);
                //mOldViewX = lView;

                //return lView;

                if (mIsXboxControllerEnabled)
                {
                    float lViewX = UnityEngine.Input.GetAxisRaw("RightStickX");
                    if (lViewX != 0f) { return lViewX; }
                }

                if ((mViewActivator == 0) ||
                    (mViewActivator == 1 && UnityEngine.Input.GetMouseButton(0)) ||
                    (mViewActivator == 2 && UnityEngine.Input.GetMouseButton(1)))
                {
                    return mViewX;
                }

                return 0f;
            }
        }

        /// <summary>
        /// Vertical view change in the range of -1 (left) to 1 (right)
        /// </summary>
        /// <value>The view in the range of -1 to 1</value>
        public static float ViewY
        {
            get
            {
                //float lView = 0f;

                //if (mUseXboxController)
                //{
                //    lView = UnityEngine.Input.GetAxisRaw("RightStickY");
                //}

                //if (lView == 0f)
                //{
                //    lView = UnityEngine.Input.GetAxis("Mouse Y") * mMouseSensativity;
                //    if (lView < -mMouseSensativity) { lView = -mMouseSensativity; }
                //    else if (lView > mMouseSensativity) { lView = mMouseSensativity; }
                //}

                //lView = Mathf.Lerp(mOldViewY, lView, 0.9f);
                //mOldViewY = lView;

                //return lView;

                if (mIsXboxControllerEnabled)
                {
                    float lViewY = UnityEngine.Input.GetAxisRaw("RightStickY");
                    if (lViewY != 0f) { return lViewY; }
                }

                if ((mViewActivator == 0) ||
                    (mViewActivator == 1 && UnityEngine.Input.GetMouseButton(0)) ||
                    (mViewActivator == 2 && UnityEngine.Input.GetMouseButton(1)))
                {
                    return mViewY;
                }

                return 0f;
            }
        }

        /// <summary>
        /// Static constructor is called at most one time, before any 
        /// instance constructor is invoked or member is accessed. 
        /// </summary>
        static InputManager()
        {
            // Check to see if an input manager stub exists. If so, associate it.
            // if not, we'll need to create one.
            Core = Component.FindObjectOfType<InputManagerCore>();
            if (Core == null)
            {
#pragma warning disable 0414
                Core = (new GameObject("InputManagerCore")).AddComponent<InputManagerCore>();
                mIsXboxControllerEnabled = Core.IsXboxControllerEnabled;
#pragma warning restore 0414
            }
        }

        /// <summary>
        /// Set the initial values
        /// </summary>
        public static void Initialize()
        {
            if (Core != null)
            {
                mIsXboxControllerEnabled = Core.IsXboxControllerEnabled;
            }
        }

        /// <summary>
        /// Grab and process information from the input in one place. This
        /// allows us to calculated changes over time too.
        /// </summary>
        public static void Update()
        {
            float lViewX = 0f;
            float lViewY = 0f;
            float lMouseSensativity = mMouseSensativity;
            if (UnityEngine.QualitySettings.vSyncCount == 0) { lMouseSensativity = lMouseSensativity * 2f; }

            if ((mViewActivator == 0) ||
                (mViewActivator == 1 && UnityEngine.Input.GetMouseButton(0)) ||
                (mViewActivator == 2 && UnityEngine.Input.GetMouseButton(1)))
            {
                // If vsync is off, we get false 0's. Ignore them here
                lViewX = UnityEngine.Input.GetAxis("Mouse X") * lMouseSensativity;
                if (lViewX != 0)
                {
                    mTargetViewX = lViewX;
                    mVSyncTimer = 0f;
                }

                // If vsync is off, we get false 0's. Ignore them here
                lViewY = UnityEngine.Input.GetAxis("Mouse Y") * -lMouseSensativity;
                if (lViewY != 0)
                {
                    mTargetViewY = lViewY;
                    mVSyncTimer = 0f;
                }
            }

            // If vsync is off, and we get no mouse input, we 
            // run the timer that will eventually set the view to 0.
            if (lViewX == 0f && lViewY == 0f)
            {
                mVSyncTimer += Time.deltaTime;
                if (mVSyncTimer > Time.fixedDeltaTime)
                {
                    mTargetViewX = 0f;
                    mTargetViewY = 0f;
                    mVSyncTimer = 0f;
                }
            }

            // Set the actual view values
            mViewX = Mathf.Lerp(mViewX, mTargetViewX, 0.9f);
            mViewY = Mathf.Lerp(mViewY, mTargetViewY, 0.9f);
        }

        /// <summary>
        /// Test if a specific key is pressed
        /// </summary>
        /// <param name="rKey"></param>
        /// <returns></returns>
        public static bool IsPressed(KeyCode rKey)
        {
            return UnityEngine.Input.GetKey(rKey);
        }

        /// <summary>
        /// Test if a specific key is pressed
        /// </summary>
        /// <param name="rKey"></param>
        /// <returns></returns>
        public static bool IsJustPressed(KeyCode rKey)
        {
            return UnityEngine.Input.GetKeyUp(rKey);
        }

        /// <summary>
        /// Tests if the given input action has occured
        /// </summary>
        /// <param name="rAction"></param>
        /// <returns></returns>
        public static bool IsPressed(string rAction)
        {
            // Determines if the character should go into a first-person
            // perspective for targeting
            if (rAction == "Aiming")
            {
                bool lIsAiming = false;

                if (InputManager.IsXboxControllerEnabled) { lIsAiming = (UnityEngine.Input.GetAxis("LeftTrigger") == 1); }
                if (!lIsAiming) { lIsAiming = UnityEngine.Input.GetMouseButton(1); }

                return lIsAiming;
            }

            return false;
        }

        /// <summary>
        /// Tests if the given input action has occured
        /// </summary>
        /// <param name="rAction"></param>
        /// <returns></returns>
        public static bool IsJustPressed(string rAction)
        {
            if (rAction == "ChangeStance")
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.T)) { return true; }
            }
            else if (rAction == "Aiming")
            {
                bool lIsAiming = false;

                if (InputManager.IsXboxControllerEnabled) { lIsAiming = (UnityEngine.Input.GetAxis("LeftTrigger") == 1); }
                if (!lIsAiming) { lIsAiming = UnityEngine.Input.GetMouseButton(1); }

                return lIsAiming;
            }

            return false;
        }
    }
}

