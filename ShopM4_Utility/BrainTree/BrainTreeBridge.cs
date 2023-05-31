﻿using Braintree;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopM4_Utility.BrainTree
{
    public class BrainTreeBridge : IBrainTreeBridge
    {
        public SettingsBrainTree SettingsBrainTree { get; set; }
        public IBraintreeGateway gateway;
        public BrainTreeBridge(IOptions<SettingsBrainTree> options)
        {
            SettingsBrainTree = options.Value;
        }
        public IBraintreeGateway CreateGateWay()
        {
            return new BraintreeGateway(SettingsBrainTree.Environment,
                SettingsBrainTree.MerchantId, SettingsBrainTree.PublicKey,
                SettingsBrainTree.PrivateKey);
        }

        public IBraintreeGateway GetGateWay()
        {
            if (gateway==null)
            {
                gateway = CreateGateWay();
            }
            return gateway;
        }

    }
}
