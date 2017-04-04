﻿using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3.Transactions;
using System;
using System.Threading.Tasks;

namespace Nethereum.Web3.Interceptors
{
    public class TransactionRequestToOfflineSignedTransactionInterceptor : RequestInterceptor
    {
        private readonly SignedTransactionManager signer;

        public TransactionRequestToOfflineSignedTransactionInterceptor(string privateKey, Web3 web3)
        { 
            signer = new SignedTransactionManager(web3.Client, privateKey);
        }

        public override async Task<object> InterceptSendRequestAsync<TResponse>(
            Func<RpcRequest, string, Task<TResponse>> interceptedSendRequestAsync, RpcRequest request,
            string route = null)
        {
            if (request.Method == "eth_sendTransaction")
            {
                var transaction = (TransactionInput) request.RawParameters[0];
                return await SignAndSendTransaction(transaction).ConfigureAwait(false);
            }
            return await base.InterceptSendRequestAsync(interceptedSendRequestAsync, request, route).ConfigureAwait(false);
        }

        public override async Task<object> InterceptSendRequestAsync<T>(
            Func<string, string, object[], Task<T>> interceptedSendRequestAsync, string method,
            string route = null, params object[] paramList)
        {
            if (method == "eth_sendTransaction")
            {
                var transaction = (TransactionInput) paramList[0];
                return await SignAndSendTransaction(transaction).ConfigureAwait(false);
            }
            return await base.InterceptSendRequestAsync(interceptedSendRequestAsync, method, route, paramList).ConfigureAwait(false);
        }

        private async Task<string> SignAndSendTransaction(TransactionInput transaction)
        {
            return await signer.SendTransactionAsync(transaction).ConfigureAwait(false);
        }

    }
}