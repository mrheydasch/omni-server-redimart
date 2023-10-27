using System;

namespace LSOmni.DataAccess.Interface.BOConnection
{
    public interface ICustomLoyBO
    {
        #region Altria
        void AltriaLogEntryCreate(string storeId, string offerId, string cardId, int activityType, int channelType);
        #endregion
    }
}
