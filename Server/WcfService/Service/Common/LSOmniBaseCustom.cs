using System;
using System.Collections.Generic;

using LSOmni.BLL;
using LSOmni.BLL.Loyalty;
using LSOmni.Common.Util;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace LSOmni.Service
{
    public partial class LSOmniBase
    {
        public virtual string MyCustomFunction(string data)
        {
            CustomBLL myBLL = new CustomBLL(config);
            return myBLL.MyCustomFunction(data);
        }

        public virtual List<PublishedOffer> PublishedOffersGetByCardId2(string cardId, string itemId, string storeId)
        {
            if (cardId == null)
                cardId = string.Empty;
            if (itemId == null)
                itemId = string.Empty;

            Statistics stat = logger.StatisticStartMain(config, serverUri);

            try
            {
                logger.Debug(config.LSKey.Key, "PublishedOffersGetByCardId2 was called");
                logger.Debug(config.LSKey.Key, "itemId:{0} cardId:{1} storeId:{2}", itemId, cardId);

                OfferBLL bll = new OfferBLL(config, clientTimeOutInSeconds);
                CustomLoyBLL customLoyBll = new CustomLoyBLL(config, clientTimeOutInSeconds);

                List<PublishedOffer> list = bll.PublishedOffersGet(cardId, itemId, string.Empty, stat);
                foreach (PublishedOffer it in list)
                {
                    logger.Debug(config.LSKey.Key, "PublishedOffersGetByCardId2 about to call AltriaLogEntryCreate");
                    customLoyBll.AltriaLogEntryCreate(storeId, it.Id, cardId, 2, 3);
                    logger.Debug(config.LSKey.Key, "PublishedOffersGetByCardId2 returned from calling AltriaLogEntryCreate");
                    foreach (ImageView iv in it.Images)
                    {
                        iv.StreamURL = GetImageStreamUrl(iv);
                    }
                    foreach (OfferDetails od in it.OfferDetails)
                    {
                        od.Image.StreamURL = GetImageStreamUrl(od.Image);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                HandleExceptions(ex, "itemId:{0} cardId:{1}", itemId, cardId);
                return null; //never gets here
            }
            finally
            {
                logger.StatisticEndMain(stat);
            }
        }

    }
}
