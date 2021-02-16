// %BANNER_BEGIN% 
// --------------------------------------------------------------------- 
// %COPYRIGHT_BEGIN%
// <copyright file="MLBarcodeScanner.cs" company="Magic Leap">
//      Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// </copyright>
// %COPYRIGHT_END% 
// --------------------------------------------------------------------- 
// %BANNER_END%

#if PLATFORM_LUMIN

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    ///     This API can be used to scan barcodes. For QR codes it also provides 6DOF poses. The
    ///     scanner supports up to 16 barcodes. Identical barcodes will be treated as seperate
    ///     barcodes and reported individually. List of currently supported barcodes:
    ///     - QR codes of Model 1
    ///     - QR codes of Model 2
    /// </summary>
    public sealed partial class MLBarcodeScanner : MLAutoAPISingleton<MLBarcodeScanner>
    {
        /// <summary>
        ///     When any results are found from barcode scanning, this event will be raised.
        /// </summary>
        public static event Action<BarcodeData> OnMLBarcodeScannerResultsFound;

        /// <summary>
        ///     A cache of the last requested settings value. Any new requested value will be
        ///     checked against this to verify that an update is needed.
        /// </summary>
        private static Settings futureSettingsValue;

        /// <summary>
        ///     Instance.settings setter.
        ///     If called with the same value while a settings update operation is in progress,
        ///     nothing will happen.
        /// </summary>
        public static async Task SetSettingsAsync(Settings value)
        {
            if (futureSettingsValue.Equals(value))
                return;

            futureSettingsValue = value;

            if (MLResult.IsOK((await MLBarcodeScannerSettingsUpdate(value)).Result))
                Instance.settings = value;
        }

        private static bool IsScanning => Instance.settings.EnableBarcodeScanning;

        private MLBarcodeScanner.Settings settings = Settings.Create(false, BarcodeType.All);
        
        /// <summary>
        ///     Asynchronous utility method to enable barcode scanning using the current <c> ScannerSettings </c>. 
        ///     Does nothing if scanning is already enabled.
        ///     Note that enabling scanning has a performance cost until scanning is disabled using 
        ///     <c> StopScanning </c> or by setting <c> ScannerSettings.enabled </c> to <c>false</c>.
        /// </summary>
        public static async Task StartScanningAsync(Settings? settings = null)
        {
            if (settings == null)
                settings = Instance.settings;

            if (settings.Value.EnableBarcodeScanning == true)
                return;

            await SetSettingsAsync(Settings.Create(true, settings.Value.ScanTypes, settings.Value.QRCodeSize));
        }

        /// <summary>
        ///     Asynchronous method to disable barcode scanning if previously activated. 
        ///     Otherwise, this does nothing.
        /// </summary>
        public async static Task StopScanningAsync()
        {
            // check future settings instead of current settings because this is asynchronous
            if (!IsStarted || futureSettingsValue.EnableBarcodeScanning == false)
                return;

            await SetSettingsAsync(Settings.Create(false, Instance.settings.ScanTypes, Instance.settings.QRCodeSize));
        }

        protected override MLResult.Code StopAPI()
        {
            if (IsScanning)
                Task.Run(StopScanningAsync).Wait();

            return NativeBindings.MLBarcodeScannerDestroy(Handle);
        }

        protected override MLResult.Code StartAPI() => MLBarcodeScannerCreate(Instance.settings);

        /// <summary>
        ///     Runs once per Unity Update loop.
        /// </summary>
        protected override void Update()
        {
            if (IsScanning)
            {
                foreach (BarcodeData data in MLBarcodeScannerGetResults())
                {
                    if (data.Type != BarcodeType.None)
                        OnMLBarcodeScannerResultsFound(data);
                }
            }
        }    
    }
}

#endif
