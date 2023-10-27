using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSOmni.Common.Util;
using LSOmni.DataAccess.Interface.BOConnection;
using LSRetail.Omni.DiscountEngine.DataModels;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace LSOmni.BLL.Loyalty
{
    public class CustomLoyBLL : BaseLoyBLL
    {
        private ICustomLoyBO iBOCustomConnection = null;

        protected ICustomLoyBO BOCustom
        {
            get
            {
                if (iBOCustomConnection == null)
                    iBOCustomConnection = GetBORepository<ICustomLoyBO>(config.LSKey.Key, config.IsJson);
                return iBOCustomConnection;
            }
        }

        public CustomLoyBLL(BOConfiguration config, int timeoutInSeconds)
            : base(config, timeoutInSeconds)
        {
        }

        public CustomLoyBLL(BOConfiguration config) : base(config, 0)
        {
        }

        #region Altria
        public void AltriaLogEntryCreate(string storeId, string offerId, string cardId, int activityType, int channelType)
        {
            BOCustom.AltriaLogEntryCreate(storeId, offerId, cardId, activityType, channelType);
        }
        #endregion
    }
}


