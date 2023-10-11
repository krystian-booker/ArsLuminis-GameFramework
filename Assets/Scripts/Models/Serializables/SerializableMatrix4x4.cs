using UnityEngine;

namespace Assets.Scripts.Models.Serializables
{
    [System.Serializable]
    public class SerializableMatrix4x4
    {
        public float m00, m01, m02, m03;
        public float m10, m11, m12, m13;
        public float m20, m21, m22, m23;
        public float m30, m31, m32, m33;

        // Constructor for converting Matrix4x4 to SerializableMatrix4x4
        public SerializableMatrix4x4(Matrix4x4 matrix)
        {
            m00 = matrix.m00;
            m01 = matrix.m01;
            m02 = matrix.m02;
            m03 = matrix.m03;

            m10 = matrix.m10;
            m11 = matrix.m11;
            m12 = matrix.m12;
            m13 = matrix.m13;

            m20 = matrix.m20;
            m21 = matrix.m21;
            m22 = matrix.m22;
            m23 = matrix.m23;

            m30 = matrix.m30;
            m31 = matrix.m31;
            m32 = matrix.m32;
            m33 = matrix.m33;
        }

        // Implicit conversion from SerializableMatrix4x4 to Matrix4x4
        public static implicit operator Matrix4x4(SerializableMatrix4x4 serializableMatrix)
        {
            return new Matrix4x4(
                new Vector4(serializableMatrix.m00, serializableMatrix.m01, serializableMatrix.m02, serializableMatrix.m03),
                new Vector4(serializableMatrix.m10, serializableMatrix.m11, serializableMatrix.m12, serializableMatrix.m13),
                new Vector4(serializableMatrix.m20, serializableMatrix.m21, serializableMatrix.m22, serializableMatrix.m23),
                new Vector4(serializableMatrix.m30, serializableMatrix.m31, serializableMatrix.m32, serializableMatrix.m33)
            );
        }

        // Implicit conversion from Matrix4x4 to SerializableMatrix4x4
        public static implicit operator SerializableMatrix4x4(Matrix4x4 matrix)
        {
            return new SerializableMatrix4x4(matrix);
        }
    }
}
