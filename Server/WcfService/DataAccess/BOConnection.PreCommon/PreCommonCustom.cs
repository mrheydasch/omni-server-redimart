using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using LSOmni.Common.Util;
using LSOmni.DataAccess.BOConnection.PreCommon.Mapping;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace LSOmni.DataAccess.BOConnection.PreCommon
{
    public partial class PreCommonBase
    {

        public OmniWrapper2.OmniWrapper2 centralWS2 = null;
        public virtual string MyCustomFunction(string data)
        {
            // TODO: Here you put the code to access BC or NAV WS
            // Data Mapping is done under Mapping folder
            return "My return data + Incoming data: " + data;
        }

        #region Altria Phase II
        public List<PublishedOffer> PublishedOffersGet2(string cardId, string itemId, string storeId, Statistics stat)
        {
            logger.StatisticStartSub(true, ref stat, out int index);

            centralWS2 = new OmniWrapper2.OmniWrapper2();
            string url = config.SettingsGetByKey(ConfigKey.BOUrl);
            centralWS2.Url = url.Replace("RetailWebServices", "OmniWrapper2");
            centralWS2.Timeout = config.SettingsIntGetByKey(ConfigKey.BOTimeout) * 1000;  //millisecs,  60 seconds
            centralWS2.PreAuthenticate = true;
            centralWS2.AllowAutoRedirect = true;
            centralWS2.Credentials = new System.Net.NetworkCredential(
                                    config.Settings.FirstOrDefault(x => x.Key == ConfigKey.BOUser.ToString()).Value,
                                    config.Settings.FirstOrDefault(x => x.Key == ConfigKey.BOPassword.ToString()).Value);

            string respCode = string.Empty;
            string errorText = string.Empty;
            ContactMapping map = new ContactMapping(config.IsJson, LSCVersion);
            OmniWrapper2.RootGetDirectMarketingInfo root = new OmniWrapper2.RootGetDirectMarketingInfo();

            logger.Debug(config.LSKey.Key, "GetDirectMarketingInfo2 - CardId: {0}, ItemId: {1}", cardId, itemId);
            centralWS2.GetDirectMarketingInfo(ref respCode, ref errorText, XMLHelper.GetString(cardId), XMLHelper.GetString(itemId), XMLHelper.GetString(storeId), ref root);
            HandleWS2ResponseCode("GetDirectMarketingInfo", respCode, errorText, ref stat, index);
            logger.Debug(config.LSKey.Key, "GetDirectMarketingInfo2 Response - " + Serialization.ToXml(root, true));
            List<PublishedOffer> data = map.MapFromRootToPublishedOffers(root);
            logger.StatisticEndSub(ref stat, index);
            return data;
        }
        #endregion Altria Phase II
    }
}
