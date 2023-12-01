using System;
using System.Collections.Generic;
using LSOmni.Common.Util;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace LSOmni.DataAccess.Interface.BOConnection
{
    public interface ICustomLoyBO
    {
        #region Altria Phase I
        void AltriaLogEntryCreate(string storeId, string offerId, string cardId, int activityType, int channelType);
        #endregion

        #region Altria Phase II
        List<PublishedOffer> PublishedOffersGet(string cardId, string itemId, string storeId, Statistics stat);
        #endregion
    }
}
