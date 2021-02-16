// %BANNER_BEGIN% 
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLBarcodeScannerSettings.cs" company="Magic Leap">
//      Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// </copyright>
// %COPYRIGHT_END% 
// ---------------------------------------------------------------------
// %BANNER_END%

using System;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLBarcodeScanner
    {
        [Serializable]
        public struct Settings
        {
            /// <summary>
            ///     If <c> true </c>, Barcode Scanner will detect barcodes and track QR codes.
            ///     Barcode Scanner should be disabled when app is paused and enabled when app
            ///     resumes. When enabled, Barcode Scanner will gain access to the camera and start
            ///     scanning barcodes. When disabled Barcode Scanner will release the camera and
            ///     stop scanning barcodes. Internal state of the scanner will be maintained.
            /// </summary>
            public bool EnableBarcodeScanning;

            /// <summary>
            ///     The physical size of the QR code that shall be tracked. The physical size is
            ///     important to know, because once a QR code is detected we can only determine its
            ///     3D position when we know its correct size. The size of the QR code is given in
            ///     meters and represents the length of one side of the square code(without the
            ///     outer margin). Min size: As a rule of thumb the size of a QR code should be at
            ///     least a 10th of the distance you intend to scan it with a camera device. Higher
            ///     version barcodes with higher information density might need to be larger than
            ///     that to be detected reliably. Max size: Our camera needs to see the whole
            ///     barcode at once. If it's too large, we won't detect it.
            /// </summary>
            public float QRCodeSize;

            /// <summary>
            ///     The barcode types that are enabled for this scanner. Enable barcodes by
            ///     combining any number of <c> BarcodeType </c> flags using '|' (bitwise 'or').
            /// </summary>
            public BarcodeType ScanTypes;

            public static Settings Create(bool enableBarcodeScanning = true, BarcodeType barcodeType = BarcodeType.All, float qRCodeSize = .1f) =>
                new Settings()
                {
                    EnableBarcodeScanning = enableBarcodeScanning,
                    ScanTypes = barcodeType,
                    QRCodeSize = qRCodeSize
                };
        }
    }
}
