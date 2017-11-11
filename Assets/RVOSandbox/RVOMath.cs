using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RVOMath{
    internal const float RVO_EPSILON = 0.00001f;

    public static float abs(Vector3 vector){
		return vector.magnitude;
    }

    public static float absSq(Vector3 vector){
		return vector.sqrMagnitude;
    }

    public static Vector3 normalize(Vector3 vector){
		return vector.normalized;
    }

    internal static float det(Vector3 vector1, Vector3 vector2){
        return vector1.x * vector2.z - vector1.z * vector2.x;
    }

    internal static float distSqPointLineSegment(Vector3 vector1, Vector3 vector2, Vector3 vector3){
		float r = Vector3.Dot((vector3 - vector1),(vector2 - vector1)) / absSq(vector2 - vector1);

        if(r < 0.0f){
            return absSq(vector3 - vector1);
        }

        if(r > 1.0f){
            return absSq(vector3 - vector2);
        }

        return absSq(vector3 - (vector1 + r * (vector2 - vector1)));
    }

    public static float fabs(float scalar){
        return Math.Abs(scalar);
    }

    internal static float leftOf(Vector3 a, Vector3 b, Vector3 c){
        return det(a-c, b-a);
    }

    internal static float sqr(float scalar){
        return scalar * scalar;
    }

    internal static float sqrt(float scalar){
        return (float)Math.Sqrt(scalar);
    }



}