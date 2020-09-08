using System;
using System.Threading.Tasks;
#if (DEBUG && BETA) || DEBUG
using MockIAPLib;
using Store = MockIAPLib;
#else
using Windows.ApplicationModel.Store;
#endif


namespace Creobe.VoiceMemos.Helpers
{
    public class LicenseHelper
    {
        public static bool IsPremium
        {
            get
            {
                ProductLicense license = CurrentApp.LicenseInformation.ProductLicenses["VoiceMemosPremium"];

                if (license != null && license.IsActive)
                    return true;

                return false;
            }
        }

        public async static Task<string> GetPremiumPriceAsync()
        {
            var listings = await CurrentApp.LoadListingInformationAsync();

            if (listings.ProductListings.ContainsKey("VoiceMemosPremium"))
            {
                return listings.ProductListings["VoiceMemosPremium"].FormattedPrice;
            }

            return null;
        }

        public static async Task<bool> UpgradeToPremiumAsync()
        {
            try
            {
                await CurrentApp.RequestProductPurchaseAsync("VoiceMemosPremium", false);

#if !DEBUG && !BETA
                if (IsPremium)
                    FlurryWP8SDK.Api.LogEvent("Premium_Purchased");
#endif

                return IsPremium;
            }
            catch
            {
                return false;
            }
        }

        public static void ClearMockIAP()
        {
#if (DEBUG && BETA) || DEBUG
            MockIAP.ClearCache();
#endif
        }
    }
}
