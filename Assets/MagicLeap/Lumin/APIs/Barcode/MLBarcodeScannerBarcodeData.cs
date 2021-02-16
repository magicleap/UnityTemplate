// %BANNER_BEGIN% 
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLBarcodeScannerBarcodeData.cs" company="Magic Leap">
//      Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// </copyright>
// %COPYRIGHT_END% 
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLBarcodeScanner
    {
        /// <summary>
        ///     Represents the different barcode types supported by the API
        /// </summary>
        [Flags]
        public enum BarcodeType
        {
            /// <summary>
            ///     Represents no barcode
            /// </summary>
            None = 0,

            /// <summary>
            ///     QR code of Model 1 or 2
            /// </summary>
            QR = 1,

            /// <summary>
            ///     All supported barcodes
            /// </summary>
            All = 0x3FFFFFFF
        }

        /// <summary>
        ///     Barcode data as returned by the Magic Leap SDK
        /// </summary>
        public struct BarcodeData
        {
            /// <summary>
            ///     The world-space position and rotation of the barcode.
            /// </summary>
            public Pose Pose;

            /// <summary>
            ///     The length of the raw data.
            /// </summary>
            public uint Length;

            /// <summary> 
            ///     The pointer to the address of the raw data in memory. To use this, you must translate  
            ///     into whatever format you are expecting, 
            ///     e.g. <c>Marshal.PtrToStringAuto(DataPointer, Length)</c>. 
            /// </summary>
            public IntPtr DataPointer;

            /// <summary>
            ///     The barcode data represented as a string.
            /// </summary>
            public string StringData
            {
                get
                {
                    if (stringData == default)
                        stringData = Marshal.PtrToStringAuto(DataPointer, (int)Length);
                    return stringData;
                }
            }

            private string stringData;

            /// <summary>
            ///     The reprojection error of this QR code detection in degrees.
            ///
            ///     The reprojection error is only useful when tracking QR codes. A high
            ///     reprojection error means that the estimated pose of the QR code doesn't match
            ///     well with the 2D detection on the processed video frame and thus the pose might
            ///     be inaccurate. The error is given in degrees, signifying by how much either
            ///     camera or QR code would have to be moved or rotated to create a perfect
            ///     reprojection. The further away your QR code is, the smaller this reprojection
            ///     error will be for the same displacement error of the code.
            /// </summary>
            public float ReprojectionError;

            /// <summary>
            ///     The type of barcode that was detected.
            /// </summary>
            public BarcodeType Type;

            internal static BarcodeData Create(BarcodeType type, Pose pose, IntPtr data, uint length, float reprojError) =>
                new BarcodeData()
                { 
                    Type = type,
                    Pose = pose,
                    DataPointer = data,
                    Length = length,
                    ReprojectionError = reprojError,
                };

            public override string ToString() =>
                $"\nType: {Enum.GetName(typeof(MLBarcodeScanner.BarcodeType), Type)}\nReprojection Error: {ReprojectionError}\nBarcode Data (string): {StringData}\nBarcode Data (pointer): {DataPointer.ToInt64()}\nData Length: {Length}";

        }
    }
}
