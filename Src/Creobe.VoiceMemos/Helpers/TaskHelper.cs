using Creobe.VoiceMemos.Resources;
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;
using System;
using System.Reflection;
using System.Text;

namespace Creobe.VoiceMemos.Helpers
{
    public class TaskHelper
    {
        public static void ReportBug()
        {
            EmailComposeTask emailTask = new EmailComposeTask();

            var device = DeviceStatus.DeviceName;
            var manufacturer = DeviceStatus.DeviceManufacturer;
            var osVersion = Environment.OSVersion.Version;
            var fwVersion = DeviceStatus.DeviceFirmwareVersion;
            var hwVersion = DeviceStatus.DeviceHardwareVersion;
            
            StringBuilder body = new StringBuilder();
            body.AppendLine();
            body.AppendLine();
            body.AppendLine("Device Information");
            body.AppendLine("------------------");
            body.AppendLine("Name: " + device);
            body.AppendLine("Manufacturer: " + manufacturer);
            body.AppendLine("OS Version: " + osVersion);
            body.AppendLine("Firmware Version: " + fwVersion);
            body.AppendLine("Hardware Version: " + hwVersion);

            emailTask.To = AppResources.ReportBugEmail;
            emailTask.Subject = string.Format("Bug: Voice Memos v{0}", GetVersion());
            emailTask.Body = body.ToString();

            emailTask.Show();
        }

        public static void ContactSupport()
        {
            EmailComposeTask emailTask = new EmailComposeTask();

            var device = DeviceStatus.DeviceName;
            var manufacturer = DeviceStatus.DeviceManufacturer;
            var osVersion = Environment.OSVersion.Version;
            var fwVersion = DeviceStatus.DeviceFirmwareVersion;
            var hwVersion = DeviceStatus.DeviceHardwareVersion;

            StringBuilder body = new StringBuilder();
            body.AppendLine();
            body.AppendLine();
            body.AppendLine("App Information");
            body.AppendLine("---------------");
            body.AppendLine("Name: Voice Memos");
            body.AppendLine(string.Format("Version: {0}", GetVersion()));
            body.AppendLine();
            body.AppendLine("Device Information");
            body.AppendLine("------------------");
            body.AppendLine("Name: " + device);
            body.AppendLine("Manufacturer: " + manufacturer);
            body.AppendLine("OS Version: " + osVersion);
            body.AppendLine("Firmware Version: " + fwVersion);
            body.AppendLine("Hardware Version: " + hwVersion);

            emailTask.To = AppResources.ContactSupportEmail;
            emailTask.Body = body.ToString();

            emailTask.Show();
        }

        public static void FindOtherApps()
        {
            MarketplaceSearchTask marketPlaceSearchTask = new MarketplaceSearchTask();
            marketPlaceSearchTask.ContentType = MarketplaceContentType.Applications;
            marketPlaceSearchTask.SearchTerms = "Creobe Ltd.";
            marketPlaceSearchTask.Show();
        }

        public static void RateApp()
        {
            MarketplaceReviewTask marketPlaceReviewTask = new MarketplaceReviewTask();
            marketPlaceReviewTask.Show();
        }


        public static void FeatureRequest()
        {
            WebBrowserTask browserTask = new WebBrowserTask();
            browserTask.Uri = new Uri("https://creobe.uservoice.com/forums/233532-feature-requests");
            browserTask.Show();
        }

        public static void ShareMessaging(string link)
        {
            SmsComposeTask smsTask = new SmsComposeTask();
            smsTask.Body = link;
            smsTask.Show();
        }

        public static void ShareEmail(string link)
        {
            EmailComposeTask emailTask = new EmailComposeTask();
            emailTask.Subject = AppResources.ShareMethodEmailSubject;
            emailTask.Body = link;
            emailTask.Show();
        }

        public static void ShareSocial(string link, string title)
        {
            ShareLinkTask linkTask = new ShareLinkTask();
            linkTask.Title = title;
            linkTask.Message = AppResources.ShareMethodSocialMessage;
            linkTask.LinkUri = new Uri(link, UriKind.RelativeOrAbsolute);
            linkTask.Show();
            
        }

        private static string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            if (version != null)
                return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

            return "";
        }

    }
}
