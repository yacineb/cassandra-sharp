﻿// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
// limitations under the License.
namespace CassandraSharp.Factory
{
    using System;
    using System.Collections.Generic;
    using CassandraSharp.Config;
    using CassandraSharp.EndpointStrategy;

    internal static class EndpointsConfigExtensions
    {
        public static IEndpointStrategy Create(this EndpointsConfig @this, string customType, IEnumerable<Endpoint> endpoints)
        {
            switch (@this.Strategy)
            {
                case EndpointStrategy.Custom:
                    if( null == customType)
                    {
                        throw new ArgumentNullException("EndpointsConfig.StrategyType");
                    }

                    Type strategyType = Type.GetType(customType);
                    if (null == strategyType)
                    {
                        string invalidTypeMsg = string.Format("'{0}' is not a valid type", strategyType);
                        throw new ArgumentException(invalidTypeMsg);
                    }

                    return (IEndpointStrategy) Activator.CreateInstance(strategyType, endpoints);

                case EndpointStrategy.Random:
                    return new RandomEndpointStrategy(endpoints);

                case EndpointStrategy.Nearest:
                    return new NearestEndpointStrategy(endpoints);
            }

            string msg = string.Format("Unknown strategy '{0}'", @this);
            throw new ArgumentException(msg);
        }
    }
}