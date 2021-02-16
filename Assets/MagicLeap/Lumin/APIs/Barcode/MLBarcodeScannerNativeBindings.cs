// %BANNER_BEGIN% 
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLBarcodeScannerNativeBindings.cs" company="Magic Leap">
//      Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// </copyright>
// %COPYRIGHT_END% 
// ---------------------------------------------------------------------
// %BANNER_END%

#if PLATFORM_LUMIN

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using UnityEngine.XR.MagicLeap.Native;


    public sealed partial class MLBarcodeScanner
    {
        private static bool hasShownPrivError = false;

        /// <summary>
        ///     Creates the MLBarcodeScanner API instance on the native side. 
        ///     This must be called before any other native call.
        /// </summary>
        /// <param name="settings"> The initial settings for the barcode scanner to use. </param>
        /// <returns> MLResult indicating the success or failure of the operation. </returns>
        /// <exception cref="System.DllNotFoundException" />
        /// <exception cref="System.EntryPointNotFoundException" />
        private static MLResult.Code MLBarcodeScannerCreate(Settings settings)
        {
            var nativeSettings = NativeBindings.MLBarcodeScannerSettings.Create(settings);
            MLResult.Code resultCode = NativeBindings.MLBarcodeScannerCreate(nativeSettings, out Instance.Handle);
            return resultCode;
        }

        /// <summary>
        ///     Poll the native API for barcode scanner results.
        /// </summary>
        /// <returns> 
        ///     An array of BarcodeData that contains the results
        ///     that the scanner has collected since the last call to this function. This array
        ///     may be empty if there are no new results.
        /// </returns>
        private static List<BarcodeData> MLBarcodeScannerGetResults()
        {          
            try
            {
                // get results from native api
                MLResult.Code resultCode = NativeBindings.MLBarcodeScannerGetResult(Instance.Handle, out NativeBindings.MLBarcodeScannerResultArray scannerResults);

                if (MLResult.IsOK(resultCode))
                {
                    var managedResults = new List<BarcodeData>((int) scannerResults.Count);

                    for (int i = 0; i < scannerResults.Count; i++)
                    {
                        // marshal native array into native structs
                        long address = scannerResults.Detections.ToInt64() + (Marshal.SizeOf<IntPtr>() * i);
                        NativeBindings.MLBarcodeScannerResult detectedResult = Marshal.PtrToStructure<NativeBindings.MLBarcodeScannerResult>(Marshal.ReadIntPtr(new IntPtr(address)));
                        MLPluginLog.Debug($"MLBarcodeScanner results found: {detectedResult}");

                        // create managed version of data
                        UnityEngine.Pose pose;
                        if (((BarcodeType)detectedResult.Type) == BarcodeType.QR)
                        {
                            if (!MagicLeapNativeBindings.UnityMagicLeap_TryGetPose(detectedResult.CoordinateFrameUID, out pose))
                            {
                                MLPluginLog.Error($"Barcode Scanner could not get pose data for coordinate frame id '{detectedResult.CoordinateFrameUID}'");
                                pose = Pose.identity;
                            }
                        }
                        else
                            pose = Pose.identity;

                        managedResults.Add
                        (
                            BarcodeData.Create
                            (
                                (BarcodeType)detectedResult.Type, 
                                pose, 
                                detectedResult.DecodedData.Data, 
                                detectedResult.DecodedData.Size, 
                                detectedResult.ReprojectionError
                            )
                        );
                    }

                    // release native memory so results can be polled again
                    if (MLResult.IsOK(NativeBindings.MLBarcodeScannerReleaseResult(scannerResults)))
                        return managedResults;
                    else
                    {
                        MLPluginLog.Error($"MLBarcodeScanner.NativeBindings.MLBarcodeScannerReleaseResult failed when trying to release the results' memory. Reason: {MLResult.CodeToString(resultCode)}");
                        return managedResults;
                    }
                }
                else
                {
                    MLPluginLog.Error($"MLBarcodeScanner.MLBarcodeScannerGetResult failed to obtain a result. Reason: {resultCode}");
                    return default;
                }
            }
            catch (EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLBarcodeScanner.MLBarcodeScannerGetResult failed. Reason: API symbols not found.");
            }

            return default;
        }

        /// <summary>
        ///     Updates the current settings used by the native barcode scanner API.
        /// </summary>
        /// <returns> MLResult indicating the success or failure of the operation. </returns>
        private static async Task<MLResult> MLBarcodeScannerSettingsUpdate(Settings settings)
        {
            try
            {
                if (settings.EnableBarcodeScanning)
                {
                    MLResult privilegeResult = await MLPrivileges.RequestPrivilegeAsync(MLPrivileges.Id.CameraCapture);
                    if (privilegeResult == MLResult.Code.PrivilegeNotGranted)
                    {
                        if (!hasShownPrivError)
                        { 
                            MLPluginLog.Warning($"User denied CameraCapture permission. {nameof(MLBarcodeScanner)} will remain disabled.");
                            hasShownPrivError = true;
                        }
                        return privilegeResult;
                    }
                    else if(privilegeResult != MLResult.Code.PrivilegeGranted)
                    {
                        MLPluginLog.Error($"Error while requesting priviliges for {nameof(MLBarcodeScanner)}.\nError: {privilegeResult}");
                        return privilegeResult;
                    }
                }

                var nativeSettings = NativeBindings.MLBarcodeScannerSettings.Create(settings);
                MLResult createSettingsResult = MLResult.Create(NativeBindings.MLBarcodeScannerUpdateSettings(Instance.Handle, nativeSettings));
                if (!createSettingsResult.IsOk)
                    MLPluginLog.ErrorFormat("MLBarcodeScanner.MLBarcodeScannerUpdateSettings failed to update scanner settings. Reason: {0}", createSettingsResult);

                return createSettingsResult;
            }
            catch (EntryPointNotFoundException)
            {
                string error = "MLBarcodeScanner.MLBarcodeScannerUpdateSettings failed. Reason: API symbols not found.";
                MLPluginLog.Error(error);
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, error);
            }
        }

        private class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            ///     Create a Barcode Scanner. Requires CameraCapture, LowLatencyLightwear priveledges.
            /// </summary>
            /// <param name="settings">
            ///     List of settings of type <c> MLBarcodeScannerSettings </c> that configure the scanner.
            /// </param>
            /// <param name="handle">
            ///     A pointer to an <c> MLHandle </c> to the newly created Barcode Scanner. If this
            ///     operation fails, handle will be <c> ML_INVALID_HANDLE </c>.
            /// </param>
            /// <returns>
            ///     <c> MLResult_InvalidParam </c>: Failed to create Barcode Scanner due to invalid
            ///     out_handle. <c> MLResult_Ok Successfully </c>: created Barcode Scanner. <c>
            ///     MLResult_PrivilegeDenied Failed </c>: to create scanner due to lack of
            ///     privilege(s). <c> MLResult_UnspecifiedFailure </c>: Failed to create the Barcode
            ///     Scanner due to an internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLBarcodeScannerCreate(in MLBarcodeScannerSettings settings, out ulong handle);

            /// <summary>
            ///     Destroy a Barcode Scanner. Requires CameraCapture, LowLatencyLightwear priveleges.
            /// </summary>
            /// <param name="scannerHandle"> MLHandle to the Barcode Scanner created by MLBarcodeScannerCreate(). </param>
            /// <returns>
            ///     <c> MLResult_Ok </c>: Successfully destroyed the Barcode Scanner.\n <c>
            ///     MLResult_UnspecifiedFailure </c>: Failed to destroy the scanner due to an
            ///     internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLBarcodeScannerDestroy(ulong scannerHandle);

            /// <summary>
            ///     brief Get the results for Barcode Scanning.= This function can be used to poll
            ///     results from the scanner. This will allocate memory for the results array that
            ///     will have to be freed later.
            /// </summary>
            /// <param name="scanner_handle">
            ///     <c> MLHandle </c> to the Barcode Scanner created by MLBarcodeScannerCreate().
            /// </param>
            /// <param name="data">
            ///     out_data Pointer to an array of pointers to MLBarcodeScannerResult. The content
            ///     will be freed by the MLBarcodeScannerReleaseResult.
            /// </param>
            /// <returns>
            ///     MLResult_InvalidParam Failed to return detection data due to invalid out_data.
            /// </returns>
            /// <returns> MLResult_Ok Successfully fetched and returned all detections. </returns>
            /// \retval MLResult_UnspecifiedFailure Failed to return detections due to an internal error.
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLBarcodeScannerGetResult(ulong scanner_handle, out MLBarcodeScannerResultArray data);

            /// <summary>
            ///     Release the resources for the results array.
            /// </summary>
            /// <param name="data">The list of detections to be freed.</param>
            /// <returns>
            ///     MLResult_InvaldParam Failed to free structure due to invalid data.
            ///     MLResult_Ok Successfully freed data structure.
            ///     MLResult_UnspecifiedFailure Failed to free data due to an internal error.
            /// </returns>
            ///
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLBarcodeScannerReleaseResult(MLBarcodeScannerResultArray data);

            /// <summary>
            ///     Update the Barcode Scanner with new settings. Requires CameraCapture,
            ///     LowLatencyLightwear priveledges.
            /// </summary>
            /// <param name="scanner_handle"> MLHandle to the Barcode Scanner created by MLArucoScannerCreate(). </param>
            /// <param name="scanner_settings"> List of new Barcode Scanner settings. </param>
            /// <returns>
            ///     <c> MLResult_InvalidParam </c>: Failed to update the settings due to invalid
            ///     scanner_settings. <c> MLResult_Ok Successfully </c>: updated the Barcode Scanner
            ///     settings. <c> MLResult_PrivilegeDenied </c>: Failed to update the settings due
            ///     to lack of privilege(s). <c> MLResult_UnspecifiedFailure </c>: Failed to update
            ///     the settings due to an internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLBarcodeScannerUpdateSettings(ulong scanner_handle,
                                                                 in MLBarcodeScannerSettings scanner_settings);

            /// <summary>
            ///     Initializes default values for MLBarcodeScannerResultArray.
            /// </summary>
            /// <param name="result"> The object to initialize as default result array. </param>
            /// <returns>
            ///     MLResult_InvalidParam Failed to init result array due to pointer being NULL.
            ///     MLResult_Ok Successfully initialized Barcode result array.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLBarcodeScannerResultArrayInit(ref MLBarcodeScannerResultArray result);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLBarcodeScannerSettingsInit(ref MLBarcodeScannerSettings inout_settings);

            /// <summary>
            ///     Represents the decoded data encoded in the barcode. Barcodes can encode binary
            ///     data, strings, URLs and more. This struct represents the decoded data read from
            ///     a barcode.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLBarcodeScannerDecodedData
            {
                /// <summary>
                ///     Data array decoded from a detected barcode. 
                /// </summary>
                public IntPtr Data;

                /// <summary>
                ///     Length of the decoded data.
                /// </summary>
                public uint Size;

                public override string ToString() => $"Data: {Marshal.PtrToStringAuto(Data, (int) Size)}\nSize: {Size}";
            }

            /// <summary>
            ///     A list of these results will be returned by the Barcode Scanner, after
            ///     processing a video frame succesfully.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLBarcodeScannerResult
            {
                /// <summary>
                ///     The data which was encoded in the barcode.
                /// </summary>
                public MLBarcodeScannerDecodedData DecodedData;

                /// <summary>
                ///     The type of barcode that was detected.
                /// </summary>
                public uint Type;

                /// <summary>
                ///     MLCoordinateFrameUID of the QR code. This FrameUID is only useful if the
                ///     barcode is of type #MLBarcodeTypeQR This should be passed to the
                ///     MLSnapshotGetTransform() function to get the 6 DOF pose of the QR code. Any
                ///     barcode that isn't a QR code will have an invalid FrameUID here.
                /// </summary>
                public MLCoordinateFrameUID CoordinateFrameUID;

                /// <summary>
                ///     The reprojection error of this QR code detection in degrees.
                ///
                ///     The reprojection error is only useful when tracking QR codes. A high
                ///     reprojection error means that the estimated pose of the QR code doesn't
                ///     match well with the 2D detection on the processed video frame and thus the
                ///     pose might be inaccurate. The error is given in degrees, signifying by how
                ///     much either camera or QR code would have to be moved or rotated to create a
                ///     perfect reprojection. The further away your QR code is, the smaller this
                ///     reprojection error will be for the same displacement error of the code.
                /// </summary>
                public float ReprojectionError;

                public override string ToString() => $"{DecodedData}\nType: {Enum.GetName(typeof(BarcodeType), Type)}\nCoordFrameID: {CoordinateFrameUID}\nReproj Error: {ReprojectionError}"; 
            }

            /// <summary>
            ///     An array of all the detection results from the barcode scanning.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLBarcodeScannerResultArray
            {
                public uint Version;

                /// <summary>
                ///     Pointer to an array of pointers for MLBarcodeResult.
                /// </summary>
                public IntPtr Detections;

                /// <summary>
                ///     Number of barcodes being tracked.
                /// </summary>
                public uint Count;
            }

            /// <summary>
            ///     When creating the Barcode Scanner, this list of settings needs to be passed to
            ///     configure the scanner properly.The estimated poses will only be correct if the
            ///     barcode length has been set correctly.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLBarcodeScannerSettings
            {
                public uint Version;

                /// <summary>
                ///     The physical size of the QR code that shall be tracked.
                ///
                ///     The physical size is important to know, because once a QR code is detected
                ///     we can only determine its 3D position when we know its correct size. The
                ///     size of the QR code is given in meters and represents the length of one side
                ///     of the square code(without the outer margin).
                ///
                ///     Min size: As a rule of thumb the size of a QR code should be at least a 10th
                ///     of the distance you intend to scan it with a camera device. Higher version
                ///     barcodes with higher information density might need to be larger than that
                ///     to be detected reliably.
                ///
                ///     Max size: Our camera needs to see the whole barcode at once. If it's too
                ///     large, we won't detect it.
                /// </summary>
                public float QRCodeSize;

                /// <summary>
                ///     If <c> true </c>, Barcode Scanner will detect barcodes and track QR codes.
                ///     Barcode Scanner should be disabled when app is paused and enabled when app
                ///     resumes. When enabled, Barcode Scanner will gain access to the camera and
                ///     start scanning barcodes. When disabled Barcode Scanner will release the
                ///     camera and stop scanning barcodes. Internal state of the scanner will be maintained.
                /// </summary>
                public bool EnableBarcodeScanning;

                /// <summary>
                ///     The barcode types that are enabled for this scanner. Enable barcodes by
                ///     combining any number of MLBarcodeType flags using '|' (bitwise 'or').
                /// </summary>
                public uint ScanTypes;

                /// <summary>
                ///     Sets the native structures from the user facing properties.
                /// </summary>
                public static MLBarcodeScannerSettings Create(Settings settings) =>
                new MLBarcodeScannerSettings
                {
                    Version = 1,
                    QRCodeSize = settings.QRCodeSize,
                    EnableBarcodeScanning = settings.EnableBarcodeScanning,
                    ScanTypes = (uint) settings.ScanTypes
                };
            }
        }
    }
}

#endif
