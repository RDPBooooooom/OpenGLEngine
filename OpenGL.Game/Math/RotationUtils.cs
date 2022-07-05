namespace OpenGL.Game.Math
{
    
    /// <summary>
    /// Copied and adapted from https://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine (http://pastebin.com/riRLRvch)
    /// </summary>
    public static class RotationUtils
    {

        public static Quaternion ToQ(Vector3 v)
        {
            return ToQ(v.Y, v.X, v.Z);
        }

        public static Vector3 FromQ(Quaternion q2)
        {
            Quaternion q = new Quaternion(q2.W, q2.Z, q2.X, q2.Y);
            Vector3 pitchYawRoll;
            pitchYawRoll.Y =
                (float) System.Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (q.Z * q.Z + q.W * q.W)); // Yaw
            pitchYawRoll.X = (float) System.Math.Asin(2f * (q.X * q.Z - q.W * q.Y)); // Pitch
            pitchYawRoll.Z =
                (float) System.Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (q.Y * q.Y + q.Z * q.Z)); // Roll
            return new Vector3(Mathf.ToDeg(pitchYawRoll.X), Mathf.ToDeg(pitchYawRoll.Y), Mathf.ToDeg(pitchYawRoll.Z));
        }

        public static Quaternion ToQ(float yaw, float pitch, float roll)
        {
            yaw = Mathf.ToRad(yaw);
            pitch = Mathf.ToRad(pitch);
            roll = Mathf.ToRad(roll);
            float rollOver2 = roll * 0.5f;
            float sinRollOver2 = Mathf.Sin(rollOver2);
            float cosRollOver2 = Mathf.Cos(rollOver2);
            float pitchOver2 = pitch * 0.5f;
            float sinPitchOver2 = Mathf.Sin(pitchOver2);
            float cosPitchOver2 = Mathf.Cos(pitchOver2);
            float yawOver2 = yaw * 0.5f;
            float sinYawOver2 = Mathf.Sin(yawOver2);
            float cosYawOver2 = Mathf.Cos(yawOver2);
            Quaternion result;
            result.W = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.X = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.Y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
            result.Z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

            return result;
        }

        public static Vector3 FromQ2(Quaternion q1)
        {
            float sqw = q1.W * q1.W;
            float sqx = q1.X * q1.X;
            float sqy = q1.Y * q1.Y;
            float sqz = q1.Z * q1.Z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = q1.X * q1.Y + q1.Z * q1.W;
            Vector3 v;
            if (test > 0.4999 * unit)
            {
                // singularity at north pole
                v.Y = (float) (2 * System.Math.Atan2(q1.X, q1.W));
                v.X = Mathf.PI / 2;
                v.Z = 0;
                return v * Mathf.TO_DEG_CONST;
            }

            if (test < -0.4999 * unit)
            {
                // singularity at south pole
                v.Y = (float) (-2 * System.Math.Atan2(q1.X, q1.W));
                v.X = -Mathf.PI / 2;
                v.Z = 0;
                return v * Mathf.TO_DEG_CONST;
            }

            Quaternion q = new Quaternion(q1.W, q1.Z, q1.X, q1.Y);
            v.Y = (float) System.Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (q.Z * q.Z + q.W * q.W)); // Yaw
            v.X = (float) System.Math.Asin(2f * (q.X * q.Z - q.W * q.Y)); // Pitch
            v.Z = (float) System.Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (q.Y * q.Y + q.Z * q.Z)); // Roll
            return v * Mathf.TO_DEG_CONST;
        }
    }
}
