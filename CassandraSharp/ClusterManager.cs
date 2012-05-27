﻿// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace CassandraSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using CassandraSharp.Config;
    using CassandraSharp.EndpointStrategy;
    using CassandraSharp.Utils;

    public class ClusterManager
    {
        private static CassandraSharpConfig _config;

        private static IRecoveryService _recoveryService;

        private static ILog _logger;

        public static ICluster GetCluster(string name)
        {
            if (null == _config)
            {
                throw new InvalidOperationException("ClusterManager is not initialized");
            }

            if (null == name)
            {
                throw new ArgumentNullException("name");
            }

            ClusterConfig clusterConfig = GetClusterConfig(name);
            return GetCluster(clusterConfig);
        }

        public static ICluster GetCluster(ClusterConfig clusterConfig)
        {
            if (null == clusterConfig)
            {
                throw new ArgumentNullException("clusterConfig");
            }

            if (null == clusterConfig.Endpoints)
            {
                throw new ArgumentNullException("clusterConfig.Endpoints");
            }

            IBehaviorConfig behaviorConfig = clusterConfig.BehaviorConfig ?? new BehaviorConfig();
            TransportConfig transportConfig = clusterConfig.Transport ?? new TransportConfig();

            IRecoveryService recoveryService = FindRecoveryService(transportConfig.Recoverable);

            // create endpoints
            ISnitch snitch = Snitch.Factory.Create(clusterConfig.Endpoints.Snitch);

            IPAddress clientAddress = NetworkFinder.Find(Dns.GetHostName());
            IEnumerable<Endpoint> endpoints = GetEndpoints(clusterConfig.Endpoints, snitch, clientAddress);

            // create endpoint strategy
            IEndpointStrategy endpointsManager = Factory.Create(clusterConfig.Endpoints.Strategy, endpoints);
            IPool<IConnection> pool = Pool.Factory.Create("Stack", transportConfig.PoolSize);

            // get timestamp service
            ITimestampService timestampService = Timestamp.Factory.Create(clusterConfig.Endpoints.Timestamp);

            // create the cluster now
            ITransportFactory transportFactory = Transport.Factory.Create(transportConfig);
            return new Cluster(behaviorConfig, pool, transportFactory, endpointsManager, recoveryService, timestampService, _logger);
        }

        private static IEnumerable<Endpoint> GetEndpoints(EndpointsConfig config, ISnitch snitch, IPAddress clientAddress)
        {
            List<Endpoint> endpoints = new List<Endpoint>();
            foreach (string server in config.Servers)
            {
                IPAddress serverAddress = NetworkFinder.Find(server);
                if (null != serverAddress)
                {
                    string datacenter = snitch.GetDataCenter(serverAddress);
                    int proximity = snitch.ComputeDistance(clientAddress, serverAddress);
                    Endpoint endpoint = new Endpoint(server, serverAddress, datacenter, proximity);
                    endpoints.Add(endpoint);
                }
            }

            return endpoints;
        }

        private static ClusterConfig GetClusterConfig(string name)
        {
            ClusterConfig clusterConfig = (from config in _config.Clusters
                                           where config.Name == name
                                           select config).FirstOrDefault();
            if (null == clusterConfig)
            {
                string msg = string.Format("Can't find cluster configuration '{0}'", name);
                throw new KeyNotFoundException(msg);
            }

            return clusterConfig;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static IRecoveryService FindRecoveryService(bool recover)
        {
            if (! recover)
            {
                return null;
            }

            return _recoveryService;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Shutdown()
        {
            if (null != _recoveryService)
            {
                _recoveryService.SafeDispose();
                _recoveryService = null;
            }

            _config = null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Configure(CassandraSharpConfig config)
        {
            if (null != _config)
            {
                throw new InvalidOperationException("ClusterManager is already initialized");
            }

            if (null == config)
            {
                throw new ArgumentNullException("config");
            }

            _recoveryService = Recovery.Factory.Create(config.Recovery);

            _logger = Logger.Factory.Create(config.Logger);

            _config = config;
        }
    }
}